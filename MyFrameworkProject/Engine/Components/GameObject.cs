using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Input;
using System.Threading;

namespace MyFrameworkProject.Engine.Components
{
    /// <summary>
    /// Represents a game object with update lifecycle methods and unique identification.
    /// Extends Entity with BeforeUpdate, Update, and AfterUpdate virtual methods for custom game logic.
    /// Provides static access to the input manager for convenient input queries in derived classes.
    /// </summary>
    public class GameObject : Entity
    {
        #region Static Fields

        /// <summary>
        /// Global counter used to generate unique identifiers for each game object instance.
        /// Uses Interlocked for thread-safe increment operations.
        /// </summary>
        private static int _counter = 0;

        /// <summary>
        /// Static reference to the input manager for convenient access in all game objects.
        /// Set by the GameLoop during initialization.
        /// </summary>
        protected static InputManager Input { get; private set; }

        #endregion

        #region Fields - Identity

        /// <summary>
        /// Unique identifier for this game object instance.
        /// </summary>
        private readonly uint _gameId;

        /// <summary>
        /// Gets the unique identifier for this game object instance.
        /// </summary>
        public uint Id => _gameId;

        #endregion

        #region Fields - State

        /// <summary>
        /// Indicates whether this game object is active and should be updated.
        /// </summary>
        private bool _active = true;

        /// <summary>
        /// Gets whether this game object is active and should be updated.
        /// </summary>
        public bool Active => _active;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameObject"/> class.
        /// Generates a unique ID and sets the game object as active by default.
        /// </summary>
        public GameObject() : base()
        {
            _gameId = (uint)Interlocked.Increment(ref _counter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameObject"/> class with the specified sprite.
        /// Generates a unique ID and sets the game object as active by default.
        /// </summary>
        /// <param name="sprite">The sprite to assign to this game object.</param>
        public GameObject(Sprite sprite) : base(sprite)
        {
            _gameId = (uint)Interlocked.Increment(ref _counter);
        }

        #endregion

        #region Internal Methods - Initialization

        /// <summary>
        /// Initializes the static input reference for all game objects.
        /// Called internally by the GameLoop during initialization.
        /// </summary>
        /// <param name="inputManager">The input manager instance to use for all game objects.</param>
        internal static void InitializeInput(InputManager inputManager)
        {
            Input = inputManager;
        }

        #endregion

        #region Public Methods - State

        /// <summary>
        /// Sets the active state of this game object.
        /// Inactive game objects are skipped during the update phase.
        /// </summary>
        /// <param name="active">True to activate the game object, false to deactivate it.</param>
        public void SetActive(bool active)
        {
            _active = active;
        }

        #endregion

        #region Virtual Methods - Update Lifecycle

        /// <summary>
        /// Called before the main Update method.
        /// Override this method to implement custom logic that should execute before the main update.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public virtual void BeforeUpdate(float deltaTime)
        {
            // Custom logic before update
        }

        /// <summary>
        /// Main update method called every frame.
        /// Override this method to implement custom game object behavior.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public virtual void Update(float deltaTime)
        {
            // Custom update logic
        }

        /// <summary>
        /// Called after the main Update method.
        /// Override this method to implement custom logic that should execute after the main update.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public virtual void AfterUpdate(float deltaTime)
        {
            // Custom logic after update
        }

        #endregion
    }
}