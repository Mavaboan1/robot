using Godot;
using System;
using System.Collections.Generic;

public partial class RadialMenu : Node2D
{
    public List<int> Options = new List<int>();
    public int SelectedIndex = -1;

    [Export] public float Radius = 100f;

    public override void _Draw()
    {
        if (Options.Count == 0)
            return;

        float sliceSize = Mathf.Tau / Options.Count;

        for (int i = 0; i < Options.Count; i++)
        {
            float startAngle = i * sliceSize;
            float endAngle = startAngle + sliceSize;

            Color color = (i == SelectedIndex)
                ? new Color(0, 1, 1, 0.8f) // highlighted
                : new Color(1, 1, 1, 0.3f);

            DrawArc(Vector2.Zero, Radius, startAngle, endAngle, 32, color, 20);
        }
    }
}