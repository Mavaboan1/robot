using Godot;
using System;
using System.Collections.Generic;

public partial class RoamingState : State
{

    [Export] public RayCast2D ledgeCheckLeft;
    [Export] public RayCast2D ledgeCheckRight;
    private bool playerDetected = false;

    private LightningCreature _creature;
    private Random rand;
    private bool roaming;
    private bool inState;
    private int direction = -1;

    public override void Enter()
    {
        _creature = parent as LightningCreature;
        GD.Print($"Entered Roaming with charge: {_creature.currentCharge}");
        roaming = true;
        rand = new Random();
    }

    public override void Exit()
    {
        roaming = false;
    }

    public override void PhysicsUpdate(float delta)
    {
        // Transition to attack phase. Currently does nothing.
        if (playerDetected)
        {
            fsm.TransitionTo("Attack");
            }
        if (roaming)
            Move(delta);
    }

    // Moves left to right, movement changed if reaching edge.
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

    // Do some damage stuff.
    private void OnPlayerDetectionBodyEntered(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            playerDetected = true;
        }
    }

    private void OnNodeDetectAreaEntered(Area2D area)
    {
        if (roaming)
        {
            GD.Print("Collides with Area");
            // Random chance to charge up charge if below half charge.
            if (area.IsInGroup("ElectroCharger"))
            {
                double halfCharge = _creature.maxCharge * 0.5;
                double thisChance = rand.NextDouble();
                if (_creature.currentCharge <= (int)halfCharge)
                {
                    if (thisChance <= _creature.chargeChance)
                    {
                        GD.Print("TransistionPhase");
                        fsm.TransitionTo("Charge");
                    }
                }
            }

            // if enough charge, use the jump rope to go from this one to the connected one.
            if (area.IsInGroup("JumpRope"))
            {
                JumpRope jumpRope = area as JumpRope;
                _creature.currentJumpRope = jumpRope;
                var jumpCost = jumpRope.jumpCost;
                if (_creature.currentCharge > jumpCost)
                {
                    double thisChance = rand.NextDouble();
                    GD.Print($"Chance rolled: {thisChance}");
                    if (thisChance <= _creature.jumpChance)
                    {
                        
                        fsm.TransitionTo("Jump");
                    }
                }
            }
        }
    }

    // sets jumprope to null after jumping
    private void OnNodeDetectAreaExited(Area2D area)
    {
        if (roaming)
        {
            if (area.IsInGroup("JumpRope"))
            {
                _creature.currentJumpRope = null;
            }
        }
    }
}