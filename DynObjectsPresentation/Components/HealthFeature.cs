using System.Windows;
using Dyn;

namespace DynObjectsPresentation
{
    public interface IHealth
        : ComponentMsgs.Hit
        , ComponentMsgs.OnDead
    {}

    public class HealthFeature : Component, IHealth
    {
        private bool myIsDead;

        public bool Hit(DynObject byWhom)
        {
            if (myIsDead)
                return false;

            myIsDead = true;
            DynThis.OnDead();
            return true;
        }

        public void OnDead()
        {
            var fader = Util.CreateDoubleAnimator(DynThis.GetComposer(), UIElement.OpacityProperty, null, 0, 1, null);
            fader.Completed += (sender, args) => DynThis.GetWorld().RemoveObject(DynThis);
            fader.Begin();
        }
    }
}
