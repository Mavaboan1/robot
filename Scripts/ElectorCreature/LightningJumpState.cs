using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Jumpstate jumps creature from one pilon to the connected one.
/// Should only be called in an fsm
/// </summary>
public partial class LightningJumpState : State
{
    
    private LightningCreature _creature;
    private Marker2D _jumpPoint;
    private Marker2D _targetPoint;
    private Vector2 _pointPos;
    private Vector2 _targetPos;

    private bool _finishedJump = false;
    private bool _finishedClimb = false;

    private enum ClimbPhase { Align, ClimbUp, Jump, ClimbDown }
    private ClimbPhase _phase = ClimbPhase.Align;
    

    public override void Enter()
    {
        // Setup when entering jumpState
        _phase = ClimbPhase.Align;
        _creature = parent as LightningCreature;
        GD.Print($"Entered JumpState with charge: {_creature.currentCharge}");
        _jumpPoint = FindMarker(_creature.currentJumpRope);
        _targetPoint = FindMarker(FindTarget(_creature.currentJumpRope.jumpBody));
        _pointPos = _jumpPoint.GlobalPosition;
        _targetPos = _targetPoint.GlobalPosition;
        _creature.currentCharge -= _creature.currentJumpRope.jumpCost;
        _creature.currentJumpRope = null;
    }

    // Finds the target connected to current jumprope
    private JumpRope FindTarget(Array<Area2D> bodies)
    {
        if (bodies.Count == 1)
        {
            return bodies[0] as JumpRope;
        }
        Random rand = new Random();
        GD.Print("We try and find a target");
        GD.Print(bodies.Count);
        int randTarget = rand.Next(0, bodies.Count);
        GD.Print(randTarget);
        return bodies[randTarget] as JumpRope;
    }

    public override void Exit()
    {
        _finishedJump = false;
        _jumpPoint = null;
        _targetPoint = null;
        _pointPos = Vector2.Zero;
        _targetPos = Vector2.Zero;
        _finishedClimb = false;
    }


    public override void PhysicsUpdate(float delta)
    {
        // Small statemachine for how jumping works
        switch (_phase)
        {
            case ClimbPhase.Align:
                Move(delta);
                break;
            case ClimbPhase.ClimbUp:
                Climb(delta);
                break;
            case ClimbPhase.Jump:
                Jump(delta);
                break;
            case ClimbPhase.ClimbDown:
                ClimbDown(delta);
                break;
        }
    }

    // Moves the creature so they align with the pilon
    private void Move(float delta)
    {
        float diff = _pointPos.X - parent.GlobalPosition.X;

        if (Math.Abs(diff) <= 5f)
        {
            parent.Velocity = Vector2.Zero;
            _phase = ClimbPhase.ClimbUp;
            return;
        }

        Vector2 velocity = parent.Velocity;
        velocity.X = Mathf.Sign(diff) * _creature.speed;
        velocity.Y = 0;
        parent.Velocity = velocity;

        parent.MoveAndSlide();
    }

    private void Climb(float delta)
    {
        if (parent.GlobalPosition.Y <= _pointPos.Y)
        {
            parent.GlobalPosition = _jumpPoint.GlobalPosition;
            _phase = ClimbPhase.Jump;
            return;
        }
        Vector2 velocity = parent.Velocity;
        velocity.X = 0;
        velocity.Y = -_creature.speed;
        parent.Velocity = velocity;

        parent.MoveAndSlide();
    }
    private void ClimbDown(float delta)
    {
        if (parent.IsOnFloor())
        {
            fsm.TransitionTo("Roaming");
            return;
        }
        Vector2 velocity = parent.Velocity;
        velocity.X = 0;
        velocity.Y = _creature.speed;
        parent.Velocity = velocity;

        parent.MoveAndSlide();

    }

    private void Jump(float delta)
    {
        parent.GlobalPosition = parent.GlobalPosition.MoveToward(_targetPos, _creature.ropeSpeed * delta);

        if (parent.GlobalPosition.DistanceTo(_targetPos) < 1f)
        {
            _phase = ClimbPhase.ClimbDown;
        }
    }

    private Marker2D FindMarker(JumpRope rope)
    {
        foreach (Node2D node in rope.GetChildren())
        {
            if (node is Marker2D)
            {
                return node as Marker2D;
            }
        }
        return null;

    }


}
