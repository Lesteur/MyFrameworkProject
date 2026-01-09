using System.Collections;
using System.Collections.Generic;

namespace MyFrameworkProject.Engine.Core.Coroutines
{
    /// <summary>
    /// Manages the execution of all active coroutines.
    /// Provides methods to start, stop, and update coroutines each frame.
    /// </summary>
    public class CoroutineManager
    {
        #region Fields

        /// <summary>
        /// The list of all currently active coroutines.
        /// </summary>
        private readonly List<Coroutine> _coroutines = [];

        /// <summary>
        /// Pending coroutines to add, processed at the end of the frame to avoid collection modification during iteration.
        /// </summary>
        private readonly List<Coroutine> _coroutinesToAdd = [];

        /// <summary>
        /// Pending coroutines to remove, processed at the end of the frame to avoid collection modification during iteration.
        /// </summary>
        private readonly HashSet<Coroutine> _coroutinesToRemove = [];

        #endregion

        #region Public Methods - Coroutine Management

        /// <summary>
        /// Starts a new coroutine from an IEnumerator.
        /// </summary>
        /// <param name="enumerator">The enumerator that defines the coroutine behavior.</param>
        /// <returns>The created coroutine instance that can be used to control execution.</returns>
        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            if (enumerator == null)
            {
                Logger.Error("Cannot start coroutine with null enumerator");
                return null;
            }

            var coroutine = new Coroutine(enumerator);
            _coroutinesToAdd.Add(coroutine);
            return coroutine;
        }

        /// <summary>
        /// Stops a running coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to stop.</param>
        public void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine == null)
                return;

            coroutine.Stop();
            _coroutinesToRemove.Add(coroutine);
        }

        /// <summary>
        /// Stops all running coroutines immediately.
        /// </summary>
        public void StopAllCoroutines()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.Stop();
            }

            _coroutines.Clear();
            _coroutinesToAdd.Clear();
            _coroutinesToRemove.Clear();
        }

        #endregion

        #region Public Methods - Update

        /// <summary>
        /// Updates all active coroutines.
        /// Called once per frame by the game loop.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        public void Update(float deltaTime)
        {
            // Update all active coroutines
            int coroutineCount = _coroutines.Count;
            for (int i = 0; i < coroutineCount; i++)
            {
                var coroutine = _coroutines[i];
                
                // Update returns false when coroutine is finished
                if (!coroutine.Update(deltaTime))
                {
                    _coroutinesToRemove.Add(coroutine);
                }
            }

            // Process pending operations
            ProcessPendingOperations();
        }

        #endregion

        #region Private Methods - Deferred Operations

        /// <summary>
        /// Processes all pending add and remove operations for coroutines.
        /// Called at the end of each frame to avoid modifying collections during iteration.
        /// </summary>
        private void ProcessPendingOperations()
        {
            if (_coroutinesToRemove.Count > 0)
            {
                foreach (var coroutine in _coroutinesToRemove)
                    _coroutines.Remove(coroutine);
                
                _coroutinesToRemove.Clear();
            }

            if (_coroutinesToAdd.Count > 0)
            {
                _coroutines.AddRange(_coroutinesToAdd);
                _coroutinesToAdd.Clear();
            }
        }

        #endregion
    }
}