using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FluidSimulation.Simulation;

namespace FluidSimulation.Rendering;

public class BoxRenderer
{
    private readonly GraphicsDevice graphicsDevice;
    private readonly BasicEffect effect;

    public BoxRenderer(GraphicsDevice graphicsDevice)
    {
        this.graphicsDevice = graphicsDevice;

        effect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            LightingEnabled = false
        };
    }

    public void DrawBox(BoxObstacle box, Matrix view, Matrix projection, Color color)
    {
        effect.World = Matrix.Identity;
        effect.View = view;
        effect.Projection = projection;

        VertexPositionColor[] vertices = CreateBoxLines(box, color);

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            graphicsDevice.DrawUserPrimitives(
                PrimitiveType.LineList,
                vertices,
                0,
                vertices.Length / 2
            );
        }
    }

    public void DrawBounds(Vector3 min, Vector3 max, Matrix view, Matrix projection, Color color)
    {
        DrawBox(new BoxObstacle(min, max), view, projection, color);
    }

    private VertexPositionColor[] CreateBoxLines(BoxObstacle box, Color color)
    {
        Vector3 min = box.Min;
        Vector3 max = box.Max;

        Vector3[] corners =
        {
            new(min.X, min.Y, min.Z),
            new(max.X, min.Y, min.Z),
            new(max.X, max.Y, min.Z),
            new(min.X, max.Y, min.Z),

            new(min.X, min.Y, max.Z),
            new(max.X, min.Y, max.Z),
            new(max.X, max.Y, max.Z),
            new(min.X, max.Y, max.Z),
        };

        int[] edges =
        {
            0,1, 1,2, 2,3, 3,0,
            4,5, 5,6, 6,7, 7,4,
            0,4, 1,5, 2,6, 3,7
        };

        VertexPositionColor[] vertices =
            new VertexPositionColor[edges.Length];

        for (int i = 0; i < edges.Length; i++)
        {
            vertices[i] = new VertexPositionColor(
                corners[edges[i]],
                color
            );
        }

        return vertices;
    }
}