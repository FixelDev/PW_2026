//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

            Ball newInstance = new(initialPosition, initialVelocity);

            Assert.AreEqual<IVector>(initialVelocity, newInstance.Velocity);
        }

        [TestMethod]
        public void MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            Vector initialVelocity = new(2.0, 3.0);
            Ball newInstance = new(initialPosition, initialVelocity);

            IVector currentPosition = new Vector(0.0, 0.0);
            int numberOfCallBackCalled = 0;

            newInstance.NewPositionNotification += (sender, position) =>
            {
                Assert.IsNotNull(sender);
                currentPosition = position;
                numberOfCallBackCalled++;
            };

            newInstance.Move(new Vector(2.0, 3.0));

            Assert.AreEqual<int>(1, numberOfCallBackCalled);
            Assert.AreEqual<double>(12.0, currentPosition.x);
            Assert.AreEqual<double>(13.0, currentPosition.y);
        }
    }
}