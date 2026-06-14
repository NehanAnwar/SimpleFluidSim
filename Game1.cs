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
    private KeyboardState previousKeyboard;
    private bool paused;
    private float fps;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1600;
        _graphics.PreferredBackBufferHeight = 900;
        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

    }

    private void CreateSimulation()
    {
        world = new FluidWorld();

        world.Obstacles.Add(new BoxObstacle(
            new Vector3(-1f, 0.5f, -1f),
            new Vector3(1f, 8.0f, 1f)
        ));

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                for (int z = 0; z < 6; z++)
                {
                    world.AddParticle(new Vector3(
                        -2.8f + x * 0.2f,
                        0.8f + y * 0.2f,
                        -0.4f + z * 0.2f
                    ));
                }
            }
        }
    }
    protected override void Initialize()
    {
        CreateSimulation();

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
        KeyboardState keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.R) &&  previousKeyboard.IsKeyUp(Keys.R))
        {
            CreateSimulation();
        }

        if (keyboard.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        if (keyboard.IsKeyDown(Keys.Space) && previousKeyboard.IsKeyUp(Keys.Space))
        {
            paused = !paused;
        }
        previousKeyboard = keyboard;


        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        fps = (int)(1/dt); //For stat visualisation, not physics related

        // If game drops below 30fps we drop physics calculation to delta time

        dt = MathHelper.Min(dt, 1f / 30f);

        if (!paused)
        {
            world.Update(dt);
        }
        base.Update(gameTime);

        Window.Title =
            $"Fluid Simulation | Particles: {world.Particles.Count} | " +
            $"Repulsion: {world.RepulsionStrength} | " +
            $"Viscosity: {world.Viscosity} | " +
            $"FPS: {fps}"; 
            //This is a workaround for stat rendering, writing text in monogame needs me to wrestle with some esoteric tools
            //FPS doesn't really mean anything since monogame has built in VSYNC and keeps the sim at 60fps even if phys calculations are at delta time
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
