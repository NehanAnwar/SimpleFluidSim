using Microsoft.Xna.Framework;

namespace FluidSimulation.Simulation;

public class Particle
{
    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Force;

    public float Radius = 0.2f;
    public float Mass = 1.0f;
    public float Density = 0f;

    public Particle(Vector3 position)
    {
        Position = position;
        Velocity = Vector3.Zero;
        Force = Vector3.Zero;
    }
}