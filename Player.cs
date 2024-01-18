using System.Collections.Generic;
using OG;
using Godot;
using System;

public partial class Player : CharacterBody3D
{
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;
    public double StartingHeight;
    public List<Node3D> Collisions = new List<Node3D>();
    public Spring PlayerSpring = new Spring();
    public Vector3 TargetPosition;

    private double _mass, _stiffness, _damping, _maxSpeed;
    [Export(PropertyHint.Range, "0.0, 250")]
    public double MaxSpeed{
        get { return _maxSpeed; }
        set{
            _maxSpeed = value;
        }
    }
    [Export(PropertyHint.Range, "0.0, 150")]
    public double Mass{
        get { return _mass; }
        set{
            _mass = value;
        }
    }
    [Export(PropertyHint.Range, "0.0, 25")]
    public double Stiffness{
        get { return _stiffness; }
        set{
            _stiffness = value;
        }
    }
    [Export(PropertyHint.Range, "0.0, 25")]
    public double Damping{
        get { return _damping; }
        set{
            _damping = value;
        }
    }

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready(){
        StartingHeight = GlobalPosition.Y;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        bool connected = false;
        Vector3 accumulate = Vector3.Zero;
        double hookshots = 0;
        foreach (var child in GetChildren())
        {
            if (child is HookShot hook)
            {
                if (hook.Connected)
                {
                    //GlobalPosition += hook.Transform.Basis.Z * hook.FiringSpeed * delta / 4;
                    //velocity += (GlobalPosition - hook.ContactPoint).Normalized() * hook.FiringSpeed * 1000;
                    accumulate += hook.ContactPoint;
                    hookshots += 1;

                    Vector3 FlatContact = hook.ContactPoint - GlobalPosition;
                    FlatContact.Y = 0;
                    hook.Hook.Position = hook.Hook.Transform.Basis.Z * FlatContact.Length();
                    connected = true;
                }
            }
        }
        if (IsOnFloor() || connected)
        {
            var pos = GlobalPosition;
            pos.Y = StartingHeight;
            GlobalPosition = pos;
        }
        else
        {
            velocity.Y -= gravity * delta;
        }

        if (connected)
        {
            TargetPosition = accumulate / hookshots;
            TargetPosition.Y = GlobalPosition.Y;
            Vector3 direction = (GlobalPosition - TargetPosition).Normalized();
            //PlayerSpring.Error = .2;
            //double dot = Math.Sign(direction.Dot(velocity.Normalized()));
            //PlayerSpring.ProcessVariable = (TargetPosition - GlobalPosition).Length();
            //var control = PlayerSpring.ControlVariable(delta);
            //GD.Print($"Error: {PlayerSpring.SetPoint - PlayerSpring.ProcessVariable}\nControl: {control}");
            velocity += Spring.GetForce(Mass, Stiffness, Damping, GlobalPosition, TargetPosition, delta, velocity);
        }

        // Reset Player.
        if (Input.IsActionJustPressed("ui_accept"))
        {
            GlobalPosition = new Vector3(-2, 1, 1);
            velocity = Vector3.Zero;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        if (!connected)
        {
            // Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            // Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
            // if (direction != Vector3.Zero)
            // {
            //     velocity.X = direction.X * Speed;
            //     velocity.Z = direction.Z * Speed;
            // }
            // else
            // {
            //     velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            //     velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
            // }
        }
        Velocity = velocity;
        MoveAndSlide();
    }
}
