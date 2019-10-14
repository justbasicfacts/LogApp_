using EventLogProject;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogAppTest
{
    [TestClass]
    public class LogAppUnitTest
    {
        private const string Path = @"hadi len";
        private const string LongPath= @"D:\lgbig.txt";

        [TestMethod]
        public void LogAppTestSync()
        {
            EventLogsFileProcessor eventLogProcessor = new EventLogsFileProcessor(Path);
            eventLogProcessor.ProcessLogs();
        }


        [TestMethod]
        public void LogAppBigData()
        {
            EventLogsFileProcessor eventLogProcessor = new EventLogsFileProcessor(LongPath);
            eventLogProcessor.ProcessLogs();
        }
    }
}
