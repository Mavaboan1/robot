using Godot;
using System;

public partial class ChargeState : State
{
    [Export]
    public int chargeRate = 1;

    LightningCreature _creature;

    public override void Enter()
    {
        _creature = parent as LightningCreature;
        GD.Print($"Entered ChargeState with Current Charge {_creature.currentCharge}");
    }

    public override void PhysicsUpdate(float delta)
    {
        if (_creature.currentCharge <= _creature.maxCharge)
        {
            _creature.currentCharge += chargeRate;
        }

        if (_creature.currentCharge >= _creature.maxCharge)
        {
            _creature.currentCharge = _creature.maxCharge;
            GD.Print($"End of Charge currently: {_creature.currentCharge}");
            fsm.TransitionTo("Roaming");
        }
    }





}
