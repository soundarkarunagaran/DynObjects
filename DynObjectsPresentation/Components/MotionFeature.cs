using System;
using System.Windows;
using System.Windows.Media;
using Dyn;

namespace DynObjectsPresentation
{
    public interface IMotionFeature
        : ComponentMsgs.AddedToWorld
    {}

    public class MotionFeature : Component, IMotionFeature, IDisposable
    {
        private readonly Point myVelocity;

        public MotionFeature(Point velocity)
        {
            myVelocity = velocity;
        }

        public void AddedToWorld(World world)
        {
            CompositionTarget.Rendering += CompositionTargetOnRendering;
        }

        public void Dispose()
        {
            CompositionTarget.Rendering -= CompositionTargetOnRendering;
        }

        private void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
        {
            var position = DynThis.GetPosition();
            var newPos = position.Add(myVelocity);
            DynThis.SetPosition(newPos);
        }
    }
}
