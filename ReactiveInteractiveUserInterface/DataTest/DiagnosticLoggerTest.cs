using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class DiagnosticsLoggerUnitTest
    {
        private const string TestLogFile = "test_concurrent_logger.json";

        [TestMethod]
        public void LoggerCreatesFileAndWritesData()
        {
            Vector pos = new Vector(10, 10);
            Vector vel = new Vector(5, 5);

            using (DiagnosticsLogger logger = new DiagnosticsLogger(TestLogFile))
            {
                Ball dummyBall = new Ball(pos, vel, 10, 10, logger);

                logger.LogBallState(dummyBall);
            }

            Assert.IsTrue(File.Exists(TestLogFile));

            string content = File.ReadAllText(TestLogFile);
            Assert.IsTrue(content.Contains("PosX"));
            Assert.IsTrue(content.Contains("10"));
        }

        [TestMethod]
        public async Task LoggerHandlesConcurrentWrites()
        {
            using (DiagnosticsLogger logger = new DiagnosticsLogger(TestLogFile))
            {
                Vector pos = new Vector(0, 0);
                Vector vel = new Vector(1, 1);
                Ball dummyBall = new Ball(pos, vel, 10, 10, logger);

                Task[] tasks = new Task[10];
                for (int i = 0; i < 10; i++)
                {
                    tasks[i] = Task.Run(() =>
                    {
                        for (int j = 0; j < 50; j++)
                        {
                            logger.LogBallState(dummyBall);
                        }
                    });
                }

                await Task.WhenAll(tasks);
            }

            string[] lines = File.ReadAllLines(TestLogFile);
            Assert.AreEqual(500, lines.Length);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(TestLogFile))
            {
                File.Delete(TestLogFile);
            }
        }
    }
}