using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyFrameworkProject.Engine.Graphics
{
    /// <summary>
    /// Central rendering system responsible for drawing sprites.
    /// </summary>
    public sealed class Renderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;

        private Camera _worldCamera;
        private Camera _uiCamera;

        public Renderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            InitializeCameras();
        }

        private void InitializeCameras()
        {
            _worldCamera = new Camera(
                0,
                0,
                EngineConfig.VirtualWidth,
                EngineConfig.VirtualHeight
            );

            _uiCamera = new Camera(
                0,
                0,
                EngineConfig.VirtualWidth,
                EngineConfig.VirtualHeight
            );
        }

        public Camera WorldCamera => _worldCamera;
        public Camera UICamera => _uiCamera;

        public void BeginFrame()
        {
            _graphicsDevice.Clear(Color.SkyBlue);
        }

        public void BeginWorld()
        {
            _spriteBatch.Begin(
                transformMatrix: _worldCamera.GetViewProjection(),
                samplerState: SamplerState.PointClamp,
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend
            );

            /*
            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp
            );
            */
        }

        public void DrawEntity(Entity entity)
        {
            var sprite = entity.GetSprite();
            if (sprite == null)
                return;

            _spriteBatch.Draw(
                sprite.GetTexture().GetNativeTexture(),
                new Vector2(entity.GetX(), entity.GetY()),
                sprite.GetSourceRectangle(entity.GetFrameNumber()),
                Color.White,
                MathHelper.ToRadians(entity.GetRotation()),
                sprite.GetOrigin(),
                new Vector2(entity.GetScaleX(), entity.GetScaleY()),
                SpriteEffects.None,
                0f
            );
        }

        public void EndWorld()
        {
            _spriteBatch.End();
        }

        public void BeginUI()
        {
            _spriteBatch.Begin(
                transformMatrix: _uiCamera.GetViewProjection(),
                samplerState: SamplerState.PointClamp,
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend
            );
        }

        public void EndUI()
        {
            _spriteBatch.End();
        }
    }
}