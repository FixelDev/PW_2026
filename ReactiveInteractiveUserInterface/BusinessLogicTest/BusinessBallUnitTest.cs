using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void MoveTestMethod()
        {
            BusinessLogicAbstractAPI.GetDimensions = new Dimensions(420.0, 400.0);

            DataBallFixture dataBallFixture = new DataBallFixture();
             
            using BusinessLogicImplementation logicLayer = new BusinessLogicImplementation(new DummyDataLayer());

            Ball newInstance = new(dataBallFixture, logicLayer);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
            dataBallFixture.Move();
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
        }

        private class DummyDataLayer : Data.DataAbstractAPI
        {
            public override void Start(int numberOfBalls, Action<Data.IVector, Data.IBall> upperLayerHandler) { }
            public override void Stop() { }
            public override void Dispose() { }
        }

        private class DataBallFixture : Data.IBall
        {
            public Data.IVector Velocity { get; set; } = new VectorFixture(1.0, 1.0);
            public Data.IVector Position { get; } = new VectorFixture(10.0, 10.0);
            public double Mass { get; } = 10.0;
            public double Radius { get; } = 10.0;

            public event EventHandler<Data.IVector>? NewPositionNotification;

            public void Dispose() { }

            internal void Move()
            {
                NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
            }
        }

        private class VectorFixture : Data.IVector
        {
            internal VectorFixture(double X, double Y)
            {
                x = X; y = Y;
            }

            public double x { get; init; }
            public double y { get; init; }
        }
    }
}