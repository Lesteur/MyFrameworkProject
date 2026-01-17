using System;
using System.Collections;

using Microsoft.Xna.Framework.Content;

using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Core.Coroutines;

namespace MyFrameworkProject.Engine.Core
{
    /// <summary>
    /// Manages scene transitions with support for asynchronous loading and custom transition effects.
    /// Provides callbacks for scene lifecycle events and ensures proper resource cleanup.
    /// </summary>
    public sealed class SceneManager
    {
        #region Fields - State

        /// <summary>
        /// The currently active game room.
        /// </summary>
        private GameRoom _currentRoom;

        /// <summary>
        /// Indicates whether a scene transition is currently in progress.
        /// </summary>
        private bool _isTransitioning = false;

        #endregion

        #region Fields - References

        /// <summary>
        /// Reference to the game loop for managing entities.
        /// </summary>
        private readonly GameLoop _gameLoop;

        /// <summary>
        /// Reference to the content manager for loading resources.
        /// </summary>
        private readonly ContentManager _content;

        /// <summary>
        /// Reference to the coroutine manager for async operations.
        /// </summary>
        private readonly CoroutineManager _coroutines;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the currently active game room.
        /// </summary>
        public GameRoom CurrentRoom => _currentRoom;

        /// <summary>
        /// Gets whether a scene transition is currently in progress.
        /// </summary>
        public bool IsTransitioning => _isTransitioning;

        #endregion

        #region Events

        /// <summary>
        /// Event raised before a room is unloaded.
        /// </summary>
        public event Action<GameRoom> OnBeforeRoomUnload;

        /// <summary>
        /// Event raised after a room is unloaded.
        /// </summary>
        public event Action OnAfterRoomUnload;

        /// <summary>
        /// Event raised before a room is loaded.
        /// </summary>
        public event Action<GameRoom> OnBeforeRoomLoad;

        /// <summary>
        /// Event raised after a room is loaded.
        /// </summary>
        public event Action<GameRoom> OnAfterRoomLoad;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneManager"/> class.
        /// </summary>
        /// <param name="gameLoop">The game loop to use for entity management.</param>
        /// <param name="content">The content manager for loading resources.</param>
        /// <param name="coroutines">The coroutine manager for async operations.</param>
        public SceneManager(GameLoop gameLoop, ContentManager content, CoroutineManager coroutines)
        {
            _gameLoop = gameLoop ?? throw new ArgumentNullException(nameof(gameLoop));
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _coroutines = coroutines ?? throw new ArgumentNullException(nameof(coroutines));

            Logger.Info("SceneManager initialized");
        }

        #endregion

        #region Public Methods - Scene Loading

        /// <summary>
        /// Loads a new room immediately, unloading the current room if one exists.
        /// This is a synchronous operation that completes within a single frame.
        /// </summary>
        /// <param name="room">The room to load.</param>
        public void LoadRoom(GameRoom room)
        {
            if (room == null)
            {
                Logger.Error("Cannot load null room");
                return;
            }

            if (_isTransitioning)
            {
                Logger.Warning("Cannot load room while a transition is in progress");
                return;
            }

            UnloadCurrentRoom();
            LoadNewRoom(room);
        }

        /// <summary>
        /// Loads a new room asynchronously with a custom transition effect.
        /// The transition is executed as a coroutine, allowing for smooth animations.
        /// </summary>
        /// <param name="room">The room to load.</param>
        /// <param name="transitionEffect">Optional transition effect coroutine (e.g., fade out/in).</param>
        public void LoadRoomAsync(GameRoom room, Func<IEnumerator> transitionEffect = null)
        {
            if (room == null)
            {
                Logger.Error("Cannot load null room");
                return;
            }

            if (_isTransitioning)
            {
                Logger.Warning("Cannot load room while a transition is in progress");
                return;
            }

            _coroutines.StartCoroutine(TransitionToRoom(room, transitionEffect));
        }

        /// <summary>
        /// Unloads the current room without loading a new one.
        /// Useful for returning to a menu state or cleaning up before application exit.
        /// </summary>
        public void UnloadCurrentRoom()
        {
            if (_currentRoom == null)
                return;

            Logger.Info($"Unloading room: {_currentRoom.GetType().Name}");

            OnBeforeRoomUnload?.Invoke(_currentRoom);
            _currentRoom.Cleanup();
            _currentRoom = null;
            OnAfterRoomUnload?.Invoke();
        }

        #endregion

        #region Private Methods - Scene Loading

        /// <summary>
        /// Loads a new room and activates it.
        /// </summary>
        /// <param name="room">The room to load.</param>
        private void LoadNewRoom(GameRoom room)
        {
            Logger.Info($"Loading room: {room.GetType().Name}");

            OnBeforeRoomLoad?.Invoke(room);
            _currentRoom = room;
            _currentRoom.Initialize(_gameLoop, _content);
            OnAfterRoomLoad?.Invoke(room);

            Logger.Info($"Room '{room.GetType().Name}' is now active");
        }

        /// <summary>
        /// Coroutine that handles asynchronous room transitions with optional effects.
        /// </summary>
        /// <param name="room">The room to transition to.</param>
        /// <param name="transitionEffect">Optional transition effect coroutine.</param>
        /// <returns>Coroutine enumerator.</returns>
        private IEnumerator TransitionToRoom(GameRoom room, Func<IEnumerator> transitionEffect)
        {
            _isTransitioning = true;

            // Execute transition out effect
            if (transitionEffect != null)
            {
                yield return _coroutines.StartCoroutine(transitionEffect());
            }

            // Unload current room
            UnloadCurrentRoom();

            // Wait one frame to ensure cleanup is complete
            yield return null;

            // Load new room
            LoadNewRoom(room);

            // Wait one frame for initialization
            yield return null;

            // Execute transition in effect
            if (transitionEffect != null)
            {
                yield return _coroutines.StartCoroutine(transitionEffect());
            }

            _isTransitioning = false;
        }

        #endregion
    }
}