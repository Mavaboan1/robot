using Godot;

public partial class LightningForm : Node
{
    public float Speed = 1600f;
    public float Jump = -4800f;
    public float Size = 12f;
    public Color FormColor = Colors.Blue;
    public bool IsCharged = false;

    public float SpeedMultiplier = 2f;

    [Export] public float BlitzSpeed = 10000f;
    [Export] public float BlitzDuration = 0.18f;

    private bool isBlitzing = false;
    private float blitzTimer = 0f;
    private Vector2 blitzDirection;

    public void Update(Player player, double delta)
    {
        HandleDash(player, delta);
    }

    private void HandleDash(Player player, double delta)
    {
        Vector2 velocity = player.Velocity;

        // currently dashing
        if (isBlitzing)
        {
            blitzTimer -= (float)delta;

            velocity.X = blitzDirection.X * BlitzSpeed;
            velocity.Y = 0;

            player.Velocity = velocity;

            if (blitzTimer <= 0)
                isBlitzing = false;

            return;
        }

        // start dash
        if (Input.IsActionJustPressed("Player_Blitz") && IsCharged)
        {
            blitzDirection = new Vector2(player.GetFacingDirection(), 0);

            isBlitzing = true;
            blitzTimer = BlitzDuration;

            SetCharged(player, false);
        }
    }

    public void SetCharged(Player player, bool value)
    {
        if (value && player.CurrentForm != FormType.Lightning)
        return;
        IsCharged = value;

        if (value)
            player.SetIndicator(true, Colors.Yellow);
        else
            player.SetIndicator(false, Colors.White);
    }
}