using Microsoft.Xna.Framework;

namespace MyFrameworkProject.Engine.Core
{
    /// <summary>
    /// Provides global access to time-related information for the game engine.
    /// Tracks delta time between frames and total elapsed time since game start.
    /// This is a static utility class that is updated internally by the engine.
    /// </summary>
    public static class Time
    {
        #region Properties - Time

        /// <summary>
        /// Gets the time in seconds that elapsed since the last frame.
        /// Use this value to make frame-rate independent calculations and animations.
        /// </summary>
        public static float DeltaTime { get; private set; }

        /// <summary>
        /// Gets the total time in seconds that has elapsed since the game started.
        /// This value continuously increases and can be used for timing events or animations.
        /// </summary>
        public static float TotalTime { get; private set; }

        #endregion

        #region Internal Methods - Lifecycle

        /// <summary>
        /// Initializes the time system by resetting all time values to zero.
        /// This method is called internally by the engine during initialization.
        /// </summary>
        internal static void Initialize()
        {
            DeltaTime = 0f;
            TotalTime = 0f;

            Logger.Info("Time system initialized");
        }

        /// <summary>
        /// Updates the time values based on the current game time.
        /// This method is called internally by the engine once per frame.
        /// </summary>
        /// <param name="gameTime">The MonoGame GameTime object containing elapsed and total time information.</param>
        internal static void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TotalTime = (float)gameTime.TotalGameTime.TotalSeconds;
        }

        #endregion
    }
}