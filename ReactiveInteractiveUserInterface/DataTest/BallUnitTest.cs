using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        private const string TestLogFile = "test_ball_diagnostics.json";

        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector initialPosition = new Vector(0.0, 0.0);
            Vector initialVelocity = new Vector(100.0, 100.0);
            double mass = 15.0;
            double radius = 10.0;

 
            using DiagnosticsLogger testLogger = new DiagnosticsLogger(TestLogFile);
            Ball newInstance = new(initialPosition, initialVelocity, mass, radius, testLogger);

            Assert.AreEqual<IVector>(initialVelocity, newInstance.Velocity);
            Assert.AreEqual<double>(mass, newInstance.Mass);
            Assert.AreEqual<double>(radius, newInstance.Radius);
            Assert.AreEqual<IVector>(initialPosition, newInstance.Position);
        }

        [TestMethod]
        public void MoveTestMethod()
        {
            using DiagnosticsLogger testLogger = new DiagnosticsLogger(TestLogFile);
            Vector initialPosition = new(10.0, 10.0);
            Vector initialVelocity = new(2.0, 3.0);
            Ball newInstance = new(initialPosition, initialVelocity, 10.0, 10.0, testLogger);

            IVector currentPosition = new Vector(0.0, 0.0);
            int numberOfCallBackCalled = 0;

            newInstance.NewPositionNotification += (sender, position) =>
            {
                Assert.IsNotNull(sender);
                currentPosition = position;
                numberOfCallBackCalled++;
            };
            MethodInfo moveMethod = typeof(Ball).GetMethod("Move", BindingFlags.NonPublic | BindingFlags.Instance)!;
            moveMethod.Invoke(newInstance, null);

            Assert.AreEqual<int>(1, numberOfCallBackCalled);
            Assert.AreEqual<double>(12.0, currentPosition.x);
            Assert.AreEqual<double>(13.0, currentPosition.y);
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