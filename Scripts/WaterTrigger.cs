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
                GD.Print("You Cannot Swim");
                return;
            }
            player.waterForm.EnterWater(player);
        }
    }

    private void _on_body_exited(Node body)
    {
                GD.Print("sss");

        if (body is Player player)
        {
            player.waterForm.ExitWater(player);

        }
    }
    
}
