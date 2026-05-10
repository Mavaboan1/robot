using Godot;

public partial class AirGust : Area2D
{
    [Export] public float PushForce = 5000f;

    private Vector2 direction = Vector2.Right;

    public override void _PhysicsProcess(double delta)
    {
        foreach (Node body in GetOverlappingBodies())
        {
            if (body is BubblePlatform bubble)
            {
                bubble.Push(direction, PushForce * (float)delta);
            }
        }
    }

    public void UpdateDirection(Vector2 dir)
    {
        direction = dir.Normalized();
        Rotation = direction.Angle();
    }
}