using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FluidSimulation.Simulation;
using FluidSimulation.Rendering;
using System;
namespace FluidSimulation;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FluidWorld world = new();
    private SphereRenderer sphereRenderer;
    private BoxRenderer boxRenderer;
    private Matrix view;
    private Matrix projection;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        world.Obstacles.Add(new BoxObstacle(
            new Vector3(-1f, 0.6f, -1f),
            new Vector3(1f, 8.0f, 1f)
        ));
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 30; y++)
            {
                for (int z = 0; z < 5 ; z++)
                {
                    world.AddParticle(new Vector3(
                        -2.5f + x * 0.25f,
                        1.5f + y * 0.25f,
                        -1.0f + z * 0.25f
                    ));
                }
            }
        }
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        sphereRenderer = new SphereRenderer(GraphicsDevice);
        boxRenderer = new BoxRenderer(GraphicsDevice);

        view = Matrix.CreateLookAt(
            new Vector3(0, 3, 10),
            new Vector3(0, 3, 0),
            Vector3.Up
        );

        projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45f),
            GraphicsDevice.Viewport.AspectRatio,
            0.1f,
            100f
        );
    }


    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // If game drops below 30fps we drop physics calculation to delta time
        dt = MathHelper.Min(dt, 1f / 30f);

        world.Update(dt);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        sphereRenderer.Draw(world.Particles, view, projection);

        foreach (var obstacle in world.Obstacles)
            {
                boxRenderer.DrawBox(
                    obstacle,
                    view,
                    projection,
                    Color.LimeGreen
                );
            }

            boxRenderer.DrawBounds(
                world.MinBounds,
                world.MaxBounds,
                view,
                projection,
                Color.White
            );
        base.Draw(gameTime);
    }
}
