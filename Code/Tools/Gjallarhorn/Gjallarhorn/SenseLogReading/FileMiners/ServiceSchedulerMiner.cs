//using System.Diagnostics;
//using Eir.Common.Extensions;

//namespace Gjallarhorn.SenseLogReading.FileMiners
//{
//    public class ServiceSchedulerMiner : BaseDataMiner, IDataMiner
//    {
//        public string MinerName => "Service_Scheduler";

//        public void Mine(string line)
//        {

//            base.MineFile(line, Analyze);
//        }

//        public void FinaliseStatistics()
//        {
//            //not needed for this miner
//        }

//        private void Analyze(int colNr, string value)
//        {

//            if (colNr == 18)
//            {
//                if (value == "0")
//                {
//                    BasicDataFromCase.ServiceSchedulerSuccess++;
//                }
//                else if (value?.IsDigitsOnly()??false)
//                {
//                    BasicDataFromCase.ServiceSchedulerFailures++;
//                }
//                else
//                {
//                    //todo:Logging failures
//                    Trace.WriteLine($"something wrong in serviceScheduleMiner should be int = {value}");
//                }

//            }
//        }
//    }
//}

