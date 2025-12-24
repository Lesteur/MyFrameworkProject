using MyFrameworkProject.Engine.Graphics;
using System.Collections.Generic;

namespace MyFrameworkProject.Engine.Core
{
    public class GameLoop
    {
        private readonly List<Entity> _entities = [];

        public GameLoop()
        {
            Logger.Info("GameLoop created");
        }

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        public void Update()
        {
            foreach (var entity in _entities)
            {
                entity.Update(Time.DeltaTime);
            }
        }

        public void Draw(Renderer renderer)
        {
            renderer.BeginWorld();

            foreach (var entity in _entities)
            {
                renderer.DrawEntity(entity);
            }

            renderer.EndWorld();

            renderer.BeginUI();

            renderer.EndUI();
        }
    }
}