using Godot;
using System;

public partial class Lightning : Area2D
{
    private void _on_body_entered(Node body)
    {
        if (body is Player player)
        {
            player.SetCharged(true);
        }
    }
}
