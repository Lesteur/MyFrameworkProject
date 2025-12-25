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

        private Matrix _scalingMatrix;

        public Renderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);

            InitializeCameras();
            CalculateScalingMatrix();
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

        private void CalculateScalingMatrix()
        {
            // Calculate the scaling factors
            float scaleX = (float)EngineConfig.WindowWidth / EngineConfig.VirtualWidth;
            float scaleY = (float)EngineConfig.WindowHeight / EngineConfig.VirtualHeight;

            // Use the smaller scale to maintain aspect ratio
            float scale = MathHelper.Min(scaleX, scaleY); // Letterbox

            _scalingMatrix = Matrix.CreateScale(scale, scale, 1f);
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
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: _worldCamera.GetTransformMatrix() * _scalingMatrix
            );
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
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: _uiCamera.GetTransformMatrix() * _scalingMatrix
            );
        }

        public void EndUI()
        {
            _spriteBatch.End();
        }
    }
}