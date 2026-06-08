using Microsoft.Xna.Framework;

namespace FluidSimulation.Simulation;

public class BoxObstacle
{
    public Vector3 Min;
    public Vector3 Max;

    public BoxObstacle(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }

    public bool Contains(Vector3 point)
    {
        return point.X >= Min.X && point.X <= Max.X &&
               point.Y >= Min.Y && point.Y <= Max.Y &&
               point.Z >= Min.Z && point.Z <= Max.Z;
    }
}