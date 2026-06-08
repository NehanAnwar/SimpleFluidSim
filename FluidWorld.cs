using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
namespace FluidSimulation.Simulation;

public class FluidWorld
{
    public List<BoxObstacle> Obstacles { get; } = new();
    public List<Particle> Particles { get; } = new();

    public Vector3 Gravity = new Vector3(0, -9.8f, 0);

    public float RepulsionStrength = 20f;
    public float Viscosity = 0.01f;
    public float InteractionRadius = 0.4f;

    public Vector3 MinBounds = new Vector3(-2, 0, -1);
    public Vector3 MaxBounds = new Vector3(2, 6, 1);

    private readonly Dictionary<Point3, List<int>> grid = new();

    private struct Point3
    {
        public int X;
        public int Y;
        public int Z;

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }


    public void AddParticle(Vector3 position)
    {
        Particles.Add(new Particle(position));
    }

    public void Update(float dt)
    {
        foreach (var p in Particles)
        {
            p.Force = Gravity * p.Mass;
            p.Density = 0f;
        }
        RebuildGrid();
        ApplyParticleForces();

        foreach (var p in Particles)
        {
            Vector3 acceleration = p.Force / p.Mass;

            p.Velocity += acceleration * dt;
            p.Position += p.Velocity * dt;

            p.Velocity *= 0.995f;

            ResolveWallCollisions(p);
            ResolveObstacleCollisions(p);
        }
    }


    private Point3 GetCell(Vector3 position)
{
    float cellSize = InteractionRadius;

    return new Point3(
        (int)Math.Floor(position.X / cellSize),
        (int)Math.Floor(position.Y / cellSize),
        (int)Math.Floor(position.Z / cellSize)
    );
}

    private void RebuildGrid()
    {
        grid.Clear();

        for (int i = 0; i < Particles.Count; i++)
        {
            Point3 cell = GetCell(Particles[i].Position);

            if (!grid.TryGetValue(cell, out var list))
            {
                list = new List<int>();
                grid[cell] = list;
            }

            list.Add(i);
        }
    }

    private void ApplyPairForce(int i, int j)
    {
        Particle a = Particles[i];
        Particle b = Particles[j];

        Vector3 offset = b.Position - a.Position;
        float distance = offset.Length();

        if (distance <= 0.0001f || distance > InteractionRadius)
            return;

        float closeness = 1f - distance / InteractionRadius;

        a.Density += closeness;
        b.Density += closeness;

        Vector3 direction = offset / distance;
        float overlap = InteractionRadius - distance;

        Vector3 repulsion = direction * overlap * RepulsionStrength;

        a.Force -= repulsion; //Newton's 3nd law
        b.Force += repulsion;

        Vector3 relativeVelocity = b.Velocity - a.Velocity;
        Vector3 viscosityForce = relativeVelocity * Viscosity; //Makes motion "grittier", eventually leads to a full loss of entropy with non-zero viscosity

        a.Force += viscosityForce;
        b.Force -= viscosityForce;
    }

    private void ApplyParticleForces()
    {
        for (int i = 0; i < Particles.Count; i++)
        {
            Particle a = Particles[i];
            Point3 cell = GetCell(a.Position);

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        Point3 neighborCell = new Point3(
                            cell.X + dx,
                            cell.Y + dy,
                            cell.Z + dz
                        );

                        if (!grid.TryGetValue(neighborCell, out var indices))
                            continue;

                        foreach (int j in indices)
                        {
                            if (j <= i) //Stops double force calculations, since each calculation applies forces on both particles already
                                continue;

                            ApplyPairForce(i, j);
                        }
                    }
                }
            }
        }
    }

private void ResolveWallCollisions(Particle p)
{
    float bounce = 0.1f;

    if (p.Position.X - p.Radius < MinBounds.X)
    {
        p.Position.X = MinBounds.X + p.Radius;
        p.Velocity.X *= -bounce;
    }
    else if (p.Position.X + p.Radius > MaxBounds.X)
    {
        p.Position.X = MaxBounds.X - p.Radius;
        p.Velocity.X *= -bounce;
    }

    if (p.Position.Y - p.Radius < MinBounds.Y)
    {
        p.Position.Y = MinBounds.Y + p.Radius;
        p.Velocity.Y *= -bounce;
    }
    else if (p.Position.Y + p.Radius > MaxBounds.Y)
    {
        p.Position.Y = MaxBounds.Y - p.Radius;
        p.Velocity.Y *= -bounce;
    }

    if (p.Position.Z - p.Radius < MinBounds.Z)
    {
        p.Position.Z = MinBounds.Z + p.Radius;
        p.Velocity.Z *= -bounce;
    }
    else if (p.Position.Z + p.Radius > MaxBounds.Z)
    {
        p.Position.Z = MaxBounds.Z - p.Radius;
        p.Velocity.Z *= -bounce;
    }
}

private void ResolveObstacleCollisions(Particle p)
{
    foreach (var box in Obstacles)
    {
        Vector3 closest = Vector3.Clamp(p.Position, box.Min, box.Max);
        Vector3 offset = p.Position - closest;
        float distance = offset.Length();

        Vector3 normal;
        float penetration;

        if (distance > 0.0001f)
        {
            if (distance >= p.Radius)
                continue;

            normal = offset / distance;
            penetration = p.Radius - distance;
        }
        else
        {
            float pushLeft = Math.Abs(p.Position.X - box.Min.X) + p.Radius;
            float pushRight = Math.Abs(box.Max.X - p.Position.X) + p.Radius;
            float pushDown = Math.Abs(p.Position.Y - box.Min.Y) + p.Radius;
            float pushUp = Math.Abs(box.Max.Y - p.Position.Y) + p.Radius;
            float pushBack = Math.Abs(p.Position.Z - box.Min.Z) + p.Radius;
            float pushFront = Math.Abs(box.Max.Z - p.Position.Z) + p.Radius;

            penetration = pushLeft;
            normal = Vector3.Left;

            if (pushRight < penetration) { penetration = pushRight; normal = Vector3.Right; }
            if (pushDown < penetration) { penetration = pushDown; normal = Vector3.Down; }
            if (pushUp < penetration) { penetration = pushUp; normal = Vector3.Up; }
            if (pushBack < penetration) { penetration = pushBack; normal = Vector3.Backward; }
            if (pushFront < penetration) { penetration = pushFront; normal = Vector3.Forward; }
        }

        p.Position += normal * penetration;

        float velocityIntoWall = Vector3.Dot(p.Velocity, normal);

        if (velocityIntoWall < 0)
        {
            p.Velocity -= normal * velocityIntoWall * 1.1f;
        }

        // Only damp movement INTO the wall, not sideways flow
        Vector3 normalVelocity = Vector3.Dot(p.Velocity, normal) * normal;
        Vector3 tangentVelocity = p.Velocity - normalVelocity;

        p.Velocity = normalVelocity + tangentVelocity * 0.98f;
            }
        }
}