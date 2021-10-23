using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine3D.Rendering;

namespace Engine3D
{
    public class Application : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch batch;
        private ScreenBuffer buffer;
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
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            buffer = new ScreenBuffer(500, 500, GraphicsDevice);

            TextureLoader.Init(GraphicsDevice);

            engine = new Engine();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            buffer.Clear();
            engine.Draw(buffer, (float)gameTime.ElapsedGameTime.TotalSeconds);

            batch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp);
            buffer.Draw(batch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            batch.End();

            base.Draw(gameTime);
        }
    }
}
