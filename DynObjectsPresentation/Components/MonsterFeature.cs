using System;
using System.Windows;
using System.Windows.Media.Animation;
using Dyn;
using DynObjectsPresentation.Visuals;

namespace DynObjectsPresentation
{
    public interface IEnemy
        : ComponentMsgs.AddedToWorld
        , ComponentMsgs.OnDead
    {}

    public class MonsterFeature : Component, IEnemy, IDisposable
    {
        private Storyboard myMissileLauncher = new Storyboard { Duration = TimeSpan.FromSeconds(3) };
        
        public static DynObject CreateEnemy(DynFactory factory)
        {
            var visualType = RandomEnemyVisual();

            var enemy = factory.CreateObject(
                new MonsterFeature(),
                new VisualFeature(visualType),
                new MotionFeature(new Point(0, 1)));
            enemy.SetPosition(new Point(RNG.Random.NextDouble() * 600, -60));

            return enemy;
        }

        private static Type RandomEnemyVisual()
        {
            var visuals = new[] { typeof(Resistor), typeof(Diode), typeof(Transistor), typeof(Capacitor) };
            return visuals[RNG.Random.Next(visuals.Length)];
        }

        public void AddedToWorld(World world)
        {
            myMissileLauncher.Completed += MissileLauncherOnCompleted;
            myMissileLauncher.Begin();
        }

        public void OnDead()
        {
            DynThis.Mutate(typeof(MonsterFeature), typeof(MotionFeature));
        }

        public void Dispose()
        {
            myMissileLauncher.Stop();
            myMissileLauncher = null;
        }

        private void MissileLauncherOnCompleted(object sender, EventArgs eventArgs)
        {
            if (myMissileLauncher == null)
                return;

            var missile = DynThis.TypeInfo.Factory.CreateObject(
                new MotionFeature(new Point(0, 3)),
                new Missile(DynThis),
                new VisualFeature(typeof(Grounding)));

            missile.SetPosition(DynThis.GetPosition());

            DynThis.GetWorld().AddObject(missile);
            myMissileLauncher.Begin();
        }
    }
}
