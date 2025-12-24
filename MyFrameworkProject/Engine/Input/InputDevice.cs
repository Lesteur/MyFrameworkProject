namespace MyFrameworkProject.Engine.Input
{
    public abstract class InputDevice<TState>
    {
        protected TState _currentState;
        protected TState _previousState;

        public virtual void Update()
        {
            _previousState = _currentState;
            _currentState = GetState();
        }

        public virtual void Reset()
        {
            _currentState = GetState();
            _previousState = _currentState;
        }

        protected abstract TState GetState();
    }
}