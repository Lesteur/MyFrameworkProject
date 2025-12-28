using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Core;
using MyFrameworkProject.Engine.Graphics;

namespace MyFrameworkProject.Assets
{
    /// <summary>
    /// Test room demonstrating resource loading and entity creation.
    /// </summary>
    public class RoomTest : GameRoom
    {
        public RoomTest() : base(400, 400)
        {
        }

        protected override void Load()
        {
            Logger.Info("RoomTest: Loading resources...");

            // Load and create player sprite
            Texture2D nativeTexture = Content.Load<Texture2D>("spr_jonathan");
            var texture = new Engine.Graphics.Texture(nativeTexture);
            var sprite = new Sprite(texture, 0, 0, 14);

            // Create player object
            var player = new ObjectTest(sprite);
            player.SetPosition(10, 10);
            player.SetScale(1.0f, 1.0f);
            player.SetMoveSpeed(150f);
            player.EnableAnimation(0.05f, true);

            // Set camera to follow player
            CameraTarget = player;

            // Add to game loop
            GameLoop.AddGameObject(player);

            // Load and create tilemap
            Texture2D tilesetTexture = Content.Load<Texture2D>("Tileset");
            var tileset = new Engine.Graphics.Texture(tilesetTexture);
            var tileSprite = new Tileset(tileset, 16, 16);

            var tilemap = new Tilemap(tileSprite, 50, 50);
            tilemap.Fill(2);
            GameLoop.AddTilemap(tilemap);

            // Load sound effect
            SoundEffect soundEffect = Content.Load<SoundEffect>("sfx_chest");
            Application.Instance.Audio.LoadSound("chest", soundEffect);

            Logger.Info("RoomTest: All resources loaded successfully");
        }

        protected override void Unload()
        {
            Logger.Info("RoomTest: Custom cleanup logic");
            // Perform any custom cleanup here
        }
    }
}
