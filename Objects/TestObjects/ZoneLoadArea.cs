using Godot;
using System;

public partial class ZoneLoadArea : Area2D
{
    [Export(PropertyHint.File, "*.tscn")] 
    public string SceneToLoad;
    
}
