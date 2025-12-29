using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Core;
using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Serialization;

namespace MyFrameworkProject.Assets
{
    /// <summary>
    /// Test room demonstrating Tiled map loading from the content pipeline.
    /// </summary>
    public class RoomTest : GameRoom
    {
        public RoomTest() : base(640, 640)
        {
        }

        protected override void Load()
        {
            Logger.Info("RoomTest: Loading content...");

            // Load the Tiled map from the content pipeline with a custom object factory
            LoadTiledMap("Levels/Level", CreateGameObjectFromTiled);

            // Load sound effect
            SoundEffect soundEffect = Content.Load<SoundEffect>("Audio/sfx_chest");
            Application.Instance.Audio.LoadSound("chest", soundEffect);

            Logger.Info("RoomTest: All resources loaded successfully");
        }

        /// <summary>
        /// Factory method to create GameObjects from Tiled objects.
        /// </summary>
        /// <param name="tiledObject">The Tiled object definition.</param>
        /// <returns>A GameObject instance or null.</returns>
        private GameObject CreateGameObjectFromTiled(TiledObject tiledObject)
        {
            switch (tiledObject.Type)
            {
                case "Player":
                    return CreatePlayer(tiledObject);

                // Add more cases for different object types as needed
                // case "Enemy":
                //     return CreateEnemy(tiledObject);
                // case "Item":
                //     return CreateItem(tiledObject);

                default:
                    Logger.Warning($"Unknown object type: {tiledObject.Type}");
                    return null;
            }
        }

        /// <summary>
        /// Creates a player GameObject from a Tiled object definition.
        /// </summary>
        /// <param name="tiledObject">The Tiled object definition.</param>
        /// <returns>A player GameObject.</returns>
        private GameObject CreatePlayer(TiledObject tiledObject)
        {
            // Load player sprite texture
            Texture2D nativeTexture = Content.Load<Texture2D>("Textures/spr_jonathan");
            var texture = new Engine.Graphics.Texture(nativeTexture);
            var sprite = new Sprite(texture, 0, 0, 14);

            // Create player game object
            var player = new ObjectTest(sprite);
            player.SetPosition((int)tiledObject.X, (int)tiledObject.Y);
            player.SetScale(1.0f, 1.0f);
            player.SetMoveSpeed(150f);
            player.EnableAnimation(0.05f, true);

            // Set camera to follow the player
            CameraTarget = player;

            Logger.Info($"Player created at ({tiledObject.X}, {tiledObject.Y})");
            return player;
        }

        protected override void Unload()
        {
            Logger.Info("RoomTest: Performing custom cleanup");
        }
    }
}