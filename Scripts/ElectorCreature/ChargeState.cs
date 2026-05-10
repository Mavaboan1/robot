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
        GD.Print($"Current Charge {_creature.currentElectroCharge}");
    }

    public override void PhysicsUpdate(float delta)
    {
        if (_creature.currentElectroCharge <= _creature.maxElectroCharge)
        {
            _creature.currentElectroCharge += chargeRate;
        }

        if (_creature.currentElectroCharge >= _creature.maxElectroCharge)
        {
            _creature.currentElectroCharge = _creature.maxElectroCharge;
            GD.Print($"End of Charge currently: {_creature.currentElectroCharge}");
            fsm.TransitionTo("Roaming");
        }
    }





}
