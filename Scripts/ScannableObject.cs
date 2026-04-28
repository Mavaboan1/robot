using Godot;
using System;

public partial class ScannableObject : Node2D
{
    [Export] public int CreatureType = 0;
    [Export] public int AbilityUnlock = 0;
    [Export] public bool Scanned = false;

    public void Scanning(Player player)
    {
        if (Scanned)
            return;

        Scanned = true;

        GD.Print($"Scanned! Type: {CreatureType}, Unlock: {AbilityUnlock}");

        player.scanForm.ReceiveScan(player, CreatureType);

        Modulate = new Color(0.5f, 0.5f, 0.5f);
    }
}