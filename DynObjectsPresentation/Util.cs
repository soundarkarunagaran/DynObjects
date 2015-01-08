using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DynObjectsPresentation
{
    public static class RNG
    {
        public readonly static Random Random = new Random();
    }

    public static class PointExtensions
    {
        public static Point Add(this Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }
    }

    public static class Util
    {
        public static Storyboard CreateDoubleAnimator(DependencyObject target, DependencyProperty property, double? from, double? to, double durationSec, IEasingFunction easingFunction)
        {
            var storyboard = new Storyboard();
            var animation = new DoubleAnimation { From = from, To = to, Duration = TimeSpan.FromSeconds(durationSec), EasingFunction = easingFunction };
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(property));
            storyboard.Children.Add(animation);
            return storyboard;
        }
    }
}