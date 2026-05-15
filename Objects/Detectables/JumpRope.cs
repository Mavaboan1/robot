using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class JumpRope : Area2D
{
    [Export] public int jumpCost = 30;
    [Export] public Marker2D jumpPoint;
    [Export] public Array<Area2D> jumpBody;



}
