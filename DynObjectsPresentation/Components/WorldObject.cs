using System;
using Dyn;

namespace DynObjectsPresentation
{
    public interface IWorldObject
        : ComponentMsgs.AddedToWorld
        , ComponentMsgs.GetWorld
    {}

    public class WorldObject : IWorldObject, IDisposable
    {
        private World myWorld;

        [Priority(1000)]
        public void AddedToWorld(World world)
        {
            myWorld = world;
        }

        public void Dispose()
        {
            myWorld = null;
        }

        public World GetWorld()
        {
            return myWorld;
        }
    }
}
