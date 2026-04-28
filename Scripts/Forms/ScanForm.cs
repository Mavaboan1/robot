// ScanForm.cs
using Godot;

public partial class ScanForm : Node
{
    public float Speed = 200f;
    public float Jump = -400f;
    public float Size = 1f;
    public Color FormColor = Colors.White;
    public void Update(Player player, double delta)
    {
        if (!Input.IsActionJustPressed("Player_Ability1"))
            return;

        DoScan(player);
        PlayScanPulse(player);
    }

    private void DoScan(Player player)
    {
        var bodies = player.ScanArea.GetOverlappingBodies();

        foreach (Node body in bodies)
        {
            if (body.IsInGroup("Scannable") && body is ScannableObject scannable)
                scannable.Scanning(player);
        }
    }

    public void ReceiveScan(Player player, int creatureType)
    {
        if (player.KnownCreatures.Add(creatureType))
            player.KnownCreatureList.Add(creatureType);
    }

    private void PlayScanPulse(Player player)
    {
        Sprite2D pulse = player.GetNode<Sprite2D>("ScanPulse");

        pulse.Visible = true;
        pulse.Scale = new Vector2(0.05f, 0.05f);
        pulse.Modulate = Colors.White;

        var tween = player.CreateTween();
        tween.TweenProperty(pulse, "scale", new Vector2(0.4f, 0.4f), 0.4f);
        tween.TweenProperty(pulse, "modulate:a", 0f, 0.4f);
        tween.TweenCallback(Callable.From(() => pulse.Visible = false));
    }
}