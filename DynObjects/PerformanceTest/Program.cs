using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Dyn;

namespace PerformanceTest
{
    public class PerfTester
    {
        private const int Iterations = 10000;

        private Stopwatch myStopwatch = new Stopwatch();

        private long myStaticMethodTestTime;
        private long myUnicastMessageTestTime;
        private long myDictLookupTestTime;

        public void Run()
        {
            PrepareProcess();

            RunStaticMethodTest();
            RunUnicastMessageTest();
            RunDictLookupMethodTest();

            PrintResults();
        }

        private void PrepareProcess()
        {
            var process = Process.GetCurrentProcess();
            process.ProcessorAffinity = (IntPtr) 1;
            process.PriorityClass = ProcessPriorityClass.RealTime;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Thread.Yield();
        }

        private void PrintResults()
        {
            Console.Out.WriteLine("Static method: {0:#.#} ns/call", PrepareForOutput(myStaticMethodTestTime));
            Console.Out.WriteLine("Unicast message: {0:#.#} ns/call", PrepareForOutput(myUnicastMessageTestTime));
            Console.Out.WriteLine("Dictionary lookup method: {0:#.#} ns/call", PrepareForOutput(myDictLookupTestTime));
        }

        private static double PrepareForOutput(long time)
        {
            var callsPerSec = checked(Stopwatch.Frequency * Iterations / time);
            return 1000000000.0 / callsPerSec;
        }

        private void RunStaticMethodTest()
        {
            //warm up
            StaticMethod();

            myStopwatch.Start();
            for (int i = 0; i < Iterations; ++i)
            {
                StaticMethod();
            }
            myStopwatch.Stop();
            var once = myStopwatch.ElapsedTicks;
            myStopwatch.Start();
            for (int i = 0; i < Iterations; ++i)
            {
                StaticMethod();
                StaticMethod();
            }
            myStopwatch.Stop();

            myStaticMethodTestTime = myStopwatch.ElapsedTicks - once;
        }

        private void RunDictLookupMethodTest()
        {
            //warm up
            DictLookupMethod();

            myStopwatch.Start();
            for (int i = 0; i < Iterations; ++i)
            {
                DictLookupMethod();
            }
            myStopwatch.Stop();
            var once = myStopwatch.ElapsedTicks;
            myStopwatch.Start();
            for (int i = 0; i < Iterations; ++i)
            {
                DictLookupMethod();
                DictLookupMethod();
            }
            myStopwatch.Stop();

            myDictLookupTestTime = myStopwatch.ElapsedTicks - once;
        }

        private void RunUnicastMessageTest()
        {
            var factory = new DynFactory(new PerfComponents());
            var perf = new Perf();
            var obj = factory.CreateObject(perf);
            obj.Unicast();

            myStopwatch.Start();
            for (int i = 0; i < Iterations; ++i)
                obj.Unicast();
            myStopwatch.Stop();
            var once = myStopwatch.ElapsedTicks;
            myStopwatch.Start();
            for (int i = 0; i < Iterations; ++i)
            {
                obj.Unicast();
                obj.Unicast();
            }
            myStopwatch.Stop();

            myUnicastMessageTestTime = myStopwatch.ElapsedTicks - once;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void StaticMethod()
        {
        }

        private static Dictionary<int, string> dict = new Dictionary<int, string> { { 1, "string" } };
        private static string DictLookupMethod()
        {
            return dict[1];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            new PerfTester().Run();
        }
    }

    public class Perf
        : PerfComponentsMsgs.Unicast
        , PerfComponentsMsgs.Multicast
    {
        public void Unicast()
        {
        }

        public void Multicast()
        {
        }

        public virtual void VirtualMethod()
        {
            
        }
    }

    public class Perf2 : PerfComponentsMsgs.Multicast
    {
        public void Multicast()
        {
        }
    }

    public class Perf3 : PerfComponentsMsgs.Multicast
    {
        public void Multicast()
        {
        }
    }
}
