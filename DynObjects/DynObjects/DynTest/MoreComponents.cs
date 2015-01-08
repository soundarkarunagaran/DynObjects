using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DynTest.More
{
    public sealed partial class DefaultMessageImplementations
    {
        public void DoSomething()
        {
        }
    }

    public class Web : MoreMsgs.Browse, GameMsgs.Baz
    {
        public int Neuron = 1234567;

        public void Browse()
        {
        }

        public void Baz()
        {
        }
    }

    public class View : MoreMsgs.Show
    {
        public int Flags = 15;

        public void Show()
        {
        }
    }

    public class Something : MoreMsgs.DoSomething
    {
        public int Subsets = 2;

        public void DoSomething()
        {
        }
    }
}
