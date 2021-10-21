using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
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
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;
            graphics.ApplyChanges();

            engine = new Engine();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            buffer = new ScreenBuffer(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //buffer.Clear();
            engine.Draw(buffer, (float)gameTime.ElapsedGameTime.TotalSeconds);

            batch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
            buffer.Draw(batch);
            batch.End();

            base.Draw(gameTime);
        }
    }
}
