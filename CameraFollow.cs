using Godot;
using System;

public partial class CameraFollow : Camera3D
{
    [Export]
    public Player Target;

    private double _mass, _stiffness, _damping, _maxSpeed;
    [Export(PropertyHint.Range, "0.0, 250")]
    public double MaxSpeed;

    [Export(PropertyHint.Range, "0.0, 150")]
    public double Mass;

    [Export(PropertyHint.Range, "0.0, 25")]
    public double Stiffness;

    [Export(PropertyHint.Range, "0.0, 250")]
    public double Damping;

    [Export(PropertyHint.Range, "0.0, 150")]
    public double Height = 30;

    private Vector3 velocity;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
	{
        if(Target == null) return;
        var TargetPosition = Target.TargetPosition;
        TargetPosition.Y = Height;
        velocity += OG.Spring.GetForce(Mass, Stiffness, Damping, GlobalPosition, TargetPosition, delta, velocity) * MaxSpeed;

        GlobalPosition += velocity;

    }
}
