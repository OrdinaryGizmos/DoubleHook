using System;
using System.Collections.Generic;
using Godot;

public partial class HookShot : Node3D
{
    [Export]
    public Player Player;

    [Export]
    public double FiringSpeed = 150;

    [Export]
    public double Length = 20;

    [Export]
    public String trigger = "left_click";

    public bool Firing;
    public bool Retracting;
    public bool Connected;
    public bool Released = true;
    public Vector3 ContactPoint;
    public Node3D HookedTarget;
    public RigidBody3D Hook;
    public List<Node3D> Collisions = new List<Node3D>();
    public OG.Spring Force;

    public ShaderMaterial Mat;

    public override void _Ready()
    {
        Hook = GetNode<RigidBody3D>("Hook");
        Mat = Hook.GetNode<MeshInstance3D>("HookMesh").GetActiveMaterial(0) as ShaderMaterial;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustReleased(trigger))
        {
            Connected = false;
            Released = true;
            Firing = false;
            Retracting = true;
            foreach(var col in Collisions){
                Player.Collisions.Remove(col);
            }
            Collisions.Clear();
        }

        if (Input.IsActionJustPressed(trigger) && !Firing && !Connected && !Retracting)
        {
            var MousePos = GetViewport().GetMousePosition();
            var PlayerScreenPos = GetViewport()
                .GetCamera3D()
                .UnprojectPosition(Player.GlobalPosition);
            var Direction = (MousePos - PlayerScreenPos).Normalized();
            var YRotate = Vector2.Down.AngleTo(Direction);
            var CurrentRotation = Rotation;
            CurrentRotation.Y = -YRotate;
            Rotation = CurrentRotation;
            GetNode<CollisionShape3D>("Hook/HookshotCollider").Disabled = false;
            GetNode<RigidBody3D>("Hook").Position = Vector3.Zero;
            Firing = true;
        }

        GlobalPosition = Player.GlobalPosition;
        if (Retracting)
        {
            if (!Connected)
            {
                Hook.Position -= Hook.Transform.Basis.Z * FiringSpeed * delta / 4;
            } else{
                if ((Hook.GlobalPosition - Player.GlobalPosition).Length() < 3)
                {
                    Retracting = false;
                    var height = Hook.Position.Y;
                    Hook.Position = new Vector3(0, height, -1);
                }
            }
            if (Hook.Position.Z < 0)
            {
                Retracting = false;
            }
        }
        if (Firing)
        {
            Hook.GetNode<CollisionShape3D>("HookshotCollider").Disabled = false;
            Hook.Position += Hook.Transform.Basis.Z * FiringSpeed * delta;
            if (Hook.Position.Z > Length)
            {
                Firing = false;
                Retracting = true;
            }
        }
        else
        {
            GetNode<CollisionShape3D>("Hook/HookshotCollider").Disabled = true;
        }
        Mat.SetShaderParameter(
            "Portal_Global_Origin",
            new Vector3(0, 538 - Hook.Position.Z, 0)
        );
    }
}
