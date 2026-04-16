using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    [Export] public float BaseSpeed = 200f;
    [Export] public float BaseJumpVelocity = -400f;
    [Export] public float BaseSize = 1f;
    private Sprite2D playerSprite;

    private float currentSpeed;
    private float currentJumpVelocity;
    private float currentSize;
    private int currentForm = 0;

    [Export] public float Gravity = 900f;
    private Area2D scanArea;
    private RadialMenu radialMenu;
    private bool isTransformMenuOpen = false;

    private HashSet<int> knownCreatures = new HashSet<int>();
    private List<int> knownCreatureList = new List<int>();
    private Node2D transformCircle;
    private Sprite2D scanPulse;

    public override void _Ready()
    {
        scanArea = GetNode<Area2D>("ScanArea");
        knownCreatures.Add(0);
        playerSprite = GetNode<Sprite2D>("Sprite2D");
        knownCreatureList.Add(0);
        transformCircle = GetNode<Node2D>("TransformCircle");
        radialMenu = GetNode<RadialMenu>("TransformCircle");
        scanPulse = GetNode<Sprite2D>("ScanPulse");
        radialMenu.Visible = false;
        scanArea.Monitoring = true;

        currentSpeed = BaseSpeed;
        currentJumpVelocity = BaseJumpVelocity;
        currentSize = BaseSize;

    }
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        
        if (!IsOnFloor())
            velocity.Y += Gravity * (float)delta;

       
        if (Input.IsActionJustPressed("Player_Jump") && IsOnFloor())
            velocity.Y = currentJumpVelocity;
        
        float direction = Input.GetAxis("Player_Left", "Player_Right");
        velocity.X = direction * currentSpeed;
        Velocity = velocity;
        MoveAndSlide();
        Scan();
        HandleTransform();
        UpdateRadialSelection();
        LateRadialFix();
    }
    public void Scan()
{
    if (currentForm != 0)
        return;
    if (Input.IsActionJustPressed("Player_Scan"))
    {
        DoScan();
        PlayScanPulse();
    }
}
private void DoScan()
{
    var bodies = scanArea.GetOverlappingBodies();

    foreach (Node body in bodies)
    {
        if (body.IsInGroup("Scannable"))
        {
            if (body is ScannableObject scannable)
            {
                scannable.Scanning(this);
            }
        }
    }
}
private void PlayScanPulse()
{
    scanPulse.Visible = true;
    scanPulse.Scale = new Vector2(0.05f, 0.05f);
    scanPulse.Modulate = new Color(1, 1, 1, 1);

    var tween = CreateTween();

    tween.TweenProperty(scanPulse, "scale", new Vector2(0.4f, 0.4f), 0.4f);
    tween.TweenProperty(scanPulse, "modulate:a", 0f, 0.4f);

    tween.TweenCallback(Callable.From(() =>
    {
        scanPulse.Visible = false;
    }));
}

    public void ReceiveScan(int creatureType, int abilityUnlock)
{
    if (knownCreatures.Add(creatureType))
    {
        knownCreatureList.Add(creatureType);
        GD.Print($"New creature discovered: {creatureType}");
    }
    else
    {
        GD.Print($"Creature {creatureType} already known");
    }
}

    public void HandleTransform()
{
    // OPEN menu
    if (Input.IsActionJustPressed("Player_Transform"))
    {
        isTransformMenuOpen = true;
        Engine.TimeScale = 0.1f;
        transformCircle.Visible = true;
        radialMenu.Options = knownCreatureList;

    }

    // RELEASE → select
    if (Input.IsActionJustReleased("Player_Transform"))
    {
        if (isTransformMenuOpen)
        {
            int selected = GetSelectedCreature();
            SelectTransform(selected);
        }

        CloseTransformMenu();
    }
}
private void SelectTransform(int option)
{
    currentForm = option;
    GD.Print("Selected: " + option);

    var sprite = playerSprite;
    switch (option)
    {
        case 0: 
            ApplyStats(BaseSpeed, BaseJumpVelocity, BaseSize);
            sprite.Modulate = new Color(1, 1, 1); 
            break;

        case 1: 
            ApplyStats(
                BaseSpeed * 0.8f,          
                BaseJumpVelocity * 1.2f,   
                BaseSize * 2f            
            );
            sprite.Modulate = new Color(0, 0, 1); 
            break;

        case 2: 
            ApplyStats(
                BaseSpeed * 2f,          
                BaseJumpVelocity * 0.7f,   
                BaseSize * 0.5f            
            );
            sprite.Modulate = new Color(0, 1, 0); 
            break;
    }

    CloseTransformMenu();
}
private void CloseTransformMenu()
{
    isTransformMenuOpen = false;
    Engine.TimeScale = 1f;
    transformCircle.Visible = false;

}
private int GetSelectedCreature()
{
    Vector2 mousePos = GetGlobalMousePosition();
    Vector2 direction = (mousePos - GlobalPosition).Normalized();

    float angle = Mathf.Atan2(direction.Y, direction.X); // -PI to PI

    // Convert to 0 → 2PI
    if (angle < 0)
        angle += Mathf.Pi * 2;

    int count = knownCreatureList.Count;

    float sliceSize = (Mathf.Pi * 2) / count;

    int index = Mathf.FloorToInt(angle / sliceSize);

    GD.Print($"Selected creature: {knownCreatureList[index]}");

    return knownCreatureList[index];
}
private void UpdateRadialSelection()
{
    if (!isTransformMenuOpen)
        return;

    Vector2 mousePos = GetGlobalMousePosition();
    Vector2 dir = (mousePos - GlobalPosition).Normalized();

    float angle = Mathf.Atan2(dir.Y, dir.X);
    if (angle < 0)
        angle += Mathf.Tau;

    int count = knownCreatureList.Count;
    float sliceSize = Mathf.Tau / count;

    int index = Mathf.FloorToInt(angle / sliceSize);

    radialMenu.SelectedIndex = index;
    radialMenu.QueueRedraw();
}
private void LateRadialFix()
{
    if (radialMenu == null) return;

    radialMenu.Scale = new Vector2(
        0.6f / Scale.X,
        0.6f / Scale.Y
    );
}
private void ApplyStats(float speed, float jump, float size)
{
    currentSpeed = speed;
    currentJumpVelocity = jump;
    currentSize = size;

    // Scale the player visually
    Scale = new Vector2(currentSize, currentSize);
}
    }

