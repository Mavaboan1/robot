using Godot;
using System;

public abstract partial class CreatureBase : CharacterBody2D
{
    [Export] public Node stateMachine;
    public enum chargeType
    {
        none,
        lightning,
        water,
    }
    [Export] public chargeType creatureType;
    [Export] public int maxHealt = 50;
    [Export] public int currentHealt = 50;
    [Export] public int damage = 10;
    [Export] public int speed = 100;
    [Export] public int gravity = 900;
    [Export] public int currentCharge = 0;
    [Export] public int maxCharge = 100;
    [Export(PropertyHint.Range, "0,1,0.01")] public double chargeChance = 0.1f;

    public string typeCharger = "";

    public override void _Ready()
    {
        switch (creatureType)
        {
            case chargeType.lightning:
                typeCharger = "ElectroCharger";
                break;
            case chargeType.water:
                typeCharger = "WaterCharger";
                break;
            case chargeType.none:
                typeCharger = "None";
                break;
            default:
                typeCharger = "";
                break;
        }
    }


    public void TakeDamage(int damage)
    {
        currentHealt -= damage;
        if (currentHealt <= 0)
        {
            QueueFree();
        }
    }

    // public void OnHurtBoxBodyEntered(CharacterBody2D body)
    // {
    //     if (body.HasMethod("TakeDamage"))
    //     {
    //         Player player = body as Player;
    //         player.TakeDamage(damage);
    //     }
    // }
}
