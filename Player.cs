using System.Collections.Generic;
using Godot;

public partial class Player : CharacterBody3D
{
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;
    public double StartingHeight;
    public List<Node3D> Collisions = new List<Node3D>();

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready(){
        StartingHeight = GlobalPosition.Y;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        bool connected = false;
        foreach (var child in GetChildren())
        {
            if (child is HookShot hook)
            {
                if (hook.Connected)
                {
                    //GlobalPosition += hook.Transform.Basis.Z * hook.FiringSpeed * delta / 4;
                    //velocity += (GlobalPosition - hook.ContactPoint).Normalized() * hook.FiringSpeed * 1000;
                    Vector3 FlatContact = hook.ContactPoint - GlobalPosition;
                    FlatContact.Y = 0;
                    if (FlatContact.LengthSquared() > 1)
                    {
                        GlobalPosition += FlatContact.Normalized() * hook.FiringSpeed * delta / 4;
                        hook.Hook.Position = hook.Hook.Transform.Basis.Z * FlatContact.Length();
                    }
                    connected = true;
                }
            }
        }

        if (IsOnFloor())
        {
            var pos = GlobalPosition;
            pos.Y = StartingHeight;
            GlobalPosition = pos;
        }
        else
        {
            if (!connected)
            {
                velocity.Y -= gravity * delta;
            }
        }

        // Handle Jump.
        // if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        //     velocity.Y = JumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        if (!connected)
        {
            Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
            if (direction != Vector3.Zero)
            {
                velocity.X = direction.X * Speed;
                velocity.Z = direction.Z * Speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
                velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
            }
        }
        Velocity = velocity;
        MoveAndSlide();
    }
}
