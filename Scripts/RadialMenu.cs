using Godot;
using System;
using System.Collections.Generic;

public partial class RadialMenu : Node2D
{
    public List<int> Options = new List<int>();
    public int SelectedIndex = -1;

    [Export] public float Radius = 1000f;

    public override void _Draw()
    {
        if (Options.Count == 0)
            return;

        float sliceSize = Mathf.Tau / Options.Count;

        for (int i = 0; i < Options.Count; i++)
        {
            float startAngle = i * sliceSize;
            float endAngle = startAngle + sliceSize;

            int creatureType = Options[i];
            Color baseColor = GetColorForCreature(creatureType);

            Color color = (i == SelectedIndex)
                ? baseColor.Lightened(0.3f) with { A = 0.9f }
                : baseColor with { A = 0.4f };

            DrawArc(Vector2.Zero, Radius, startAngle, endAngle, 32, color, 20);
        }
    }
    private Color GetColorForCreature(int type)
{
    switch (type)
    {
        case 0: return new Color(1, 1, 1); // default
        case 1: return new Color(0, 0, 1); // blue
        case 2: return new Color(0, 1, 0); // green
        default: return new Color(1, 1, 1);
    }
}
}