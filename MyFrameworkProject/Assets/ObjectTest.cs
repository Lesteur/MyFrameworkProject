using MyFrameworkProject.Engine.Components;
using MyFrameworkProject.Engine.Graphics;
using MyFrameworkProject.Engine.Input;
using MyFrameworkProject.Engine.Core;

namespace MyFrameworkProject.Assets
{
    /// <summary>
    /// Test implementation of GameObject for demonstration and prototyping.
    /// Provides keyboard-controlled movement and basic interaction capabilities.
    /// This class serves as an example of how to extend GameObject with custom behavior.
    /// </summary>
    public class ObjectTest : GameObject
    {
        #region Fields - Movement

        /// <summary>
        /// The horizontal movement speed in pixels per second.
        /// </summary>
        private float _moveSpeed = 100f;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectTest"/> class.
        /// </summary>
        public ObjectTest() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectTest"/> class with the specified sprite.
        /// </summary>
        /// <param name="sprite">The sprite to assign to this test object.</param>
        public ObjectTest(Sprite sprite) : base(sprite)
        {
        }

        #endregion

        #region Public Methods - Configuration

        /// <summary>
        /// Sets the movement speed for this test object.
        /// </summary>
        /// <param name="speed">The movement speed in pixels per second.</param>
        public void SetMoveSpeed(float speed)
        {
            _moveSpeed = speed;
        }

        #endregion

        #region Override Methods - Update Lifecycle

        /// <summary>
        /// Called before the main Update method.
        /// Can be used for pre-update initialization or state preparation.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public override void BeforeUpdate(float deltaTime)
        {
            // Example: Reset temporary flags or prepare state
        }

        /// <summary>
        /// Main update method that handles input and movement logic.
        /// Demonstrates how to use the Input system to control a GameObject.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public override void Update(float deltaTime)
        {
            // Handle horizontal movement
            if (Input.IsDown(InputAction.Left))
            {
                _x -= (int)(_moveSpeed * deltaTime);
            }

            if (Input.IsDown(InputAction.Right))
            {
                _x += (int)(_moveSpeed * deltaTime);
            }

            // Handle vertical movement
            if (Input.IsDown(InputAction.Up))
            {
                _y -= (int)(_moveSpeed * deltaTime);
            }

            if (Input.IsDown(InputAction.Down))
            {
                _y += (int)(_moveSpeed * deltaTime);
            }

            // Example: Respond to action presses
            if (Input.IsPressed(InputAction.Confirm))
            {
                // Trigger an action on key press (e.g., jump, interact)
                OnConfirmPressed();

                Audio.PlaySound("chest");
            }

            if (Input.IsPressed(InputAction.Cancel))
            {
                // Handle cancel action (e.g., close menu, cancel action)
                OnCancelPressed();
            }
        }

        /// <summary>
        /// Called after the main Update method.
        /// Can be used for post-update cleanup or finalizing state changes.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public override void AfterUpdate(float deltaTime)
        {
            // Example: Apply physics constraints, clamp positions, etc.
            ClampPosition();
        }

        #endregion

        #region Private Methods - Actions

        /// <summary>
        /// Called when the Confirm action is pressed.
        /// Override or extend this method for custom confirm behavior.
        /// </summary>
        protected virtual void OnConfirmPressed()
        {
            // Example: Log or trigger specific behavior
            Logger.Info($"ObjectTest {Id}: Confirm pressed at position ({X}, {Y})");
        }

        /// <summary>
        /// Called when the Cancel action is pressed.
        /// Override or extend this method for custom cancel behavior.
        /// </summary>
        protected virtual void OnCancelPressed()
        {
            // Example: Deactivate or perform cancel action
            Logger.Info($"ObjectTest {Id}: Cancel pressed");
        }

        /// <summary>
        /// Clamps the object's position to screen boundaries.
        /// This is a demonstration of post-update position constraints.
        /// </summary>
        private void ClampPosition()
        {
            // Example boundary clamping (adjust based on your game's needs)
            // Uncomment and modify as needed:
            // if (_x < 0) _x = 0;
            // if (_y < 0) _y = 0;
            // if (_x > maxX) _x = maxX;
            // if (_y > maxY) _y = maxY;
        }

        #endregion
    }
}