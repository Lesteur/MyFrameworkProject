using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MyFrameworkProject.Engine.Core
{
    public class GameLoop
    {
        public GameLoop()
        {
            Logger.Info("GameLoop created");
        }

        public void Update()
        {
            // Global update logic
            // Later: Scene management, input handling, etc.
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.CornflowerBlue);

            // Later: Scene rendering, UI rendering, etc.
        }
    }
}