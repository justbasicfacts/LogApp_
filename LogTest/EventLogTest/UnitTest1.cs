using LogApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogAppTest
{
    [TestClass]
    public class LogAppUnitTest
    {
        private const string Path = @"D:\logTest.txt";

        [TestMethod]
        public void LogAppTest()
        {
            EventLogProcessor eventLogProcessor = new LogApp.EventLogProcessor(Path);
            eventLogProcessor.ProcessFile();
        }
    }
}
