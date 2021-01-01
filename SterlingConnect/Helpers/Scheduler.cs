using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SterlingConnect.Helpers
{
    public class Scheduler
    {
        static TimeSpan ExchangeTime { get; set; }
        static Scheduler()
        {
            ExchangeTime = new TimeSpan(23, 59, 59);
            BackgroundJob.Schedule(() => KeyExchanger(), ExchangeTime - DateTime.Now.TimeOfDay);
        }

        private static void KeyExchanger()
        {

            BackgroundJob.Schedule(() => KeyExchanger(), ExchangeTime - DateTime.Now.TimeOfDay);
        }


    }
}
