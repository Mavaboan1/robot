using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class LightningCreature : CharacterBody2D
{

    [Export] public Node stateMachine;
    [Export] public int maxHealt = 50;
    [Export] public int currentHealt = 50;
    [Export] public int damage = 10;
    [Export] public int speed = 100;
    [Export] public int ropeSpeed = 600;
    [Export] public int gravity = 900;

    [Export] public int currentElectroCharge = 0;
    [Export] public int maxElectroCharge = 100;
    [Export(PropertyHint.Range, "0,1,0.01")] public double jumpChance = 0.1f;
    [Export(PropertyHint.Range, "0,1,0.01")] public double chargeChance = 0.1f;


    [Export] public JumpRope currentJumpRope;

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
