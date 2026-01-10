using System.Collections;
using System.Threading;

using MyFrameworkProject.Engine.Audio;
using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Input;
using MyFrameworkProject.Engine.Core.Coroutines;
using MyFrameworkProject.Engine.Components.Collisions;

namespace MyFrameworkProject.Engine.Components
{
    /// <summary>
    /// Represents a game object with update lifecycle methods and unique identification.
    /// Extends Entity with BeforeUpdate, Update, and AfterUpdate virtual methods for custom game logic.
    /// Provides static access to input, audio, and coroutine systems for convenient usage in derived classes.
    /// </summary>
    public class GameObject : Entity
    {
        #region Static Fields - Systems

        /// <summary>
        /// Static reference to the input manager for convenient access in all game objects.
        /// Set by the GameLoop during initialization.
        /// </summary>
        protected static InputManager Input { get; private set; }

        /// <summary>
        /// Static reference to the audio manager for convenient access in all game objects.
        /// Set by the GameLoop during initialization.
        /// </summary>
        protected static AudioManager Audio { get; private set; }

        /// <summary>
        /// Static reference to the coroutine manager for convenient access in all game objects.
        /// Set by the GameLoop during initialization.
        /// </summary>
        protected static CoroutineManager Coroutines { get; private set; }

        #endregion

        #region Fields - State

        /// <summary>
        /// Indicates whether this game object is active and should be updated.
        /// </summary>
        private bool _active = true;

        #endregion

        #region Fields - Collision

        /// <summary>
        /// The collision mask associated with this game object.
        /// If null, the game object has no collision detection.
        /// </summary>
        private CollisionMask _collisionMask;

        #endregion

        #region Properties - State

        /// <summary>
        /// Gets whether this game object is active and should be updated.
        /// </summary>
        public bool Active => _active;

        #endregion

        #region Properties - Collision

        /// <summary>
        /// Gets the collision mask associated with this game object.
        /// </summary>
        public CollisionMask CollisionMask => _collisionMask;

        /// <summary>
        /// Gets whether this game object has a collision mask.
        /// </summary>
        public bool HasCollisionMask => _collisionMask != null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameObject"/> class.
        /// Sets the game object as active by default.
        /// </summary>
        public GameObject() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameObject"/> class with the specified sprite.
        /// Sets the game object as active by default.
        /// </summary>
        /// <param name="sprite">The sprite to assign to this game object.</param>
        public GameObject(Sprite sprite) : base(sprite)
        {
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

        /// <summary>
        /// Initializes the static audio reference for all game objects.
        /// Called internally by the GameLoop during initialization.
        /// </summary>
        /// <param name="audioManager">The audio manager instance to use for all game objects.</param>
        internal static void InitializeAudio(AudioManager audioManager)
        {
            Audio = audioManager;
        }

        /// <summary>
        /// Initializes the static coroutine reference for all game objects.
        /// Called internally by the GameLoop during initialization.
        /// </summary>
        /// <param name="coroutineManager">The coroutine manager instance to use for all game objects.</param>
        internal static void InitializeCoroutines(CoroutineManager coroutineManager)
        {
            Coroutines = coroutineManager;
        }

        #endregion

        #region Public Methods - State Management

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

        #region Public Methods - Collision Management

        /// <summary>
        /// Sets the collision mask for this game object.
        /// </summary>
        /// <param name="collisionMask">The collision mask to assign, or null to remove collision detection.</param>
        public void SetCollisionMask(CollisionMask collisionMask)
        {
            _collisionMask = collisionMask;
        }

        /// <summary>
        /// Checks if this game object's collision mask intersects with another game object's collision mask.
        /// Returns false if either object doesn't have a collision mask.
        /// </summary>
        /// <param name="other">The other game object to check collision against.</param>
        /// <returns>True if the collision masks intersect; otherwise, false.</returns>
        public bool CollidesWith(GameObject other)
        {
            if (_collisionMask == null || other == null || other._collisionMask == null)
                return false;

            return _collisionMask.Intersects(other._collisionMask, _x, _y, other._x, other._y);
        }

        /// <summary>
        /// Checks if this game object's collision mask intersects with a specific collision mask at a given position.
        /// Returns false if this object doesn't have a collision mask.
        /// </summary>
        /// <param name="mask">The collision mask to check against.</param>
        /// <param name="maskX">The world X-coordinate of the collision mask.</param>
        /// <param name="maskY">The world Y-coordinate of the collision mask.</param>
        /// <returns>True if the collision masks intersect; otherwise, false.</returns>
        public bool CollidesWithMask(CollisionMask mask, float maskX, float maskY)
        {
            if (_collisionMask == null || mask == null)
                return false;

            return _collisionMask.Intersects(mask, _x, _y, maskX, maskY);
        }

        #endregion

        #region Public Methods - Coroutine Management

        /// <summary>
        /// Starts a new coroutine from an IEnumerator.
        /// The coroutine will be executed over multiple frames.
        /// </summary>
        /// <param name="enumerator">The enumerator that defines the coroutine behavior.</param>
        /// <returns>The created coroutine instance that can be used to control execution.</returns>
        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return Coroutines?.StartCoroutine(enumerator);
        }

        /// <summary>
        /// Stops a running coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to stop.</param>
        public void StopCoroutine(Coroutine coroutine)
        {
            Coroutines?.StopCoroutine(coroutine);
        }

        /// <summary>
        /// Stops all running coroutines started by this game object.
        /// Note: This stops ALL coroutines in the manager, not just those started by this object.
        /// </summary>
        public void StopAllCoroutines()
        {
            Coroutines?.StopAllCoroutines();
        }

        #endregion

        #region Virtual Methods - Update Lifecycle

        /// <summary>
        /// Called before the main Update method.
        /// Override this method to implement custom logic that should execute before the main update.
        /// Useful for input processing or state preparation.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public virtual void BeforeUpdate(float deltaTime)
        {
            // Custom logic before update
        }

        /// <summary>
        /// Main update method called every frame.
        /// Override this method to implement custom game object behavior.
        /// This is where most game logic should be implemented.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public virtual void Update(float deltaTime)
        {
            // Custom update logic
        }

        /// <summary>
        /// Called after the main Update method.
        /// Override this method to implement custom logic that should execute after the main update.
        /// Useful for physics resolution or final state adjustments.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public virtual void AfterUpdate(float deltaTime)
        {
            // Custom logic after update
        }

        #endregion
    }
}