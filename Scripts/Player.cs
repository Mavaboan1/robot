using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    
    [Export] public float BaseSpeed = 2000f;
    [Export] public float BaseJumpVelocity = -4000f;
    [Export] public float BaseSize = 10f;
    [Export] public float Gravity = 9000f;
    [Export] public float CoyoteTime = 0.12f;
    [Export] public float JumpBufferTime = 0.12f;
    [Export] public float JumpCutMultiplier = 0.45f;
    [Export] public float FallGravityMultiplier = 1.35f;
    [Export] public float LowJumpGravityMultiplier = 1.6f;
    [Export] public float GroundAcceleration = 18000f;
    [Export] public float GroundDeceleration = 22000f;
    [Export] public float AirAcceleration = 14000f;
    [Export] public float AirDeceleration = 12000f;
    [Export] public PackedScene BubblePlatformScene;
    [Export] public PackedScene AirGustScene;
    

    private ProgressBar airBar;
    private ProgressBar HealthBar;
    public int Maxhealth = 100;
    public int CurrentHealth;
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

    public HashSet<int> knownCreatures = new();
    public List<int> knownCreatureList = new();

    public ScanForm scanForm;
    public LightningForm lightningForm;
    public WaterForm waterForm;


    private bool inWater = false;

    private float coyoteTimer = 0f;
    private float jumpBufferTimer = 0f;

    [Export] public float WaterGravity = 1500f;
    [Export] public float WaterFloatForce = -2500f;
    [Export] public float MaxWaterFallSpeed = 600f;

    public HashSet<int> KnownCreatures => knownCreatures;
    public List<int> KnownCreatureList => knownCreatureList;
    public Area2D ScanArea => scanArea;
    private Area2D ElectricAttack;
    private Sprite2D ElectricAttackVisual;

    public override void _Ready()
    {
        playerSprite = GetNode<Sprite2D>("Sprite2D");
        indicator = GetNode<Sprite2D>("Indicator");
        scanArea = GetNode<Area2D>("ScanArea");
        transformCircle = GetNode<Node2D>("TransformCircle");
        radialMenu = GetNode<RadialMenu>("TransformCircle");
        airBar = GetNode<ProgressBar>("AirBar");
        HealthBar = GetNode<ProgressBar>("Healthbar");
        ElectricAttack = GetNode<Area2D>("ElectricAttack");
        ElectricAttack.Monitoring = false;
        ElectricAttackVisual = GetNode<Sprite2D>("ElectricAttack/AttackVisual");
        ElectricAttackVisual.Visible = false;

        CurrentHealth = Maxhealth;
        HealthBar.Value = CurrentHealth;

        radialMenu.Visible = false;
        indicator.Visible = false;
        airBar.Visible = false;

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

        CheckDeath();
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
        float dt = (float)delta;
        Vector2 velocity = Velocity;
        bool jumpPressed = Input.IsActionJustPressed("Player_Jump");
        bool jumpReleased = Input.IsActionJustReleased("Player_Jump");
        bool jumpHeld = Input.IsActionPressed("Player_Jump");
        bool onFloor = IsOnFloor();

        if (jumpPressed)
            jumpBufferTimer = JumpBufferTime;
        else
            jumpBufferTimer = Mathf.Max(jumpBufferTimer - dt, 0f);

        if (onFloor)
            coyoteTimer = CoyoteTime;
        else
            coyoteTimer = Mathf.Max(coyoteTimer - dt, 0f);

        if (inWater)
        {
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;

            velocity.Y += WaterGravity * dt;

            if (velocity.Y > 0)
                velocity.Y += WaterFloatForce * dt;

            velocity.Y = Mathf.Min(velocity.Y, MaxWaterFallSpeed);

            if (jumpPressed)
                velocity.Y = currentJumpVelocity * 0.75f;
        }
        else
        {
            if (!onFloor)
            {
                float gravityMultiplier = 1f;
                if (velocity.Y > 0)
                    gravityMultiplier = FallGravityMultiplier;
                else if (velocity.Y < 0 && !jumpHeld)
                    gravityMultiplier = LowJumpGravityMultiplier;

                velocity.Y += Gravity * gravityMultiplier * dt;
            }

            if (jumpBufferTimer > 0f && coyoteTimer > 0f)
            {
                velocity.Y = currentJumpVelocity;
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }

            if (jumpReleased && velocity.Y < 0)
                velocity.Y *= JumpCutMultiplier;
        }

        float direction = Input.GetAxis("Player_Left", "Player_Right");

        if (direction != 0)
            facingDirection = (int)Mathf.Sign(direction);

        float speed = currentSpeed;

        if (CurrentForm == FormType.Lightning && lightningForm.IsCharged)
            speed *= lightningForm.SpeedMultiplier;

        float targetSpeed = direction * speed;
        float acceleration;

        if (direction != 0)
            acceleration = onFloor ? GroundAcceleration : AirAcceleration;
        else
            acceleration = onFloor ? GroundDeceleration : AirDeceleration;

        velocity.X = Mathf.MoveToward(velocity.X, targetSpeed, acceleration * dt);

        Velocity = velocity;
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }
    public void SetInWater(bool value)
    {
        inWater = value;
    }
    public void CheckDeath()
    {
        if (CurrentHealth <= 0)
        {
            GD.Print("Yo you dead Fr Fr.");
        }
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


    public void SetAirBarVisible(bool visible)
    {
        airBar.Visible = visible;
    }

    public void SetAirBar(float value, float maxValue)
    {
        airBar.MaxValue = maxValue;
        airBar.Value = value;
    }
    public void TakeDamage(int value)
    {
        CurrentHealth -= value;
        HealthBar.Value = CurrentHealth;
        GD.Print("Health =", CurrentHealth);
    }
    public void DealDamage(int value)
    {
        GD.Print("deal damage");
    }
    public void AttackHitbox(int damage)
{
    UpdateAttackDirection();
    ElectricAttack.Monitoring = true;
        ElectricAttackVisual.Visible = true;


    foreach (Node2D body in ElectricAttack.GetOverlappingBodies())
    {
        if (body.HasMethod("TakeDamage"))
        {
            body.Call("TakeDamage", damage);
        }
    }

    ElectricAttack.Monitoring = false;
}
public void HideAttackVisual()
{
    ElectricAttackVisual.Visible = false;
}
private void UpdateAttackDirection()
{
    int direction = GetFacingDirection();

    // Mirror the entire attack Area2D.
    ElectricAttack.Scale = new Vector2(
        Mathf.Abs(ElectricAttack.Scale.X) * direction,
        ElectricAttack.Scale.Y
    );
}
}
