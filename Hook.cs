using Godot;

public partial class Hook : RigidBody3D
{
    public override void _IntegrateForces(PhysicsDirectBodyState3D state){
        for (var i = 0; i < state.GetContactCount(); i++){
            var obj = state.GetContactColliderObject(i) as Node3D;
            var Body = obj as CollisionObject3D;
            var Map = obj as GridMap;
            uint layer = 0;
            if(Body != null) layer |= Body.CollisionLayer;
            if(Map != null) layer |= Map.CollisionLayer;
            var Hook = GetParent() as HookShot;
            if((layer & 10) != 0 && Hook.Firing && !Hook.Player.Collisions.Contains(obj)){
                Hook.Firing = false;
                Hook.Retracting = true;
                GD.Print($"{obj.Name}");
                if((layer & 8) != 0 && Hook.Released ){
                    Hook.ContactPoint = state.GetContactLocalPosition(i);
                    Hook.HookedTarget = obj;
                    Hook.Collisions.Add(obj);
                    Hook.Player.Collisions.Add(obj);
                    Hook.Connected = true;
                    Hook.Released = false;
                }
            }
        }
    }

}
