using Godot;

public partial class BubblePlatform : StaticBody2D
{
    [Export] public float LaunchSpeed = 1800f;
    [Export] public float LaunchTime = 0.25f;
    [Export] public float FinalScale = 10f;

    private Vector2 moveDirection = Vector2.Zero;
    private bool launching = true;
    private bool playerOnTop = false;
    private Player player;
    private Vector2 externalVelocity = Vector2.Zero;
    public override void _Ready()
    {
        Scale = new Vector2(2f, 2f);

        GetNode<Area2D>("TopArea").BodyEntered += OnTopEntered;
        GetNode<Area2D>("TopArea").BodyExited += OnTopExited;
        GetNode<Area2D>("SideArea").BodyEntered += OnSideEntered;

        var tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(FinalScale, FinalScale), 0.3f);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (launching)
        {
            GlobalPosition += moveDirection * LaunchSpeed * (float)delta;
        }
        GlobalPosition += externalVelocity * (float)delta;
        externalVelocity = externalVelocity.Lerp(Vector2.Zero, 0.05f);

        
    }

    public void Initialize(Vector2 direction)
    {
        moveDirection = direction.Normalized();

        GetTree().CreateTimer(LaunchTime).Timeout += () =>
        {
            launching = false;
        };
    }

    private void OnTopEntered(Node body)
    {
        if (body is Player p)
        {
            player = p;
            playerOnTop = true;
        }
    }

    private void OnTopExited(Node body)
    {
        if (body is Player)
        {
            Pop();
        }
    }

    private void OnSideEntered(Node body)
    {
        if (body is Player && !playerOnTop)
        {
            
            Pop();
            
        }
    }

    private void Pop()
    {
        GD.Print("Pop");

        CollisionShape2D solidCollision = GetNode<CollisionShape2D>("CollisionShape2D");
        solidCollision.SetDeferred("disabled", true);
        QueueFree();
    }
    public void Push(Vector2 direction, float force)
    {
        externalVelocity += direction.Normalized() * force;
    }
}