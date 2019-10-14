using EventLogProject;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogAppTest
{
    [TestClass]
    public class LogAppUnitTest
    {
        private const string __PATH__ = "Should enter file path";

        [TestMethod]
        public void ProcessLogFile()
        {
            EventLogsFileProcessor eventLogProcessor = new EventLogsFileProcessor(__PATH__);
            eventLogProcessor.ProcessFile();
        }
    }
}
