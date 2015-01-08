using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Dyn;

namespace DynObjectsPresentation
{
    public class World
    {
        private HashSet<DynObject> myObjects = new HashSet<DynObject>();
        private HashSet<DynObject> myRemovalQueue = new HashSet<DynObject>();

        public Canvas Renderer { get; private set; }

        public ICollection<DynObject> Objects { get { return myObjects; } }

        public World(Canvas renderer)
        {
            Renderer = renderer;

            CompositionTarget.Rendering += CompositionTargetOnRendering;
        }

        public void AddObject(DynObject obj)
        {
            myObjects.Add(obj);
            obj.AddedToWorld(this);
        }

        public void RemoveObject(DynObject obj)
        {
            myRemovalQueue.Add(obj);
        }

        private void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
        {
            foreach (var obj in myRemovalQueue)
            {
                myObjects.Remove(obj);
                obj.Dispose();
            }
            myRemovalQueue.Clear();
        }
    }
}
