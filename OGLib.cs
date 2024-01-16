using Godot;
namespace OG
{
    public struct Spring
    {
        public double mass = 0;
        public double k = 0;
        public double damp = 0;
        public double force = 0;
        public Vector2 position = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;
        public Vector2 point1 = Vector2.Zero;
        public Vector2 point2 = Vector2.Zero;

        public Spring(double m, double _k, double d)
        {
            mass = m;
            k = _k;
            damp = d;
        }

        public Vector2 GetForce(Vector2 p1, Vector2 p2, double delta)
        {
            double force = (p1 - p2).Length() * k;
            Vector2 direction = (p1 - p2).Normalized();
            double acc = force / mass;
            Vector2 velocity = (direction * acc) * delta;

            return -force * direction - velocity * damp;
        }
    }
}
