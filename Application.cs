using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine3D.Rendering;
using Microsoft.Xna.Framework.Input;

namespace Engine3D
{
    public class Application : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch batch;
        private ScreenBuffer drawBuffer;
        private DepthBuffer depthBuffer;
        private Engine engine;

        public Application()
        {
            graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 750;
            graphics.PreferredBackBufferHeight = 750;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            drawBuffer = new ScreenBuffer(250, 250, GraphicsDevice);
            depthBuffer = new DepthBuffer(drawBuffer.Width, drawBuffer.Height);

            TextureLoader.Init(GraphicsDevice);

            engine = new Engine();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Input.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            drawBuffer.Clear();
            depthBuffer.Clear();
            engine.Draw(drawBuffer, depthBuffer, (float)gameTime.ElapsedGameTime.TotalSeconds);

            batch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp);
            drawBuffer.Draw(batch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            batch.End();

            base.Draw(gameTime);
        }
    }
}
