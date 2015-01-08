using System;
using System.Windows;
using System.Windows.Controls;
using Dyn;

namespace DynObjectsPresentation
{
    public interface IVisual
        : ComponentMsgs.AddedToWorld
        , ComponentMsgs.SetPosition
        , ComponentMsgs.GetPosition
        , ComponentMsgs.GetSize
        , ComponentMsgs.GetComposer
    {}

    public class VisualFeature : Component, IVisual, IDisposable
    {
        private readonly Type myVisualType;

        private Point myPosition;
        private Canvas myComposer = new Canvas();
        private Size mySize;
        private World myWorld;

        public void AddedToWorld(World world)
        {
            myWorld = world;

            var visual = (FrameworkElement) Activator.CreateInstance(myVisualType);

            myComposer.Children.Add(visual);
            mySize.Width = visual.Width;
            mySize.Height = visual.Height;

            myWorld.Renderer.Children.Add(myComposer);
        }

        public void Dispose()
        {
            myWorld.Renderer.Children.Remove(myComposer);
        }

        public void SetPosition(Point position)
        {
            myPosition = position;

            Canvas.SetLeft(myComposer, position.X);
            Canvas.SetTop(myComposer, position.Y);

            DynThis.PositionChanged(myPosition);
        }

        public Point GetPosition()
        {
            return myPosition;
        }

        public Size GetSize()
        {
            return mySize;
        }

        public Canvas GetComposer()
        {
            return myComposer;
        }

        public VisualFeature(Type visualType)
        {
            myVisualType = visualType;
        }
    }
}
