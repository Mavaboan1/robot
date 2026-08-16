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
    [Export] public float ComboResetTime = 0.45f;
    [Export] public float AttackDuration = 0.18f;

    private int comboStep = 0;
    private float comboTimer = 0f;
    private float attackTimer = 0f;
    private bool isAttacking = false;

    public void Update(Player player, double delta)
    {
        HandleDash(player, delta);
        HandleAttack(player,delta);
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
    private void HandleAttack(Player player, double delta)
{
    float dt = (float)delta;

    if (comboTimer > 0)
        comboTimer -= dt;
    else
        comboStep = 0;

    if (isAttacking)
    {
        attackTimer -= dt;

        if (attackTimer <= 0)
            {
            isAttacking = false;
            player.HideAttackVisual();
            }
            

    }

    if (Input.IsActionJustPressed("Player_PrimFire"))
    {
        StartAttack(player);
    }
}

private void StartAttack(Player player)
{

    comboStep++;

    if (comboStep > 3)
        comboStep = 1;

    comboTimer = ComboResetTime;
    attackTimer = AttackDuration;
    isAttacking = true;

    switch (comboStep)
    {
        case 1:
            GD.Print("Lightning attack 1");
            player.AttackHitbox(5);
            break;

        case 2:
            GD.Print("Lightning attack 2");
            player.AttackHitbox(6);
            break;

        case 3:
            GD.Print("Lightning finisher");
            player.AttackHitbox(10);
            comboStep = 0;
            break;
    }
}
}