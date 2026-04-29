using Godot;

public class WaterForm
{
    public float Speed = 400f;
    public float Jump = -480f;
    public float Size = 2f;
    public Color FormColor = Colors.Green;
    private float air = NormalMaxAir;
    private float maxAir = NormalMaxAir;

    private const float NormalMaxAir = 50f;
    private const float StoredAirMaxAir = 100f;
    private const float AirDrainSpeed = 10f;
    private const float AirRefillSpeed = 10f;
    private BubblePlatform activeBubble;
    public enum WaterState
{
    Empty,
    Air,
    Water
}
public WaterState CurrentState = WaterState.Empty;

private bool isInWater = false;
    public void Update(Player player, double delta)
    {
             HandleAbility(player);
             HandleAir(player, delta);
             HandleBubbleAbility(player);


    }
    private void HandleAbility(Player player)
{
    if (!Input.IsActionJustPressed("Player_Ability1"))
        return;

    if (CurrentState != WaterState.Empty)
        return;

    if (isInWater)
    {
        CurrentState = WaterState.Water;
        player.SetIndicator(true, new Color(0, 0, 1)); 

        GD.Print("Lung form filled with WATER");
    }
    else
    {
        CurrentState = WaterState.Air;
        player.SetIndicator(true, new Color(0.4f, 0.8f, 1f)); 
        maxAir = StoredAirMaxAir;
        air = maxAir;
        player.SetAirBarVisible(false);


        GD.Print("lung filled with AIR");
    }
}
public void SetInWater(bool value)
{
    isInWater = value;
}
public void ResetState(Player player)
{
    CurrentState = WaterState.Empty;
    maxAir = NormalMaxAir;
    air = Mathf.Min(air, maxAir);
    player.SetAirBarVisible(false);

    player.SetIndicator(false, Colors.White);

}
public void EnterWater(Player player)
    {
        isInWater = true;

        maxAir = CurrentState == WaterState.Air
            ? StoredAirMaxAir
            : NormalMaxAir;

         air = Mathf.Min(air, maxAir);

        player.SetAirBarVisible(true);
        player.SetAirBar(air, maxAir);
    }
public void ExitWater(Player player)
    {
        isInWater = false;
    }
    private void HandleAir(Player player, double delta)
{
    if (isInWater)
    {
        air -= AirDrainSpeed * (float)delta;
        air = Mathf.Max(air, 0);

        player.SetAirBarVisible(true);
        player.SetAirBar(air, maxAir);

        if (air <= 0)
            GD.Print("Out of air!");

        return;
    }

    // Refill air after leaving water
    if (air < maxAir)
    {
        air += AirRefillSpeed * (float)delta;
        air = Mathf.Min(air, maxAir);

        player.SetAirBarVisible(true);
        player.SetAirBar(air, maxAir);

        if (air >= maxAir)
            player.SetAirBarVisible(false);
    }
}
private void HandleBubbleAbility(Player player)
{
    if (!Input.IsActionJustPressed("Player_SecondFire"))
        return;

    if (CurrentState != WaterState.Water)
        return;

    Vector2 direction = (player.GetGlobalMousePosition() - player.GlobalPosition).Normalized();

    SpawnBubblePlatform(player, direction);
    // consume stored water
    CurrentState = WaterState.Empty;
    player.SetIndicator(false, Colors.White);
}
private void SpawnBubblePlatform(Player player, Vector2 direction)
{
    if (player.BubblePlatformScene == null)
    {
        GD.Print("BubblePlatformScene is missing!");
        return;
    }

    // Kill old bubble
    if (activeBubble != null && GodotObject.IsInstanceValid(activeBubble))
    {
        activeBubble.QueueFree();
    }

    BubblePlatform bubble = player.BubblePlatformScene.Instantiate<BubblePlatform>();

    player.GetParent().AddChild(bubble);

    Vector2 spawnPos = player.GlobalPosition + direction * 80f;
    bubble.GlobalPosition = spawnPos;

    bubble.Initialize(direction);

    activeBubble = bubble;
}
}
