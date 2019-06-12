using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var worker = new DoWork();
            worker.GetItDone();
        }
    }
}
