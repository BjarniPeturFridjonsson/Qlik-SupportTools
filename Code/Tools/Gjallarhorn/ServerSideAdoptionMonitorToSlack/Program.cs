using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSideAdoptionMonitorToSlack
{
    class Program
    {
        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var work = new Worker();
            await work.DoWork();
        }
    }
}
