using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCServer
{
    public class Generator
    {
        public string Value { get; set; }
        private Thread generatorThread;
        private bool isRunning;

        private DateTime startTime = DateTime.Now;

        public void Start()
        {
            isRunning = true;
            generatorThread = new Thread(GetSinValue);
            generatorThread.Start();
        }

        private void GetSinValue()
        {
            while (isRunning)
            {
                var t = (DateTime.Now - startTime).TotalMilliseconds / 10000;
                //var t = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond;
                Value = Math.Sin(t * 2 * Math.PI).ToString("0.0000000000000");
                System.Threading.Thread.Sleep(100);
            }
        }
        public void Stop()
        {
            isRunning = false;
            generatorThread.Join();
        }
    }
}
