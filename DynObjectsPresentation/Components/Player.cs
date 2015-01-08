using System;
using System.Windows;
using System.Windows.Input;
using Dyn;
using DynObjectsPresentation.Visuals;

namespace DynObjectsPresentation
{
    public interface IMouseController
        : ComponentMsgs.AddedToWorld
        , ComponentMsgs.OnDead
    {}

    public class Player : Component, IMouseController, IDisposable
    {
        private World myWorld;

        public void AddedToWorld(World world)
        {
            myWorld = world;
            var renderer = world.Renderer;
            renderer.MouseMove += RendererOnMouseMove;
            renderer.MouseLeftButtonDown += RendererOnMouseLeftButtonDown;
        }

        public void OnDead()
        {
            DynThis.Mutate(typeof(Player));
        }

        public void Dispose()
        {
            var renderer = myWorld.Renderer;
            renderer.MouseMove -= RendererOnMouseMove;
            renderer.MouseLeftButtonDown -= RendererOnMouseLeftButtonDown;
        }

        private void RendererOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var factory = DynThis.TypeInfo.Factory;
            var missile = factory.CreateObject(
                new MotionFeature(new Point(0, -4)),
                new Missile(DynThis),
                new VisualFeature(typeof(Antenna))
                );

            missile.SetPosition(DynThis.GetPosition().Add(new Point(22, 0)));
            myWorld.AddObject(missile);
        }

        private void RendererOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var mousePos = mouseEventArgs.GetPosition(null);
            var size = DynThis.GetSize();
            DynThis.SetPosition(new Point(mousePos.X - size.Width/2, mousePos.Y - size.Height/2));
        }
    }
}
