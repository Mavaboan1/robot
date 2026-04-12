using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    [Export] public float Speed = 200f;
    [Export] public float JumpVelocity = -400f;
    [Export] public float Gravity = 900f;
    private Area2D scanArea;
    private RadialMenu radialMenu;
    private bool isTransformMenuOpen = false;

    private HashSet<int> knownCreatures = new HashSet<int>();
    private List<int> knownCreatureList = new List<int>();
    private Node2D transformCircle;

    public override void _Ready()
    {
        scanArea = GetNode<Area2D>("ScanArea");
        knownCreatures.Add(0);
        knownCreatureList.Add(0);
        transformCircle = GetNode<Node2D>("TransformCircle");
        radialMenu = GetNode<RadialMenu>("TransformCircle");
        radialMenu.Visible = false;

    }
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        
        if (!IsOnFloor())
            velocity.Y += Gravity * (float)delta;

       
        if (Input.IsActionJustPressed("Player_Jump") && IsOnFloor())
            velocity.Y = JumpVelocity;

        
        float direction = Input.GetAxis("Player_Left", "Player_Right");
        velocity.X = direction * Speed;

        Velocity = velocity;
        MoveAndSlide();
        Scan();
        HandleTransform();
        UpdateRadialSelection();
    }
    public void Scan()
    {
    if (Input.IsActionPressed("Player_Scan"))
    {
        
        scanArea.Monitoring = true;

        var bodies = scanArea.GetOverlappingBodies();

        foreach (Node body in bodies)
        {
            if (body.IsInGroup("Scannable"))
            {
                if (body is ScannableObject scannable)
                    {
                        scannable.Scanning(this);}
            }
        }
    }
    else
    {
        scanArea.Monitoring = false;
    }
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
    GD.Print("Selected: " + option);

    var sprite = GetNode<Sprite2D>("Sprite2D");

    switch (option)
    {
        case 0:
            sprite.Modulate = new Color(1, 1, 1); // default (white)
            break;

        case 1:
            sprite.Modulate = new Color(0, 0, 1); // blue
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
    }

