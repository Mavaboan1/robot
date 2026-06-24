using Godot;
using System;

public partial class WaterTrigger : Area2D
{
    private void _on_body_entered(Node body)
    {
        if (body is Player player)
        {
            if (player.CurrentForm != FormType.Water)
            {
                player.SetInWater(true);
                return;
            }
            player.waterForm.EnterWater(player);
        }
    }

    private void _on_body_exited(Node body)
    {

        if (body is Player player)
        {
           if (player.CurrentForm != FormType.Water)
            {
                player.SetInWater(false);
                return;
            }
           player.waterForm.ExitWater(player);

        }
    }
    
}
