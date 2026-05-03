using Godot;
using System;

public partial class RoamingState : State
{

    [Export] public float speed = 200f;    
    [Export] public float gravity = 900f;
    [Export] public RayCast2D ledgeCheckLeft;
    [Export] public RayCast2D ledgeCheckRight;
    private bool playerDetected = false;

    private bool roaming;
    

    private int direction = -1;

    public override void Enter()
    {
        roaming = true;
    }

    public override void Exit()
    {
        roaming = false;
    }

    public override void PhysicsUpdate(float delta)
    {
        if (playerDetected)
        {
            fsm.TransitionTo("Attack");
        }
        if (roaming)
            Move(delta);
        
    }

    private void Move(float delta)
    {
        Vector2 velocity = parent.Velocity;
        bool isAtLedge = ledgeCheckLeft.IsColliding() && ledgeCheckRight.IsColliding();

        if (!parent.IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;
        }

        if (!isAtLedge)
        {
            direction *= -1;
        }

        velocity.X = direction * speed;
        parent.Velocity = velocity;

        parent.MoveAndSlide();

    }

    private void OnPlayerDetectionBodyEntered(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            playerDetected = true;
        }
    }




}
