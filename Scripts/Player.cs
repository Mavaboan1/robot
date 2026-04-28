// Player.cs
using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    [Export] public float BaseSpeed = 200f;
    [Export] public float BaseJumpVelocity = -400f;
    [Export] public float BaseSize = 1f;
    [Export] public float Gravity = 900f;

    private Sprite2D playerSprite;
    private Sprite2D indicator;
    private Area2D scanArea;
    private RadialMenu radialMenu;
    private Node2D transformCircle;

    private float currentSpeed;
    private float currentJumpVelocity;
    private float currentSize;

    public FormType CurrentForm = FormType.Scan;
    private int facingDirection = 1;

    private bool isTransformMenuOpen = false;

    private HashSet<int> knownCreatures = new();
    private List<int> knownCreatureList = new();

    public ScanForm scanForm;
    public LightningForm lightningForm;
    public WaterForm waterForm;

    public HashSet<int> KnownCreatures => knownCreatures;
    public List<int> KnownCreatureList => knownCreatureList;
    public Area2D ScanArea => scanArea;

    public override void _Ready()
    {
        playerSprite = GetNode<Sprite2D>("Sprite2D");
        indicator = GetNode<Sprite2D>("Indicator");
        scanArea = GetNode<Area2D>("ScanArea");
        transformCircle = GetNode<Node2D>("TransformCircle");
        radialMenu = GetNode<RadialMenu>("TransformCircle");

        radialMenu.Visible = false;
        indicator.Visible = false;

        knownCreatures.Add(0);
        knownCreatureList.Add(0);

        currentSpeed = BaseSpeed;
        currentJumpVelocity = BaseJumpVelocity;
        currentSize = BaseSize;

        scanForm = new ScanForm();
        lightningForm = new LightningForm();
        waterForm = new WaterForm();
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMovement(delta);
        MoveAndSlide();

        HandleTransform();
        UpdateRadialSelection();
        FixRadialScale();

        switch (CurrentForm)
{
    case FormType.Scan:
        scanForm.Update(this, delta);
        break;

    case FormType.Lightning:
        lightningForm.Update(this, delta);
        break;

    case FormType.Water:
        waterForm.Update(this, delta);
        break;
}
    }

    private void HandleMovement(double delta)
    {
        Vector2 velocity = Velocity;

        if (!IsOnFloor())
            velocity.Y += Gravity * (float)delta;

        if (Input.IsActionJustPressed("Player_Jump") && IsOnFloor())
            velocity.Y = currentJumpVelocity;

        float direction = Input.GetAxis("Player_Left", "Player_Right");

        if (direction != 0)
            facingDirection = (int)Mathf.Sign(direction);

        float speed = currentSpeed;

        if (CurrentForm == FormType.Lightning && lightningForm.IsCharged)
            speed *= lightningForm.SpeedMultiplier;

        velocity.X = Mathf.Lerp(velocity.X, direction * speed, 0.15f);

        Velocity = velocity;
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    public void ApplyStats(float speed, float jump, float size)
    {
        currentSpeed = speed;
        currentJumpVelocity = jump;
        currentSize = size;

        Scale = new Vector2(size, size);
    }

    public void SetColor(Color color)
    {
        playerSprite.Modulate = color;
    }

    public void SetIndicator(bool visible, Color color)
    {
        indicator.Visible = visible;
        indicator.Modulate = color;
    }

    private void HandleTransform()
    {
        if (Input.IsActionJustPressed("Player_Transform"))
        {
            isTransformMenuOpen = true;
            Engine.TimeScale = 0.1f;
            transformCircle.Visible = true;
            radialMenu.Options = knownCreatureList;
        }

        if (Input.IsActionJustReleased("Player_Transform"))
        {
            if (isTransformMenuOpen)
                SelectTransform((FormType)GetSelectedCreature());
            CloseTransformMenu();
        }
    }

    private void SelectTransform(FormType form)
{
    if (form == CurrentForm)
    {
        CloseTransformMenu();
        return;
    }
    CurrentForm = form;

    if (CurrentForm != FormType.Water)
        waterForm.ResetState(this);

    if (CurrentForm != FormType.Lightning)
        lightningForm.SetCharged(this, false);
    switch (form)
    {
        case FormType.Scan:
            ApplyStats(scanForm.Speed, scanForm.Jump, scanForm.Size);
            SetColor(scanForm.FormColor);
            break;

        case FormType.Lightning:
            ApplyStats(lightningForm.Speed, lightningForm.Jump, lightningForm.Size);
            SetColor(lightningForm.FormColor);
            break;

        case FormType.Water:
            ApplyStats(waterForm.Speed, waterForm.Jump, waterForm.Size);
            SetColor(waterForm.FormColor);
            break;
    }
}

    private void CloseTransformMenu()
    {
        isTransformMenuOpen = false;
        Engine.TimeScale = 1f;
        transformCircle.Visible = false;
    }

    private int GetSelectedCreature()
    {
        Vector2 dir = (GetGlobalMousePosition() - GlobalPosition).Normalized();

        float angle = Mathf.Atan2(dir.Y, dir.X);
        if (angle < 0) angle += Mathf.Tau;

        int count = knownCreatureList.Count;
        float slice = Mathf.Tau / count;

        return knownCreatureList[Mathf.FloorToInt(angle / slice)];
    }

    private void UpdateRadialSelection()
    {
        if (!isTransformMenuOpen) return;

        Vector2 dir = (GetGlobalMousePosition() - GlobalPosition).Normalized();

        float angle = Mathf.Atan2(dir.Y, dir.X);
        if (angle < 0) angle += Mathf.Tau;

        int count = knownCreatureList.Count;
        float slice = Mathf.Tau / count;

        radialMenu.SelectedIndex = Mathf.FloorToInt(angle / slice);
        radialMenu.QueueRedraw();
    }

    private void FixRadialScale()
    {
        radialMenu.Scale = new Vector2(
            0.6f / Scale.X,
            0.6f / Scale.Y
        );
    }
}