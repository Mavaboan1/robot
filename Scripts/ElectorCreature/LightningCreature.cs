using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class LightningCreature : CreatureBase
{

    [Export] public int ropeSpeed = 600;

    [Export(PropertyHint.Range, "0,1,0.01")] public double jumpChance = 0.1f;
    [Export] public JumpRope currentJumpRope;


}
