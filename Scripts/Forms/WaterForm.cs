using Godot;

public class WaterForm
{
    public float Speed = 400f;
    public float Jump = -480f;
    public float Size = 2f;
    public Color FormColor = Colors.Green;
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
    player.SetIndicator(false, Colors.White);

}
}
