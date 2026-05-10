using Godot;
using System;

public partial class RoamingState : State
{

    // [Export] public float speed = 200f;    
    // [Export] public float gravity = 900f;
    [Export] public RayCast2D ledgeCheckLeft;
    [Export] public RayCast2D ledgeCheckRight;
    private bool playerDetected = false;

    private LightningCreature _creature;


    private bool roaming;
    

    private int direction = -1;

    public override void Enter()
    {
        _creature = parent as LightningCreature;
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
            velocity.Y += _creature.gravity * (float)delta;
        }

        if (!isAtLedge)
        {
            direction *= -1;
        }

        velocity.X = direction * _creature.speed;
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

    private void OnNodeDetectAreaEntered(Area2D area)
    {
        GD.Print($"Collided with {area}");
        GD.Print($"Group {area.GetGroups}");
        if (area.IsInGroup("ElectroCharger"))
        {
            double halfCharge = _creature.maxElectroCharge * 0.5;
            if (_creature.currentElectroCharge < (int)halfCharge)
            {
                fsm.TransitionTo("Charge");
            }
        }

        if (area.IsInGroup("JumpRope") && area.HasMeta("chargeCost"))
        {
            var costMeta = area.GetMeta("ChargeCost");
            if (_creature.currentElectroCharge > (int)costMeta)
            {
                fsm.TransitionTo("Jump");
            }

        }

    }

    private void _on_node_detect_area_entered(Area2D area)
    {
        GD.Print("Area entered weird");
    }

    private void OnNodeDetectAreaShapeEntered(Area2D area)
    {
        GD.Print("Area Shape entered");
    }

    private void OnNodeDetectBodyEntered(Node2D body)
    {
        GD.Print("Body entered");
    }




}
