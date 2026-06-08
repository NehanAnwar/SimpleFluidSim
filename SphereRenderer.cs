using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FluidSimulation.Simulation;
using System.Collections.Generic;
namespace FluidSimulation.Rendering;

public class SphereRenderer
{
    private GraphicsDevice graphicsDevice;
    private BasicEffect effect;

    private VertexPositionColor[] vertices;
    private short[] indices;

    public SphereRenderer(GraphicsDevice graphicsDevice)
    {
        this.graphicsDevice = graphicsDevice;

        effect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            LightingEnabled = false
        };

        graphicsDevice.RasterizerState = RasterizerState.CullNone;

        BuildSphere(6, 6); //Reduce for fps gains, 2x2 minimum
    }

private void DrawSphere(Vector3 position, float radius, Color color)
{
    for (int i = 0; i < vertices.Length; i++)
    {
        vertices[i].Color = color;
    }

    effect.World =
        Matrix.CreateScale(radius) *
        Matrix.CreateTranslation(position);

    foreach (var pass in effect.CurrentTechnique.Passes)
    {
        pass.Apply();

        graphicsDevice.DrawUserIndexedPrimitives(
            PrimitiveType.TriangleList,
            vertices,
            0,
            vertices.Length,
            indices,
            0,
            indices.Length / 3
        );
    }
}

public void Draw(List<Particle> particles, Matrix view, Matrix projection)
{
    effect.View = view;
    effect.Projection = projection;

    graphicsDevice.DepthStencilState = DepthStencilState.Default;
    graphicsDevice.RasterizerState = RasterizerState.CullNone;

    foreach (var p in particles)
    {
        float t = MathHelper.Clamp(p.Density / 18f, 0f, 1f); //tweak denominator of value for finer or rougher color calculations, increasing makes it easier to differentiate finer densities but "cools down" the colors.  
        Color densityColor = Color.Lerp(Color.Blue,Color.Red, t);

        DrawSphere(p.Position, p.Radius, densityColor);
    }
}


    private void BuildSphere(int slices, int stacks)
    {
        var verts = new List<VertexPositionColor>();
        var inds = new List<short>();

        for (int stack = 0; stack <= stacks; stack++)
        {
            float phi = MathHelper.Pi * stack / stacks;
            float y = (float)System.Math.Cos(phi);
            float radius = (float)System.Math.Sin(phi);

            for (int slice = 0; slice <= slices; slice++)
            {
                float theta = MathHelper.TwoPi * slice / slices;

                float x = radius * (float)System.Math.Cos(theta);
                float z = radius * (float)System.Math.Sin(theta);

                verts.Add(new VertexPositionColor(
                    new Vector3(x, y, z),
                    Color.SaddleBrown //Color here is solely placeholder and is overwritten by density coloring in Draw()
                ));
            }
        }

        for (int stack = 0; stack < stacks; stack++)
        {
            for (int slice = 0; slice < slices; slice++)
            {
                short first = (short)(stack * (slices + 1) + slice);
                short second = (short)(first + slices + 1);

                inds.Add(first);
                inds.Add(second);
                inds.Add((short)(first + 1));

                inds.Add(second);
                inds.Add((short)(second + 1));
                inds.Add((short)(first + 1));
            }
        }

        vertices = verts.ToArray();
        indices = inds.ToArray();
    }
}