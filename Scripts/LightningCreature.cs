using Godot;
using Godot.Collections;
using System;

public partial class LightningCreature : CharacterBody2D
{

    [Export] public Node stateMachine;
    /*
    [Export] public float speed = 200f;
    [Export] public float gravity = 900f;
    [Export] public RayCast2D ledgeCheckLeft;
    [Export] public RayCast2D ledgeCheckRight;

    private int direction = 1;
    */


    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        /*
        stateMachine._PhysicsProcess(delta);
        Vector2 velocity = Velocity;
        bool isAtLedge = ledgeCheckLeft.IsColliding() && ledgeCheckRight.IsColliding();

        if (!IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;
        }

        if (!isAtLedge)
        {
            direction *= -1;
        }

        velocity.X = direction * speed;
        Velocity = velocity;

        MoveAndSlide();
        */
    }




}
