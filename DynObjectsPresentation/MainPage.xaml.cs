using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Dyn;
using DynObjectsPresentation.Visuals;

namespace DynObjectsPresentation
{
    public partial class MainPage : UserControl
    {
        private DynFactory myFactory;
        private World myWorld;
        private int myScore;

        private Storyboard mySpawner = new Storyboard { Duration = TimeSpan.FromSeconds(1) };

        public MainPage()
        {
            InitializeComponent();
            
            CreateFactory();

            myWorld = new World(LayoutRoot);

            DynObject playerObj = myFactory.CreateObject(new VisualFeature(typeof(OrGate)), new Player());
            myWorld.AddObject(playerObj);

            mySpawner.Completed += SpawnerOnCompleted;
            mySpawner.Begin();
        }

        private void CreateFactory()
        {
            myFactory = new DynFactory(new Components());
            myFactory.AddMandatoryComponent(typeof(WorldObject));
            myFactory.AddMandatoryComponent(typeof(Beacon));
            myFactory.AddInjectionRule(typeof(Player), typeof(HealthFeature));
            myFactory.AddInjectionRule(typeof(MonsterFeature), typeof(HealthFeature));
        }

        private void SpawnerOnCompleted(object sender, EventArgs eventArgs)
        {
            var enemy = MonsterFeature.CreateEnemy(myFactory);
            enemy.GetBeacon().OnDeadBeacon += o =>
                                                  {
                                                      myScore++;
                                                      myScoreText.Text = myScore.ToString();
                                                  };

            myWorld.AddObject(enemy);
            mySpawner.Begin();
        }
    }
}
