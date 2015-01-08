using System.Windows;
using Dyn;

namespace DynObjectsPresentation
{
    public interface IMissile
        : ComponentMsgs.PositionChanged
    {}

    public class Missile : Component, IMissile
    {
        private readonly DynObject myOwner;

        public Missile(DynObject owner)
        {
            myOwner = owner;
        }

        public void PositionChanged(Point newPosition)
        {
            var world = DynThis.GetWorld();
            if (world == null)
                return;

            foreach (var obj in world.Objects)
            {
                if (obj == myOwner || obj == DynThis)
                    continue;

                var pos = obj.GetPosition();
                var size = obj.GetSize();
                var boundingRect = new Rect(pos, size);

                if (boundingRect.Contains(newPosition))
                {
                    if (obj.Hit(DynThis))
                    {
                        world.RemoveObject(DynThis);
                        break;
                    }
                }
            }
        }
    }
}
