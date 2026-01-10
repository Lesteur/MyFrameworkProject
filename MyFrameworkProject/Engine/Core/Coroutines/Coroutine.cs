using System;
using System.Collections;

namespace MyFrameworkProject.Engine.Core.Coroutines
{
    /// <summary>
    /// Represents a coroutine that can be executed over multiple frames.
    /// Wraps an IEnumerator to provide pause/resume functionality similar to Unity.
    /// </summary>
    public sealed class Coroutine
    {
        #region Fields - Execution

        /// <summary>
        /// The underlying enumerator that represents the coroutine execution.
        /// </summary>
        private readonly IEnumerator _enumerator;

        #endregion

        #region Fields - State

        /// <summary>
        /// Indicates whether this coroutine has completed execution.
        /// </summary>
        private bool _isFinished;

        /// <summary>
        /// Indicates whether this coroutine is currently paused.
        /// </summary>
        private bool _isPaused;

        #endregion

        #region Properties - State

        /// <summary>
        /// Gets whether this coroutine has finished executing.
        /// </summary>
        public bool IsFinished => _isFinished;

        /// <summary>
        /// Gets whether this coroutine is currently paused.
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Gets whether this coroutine is currently running (not paused and not finished).
        /// </summary>
        public bool IsRunning => !_isFinished && !_isPaused;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Coroutine"/> class.
        /// </summary>
        /// <param name="enumerator">The enumerator that represents the coroutine logic.</param>
        internal Coroutine(IEnumerator enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            _isFinished = false;
            _isPaused = false;
        }

        #endregion

        #region Public Methods - Control

        /// <summary>
        /// Pauses the execution of this coroutine.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Resumes the execution of this coroutine.
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Stops the execution of this coroutine immediately.
        /// </summary>
        public void Stop()
        {
            _isFinished = true;
        }

        #endregion

        #region Internal Methods - Update

        /// <summary>
        /// Updates the coroutine by one step.
        /// Called by the CoroutineManager each frame.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        /// <returns>True if the coroutine should continue, false if it has finished.</returns>
        internal bool Update(float deltaTime)
        {
            if (_isFinished || _isPaused)
                return !_isFinished;

            // Check if current yield instruction needs to wait
            if (_enumerator.Current is IYieldInstruction yieldInstruction)
            {
                if (!yieldInstruction.IsDone(deltaTime))
                    return true;
            }

            // Move to next step
            if (!_enumerator.MoveNext())
            {
                _isFinished = true;
                return false;
            }

            return true;
        }

        #endregion
    }

    #region Yield Instructions

    /// <summary>
    /// Interface for custom yield instructions that can control coroutine timing.
    /// </summary>
    public interface IYieldInstruction
    {
        /// <summary>
        /// Determines whether the yield instruction has completed.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        /// <returns>True if the instruction is done and the coroutine should continue.</returns>
        bool IsDone(float deltaTime);
    }

    /// <summary>
    /// Yield instruction that waits for a specified number of seconds.
    /// </summary>
    public sealed class WaitForSeconds : IYieldInstruction
    {
        #region Fields

        /// <summary>
        /// The remaining time to wait in seconds.
        /// </summary>
        private float _remainingTime;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForSeconds"/> class.
        /// </summary>
        /// <param name="seconds">The number of seconds to wait.</param>
        public WaitForSeconds(float seconds)
        {
            _remainingTime = Math.Max(0f, seconds);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the wait time has elapsed.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        /// <returns>True if the wait time has elapsed, false otherwise.</returns>
        public bool IsDone(float deltaTime)
        {
            _remainingTime -= deltaTime;
            return _remainingTime <= 0;
        }

        #endregion
    }

    /// <summary>
    /// Yield instruction that waits for the next frame.
    /// </summary>
    public sealed class WaitForNextFrame : IYieldInstruction
    {
        #region Fields

        /// <summary>
        /// Indicates whether one frame has passed.
        /// </summary>
        private bool _hasWaited;

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if one frame has passed.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        /// <returns>True if one frame has passed, false otherwise.</returns>
        public bool IsDone(float deltaTime)
        {
            if (_hasWaited)
                return true;

            _hasWaited = true;
            return false;
        }

        #endregion
    }

    /// <summary>
    /// Yield instruction that waits until a condition becomes true.
    /// </summary>
    public sealed class WaitUntil : IYieldInstruction
    {
        #region Fields

        /// <summary>
        /// The condition to wait for.
        /// </summary>
        private readonly Func<bool> _predicate;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil"/> class.
        /// </summary>
        /// <param name="predicate">The condition to wait for.</param>
        public WaitUntil(Func<bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the condition is true.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        /// <returns>True if the condition is true, false otherwise.</returns>
        public bool IsDone(float deltaTime)
        {
            return _predicate();
        }

        #endregion
    }

    /// <summary>
    /// Yield instruction that waits while a condition is true.
    /// </summary>
    public sealed class WaitWhile : IYieldInstruction
    {
        #region Fields

        /// <summary>
        /// The condition to wait while true.
        /// </summary>
        private readonly Func<bool> _predicate;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitWhile"/> class.
        /// </summary>
        /// <param name="predicate">The condition to wait while true.</param>
        public WaitWhile(Func<bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the condition is false.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        /// <returns>True if the condition is false, false otherwise.</returns>
        public bool IsDone(float deltaTime)
        {
            return !_predicate();
        }

        #endregion
    }

    #endregion
}