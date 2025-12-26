using MyFrameworkProject.Engine.Graphics;
namespace MyFrameworkProject.Engine.Components
{
    public class GameObject : Entity
    {
        #region Static Fields

        /// <summary>
        /// Global counter used to generate unique identifiers for each entity instance.
        /// </summary>
        private static int _counter = 0;

        #endregion

        #region Fields - Identity

        /// <summary>
        /// Unique identifier for this entity instance.
        /// </summary>
        private readonly uint _gameId;

        public uint Id => _gameId;

        #endregion

        private bool _active;

        public void SetActive(bool active)
        {
            _active = active;
        }

        public bool Active => _active;

        public GameObject() : base()
        {
            _gameId = (uint)++_counter;
        }

        public GameObject(Sprite sprite) : base(sprite)
        {
            _gameId = (uint)++_counter;
        }

        public virtual void BeforeUpdate(float deltaTime)
        {
            // Custom logic before update
        }

        public virtual void Update(float deltaTime)
        {
            // Custom update logic
        }

        public virtual void AfterUpdate(float deltaTime)
        {
            // Custom logic after update
        }
    }
}