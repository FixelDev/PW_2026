using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicAbstractAPIUnitTest
    {
        [TestMethod]
        public void BusinessLogicConstructorTestMethod()
        {
            BusinessLogicAbstractAPI instance1 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            BusinessLogicAbstractAPI instance2 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            Assert.AreSame(instance1, instance2);
            instance1.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => instance2.Dispose());
        }

        [TestMethod]
        public void GetDimensionsTestMethod()
        {
            // Usunięto parametr BallDimension (20.0)
            Assert.AreEqual<Dimensions>(new Dimensions(420.0, 400.0), BusinessLogicAbstractAPI.GetDimensions);

            Dimensions newDimensions = new Dimensions(500.0, 500.0);
            BusinessLogicAbstractAPI.GetDimensions = newDimensions;
            Assert.AreEqual<Dimensions>(newDimensions, BusinessLogicAbstractAPI.GetDimensions);
        }
    }
}