using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector initialPosition = new Vector(0.0, 0.0);
            Vector initialVelocity = new Vector(1.0, 1.0);
            double mass = 15.0;
            double radius = 10.0;

            Ball newInstance = new(initialPosition, initialVelocity, mass, radius);

            Assert.AreEqual<IVector>(initialVelocity, newInstance.Velocity);
            Assert.AreEqual<double>(mass, newInstance.Mass);
            Assert.AreEqual<double>(radius, newInstance.Radius);
            Assert.AreEqual<IVector>(initialPosition, newInstance.Position);
        }

        [TestMethod]
        public void MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            Vector initialVelocity = new(2.0, 3.0);
            Ball newInstance = new(initialPosition, initialVelocity, 10.0, 10.0);

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
    }
}