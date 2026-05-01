//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic
{
    public abstract class BusinessLogicAbstractAPI : IDisposable
    {
        public abstract void Stop();

        public static BusinessLogicAbstractAPI GetBusinessLogicLayer()
        {
            return modelInstance.Value;
        }


        public static Dimensions GetDimensions { get; set; } = new Dimensions(420.0, 400.0);

        public abstract void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler);

        public abstract void Dispose();

        private static Lazy<BusinessLogicAbstractAPI> modelInstance = new Lazy<BusinessLogicAbstractAPI>(() => new BusinessLogicImplementation());

    }
    /// <summary>
    /// Immutable type representing table dimensions
    /// </summary>
    /// <param name="BallDimension"></param>
    /// <param name="TableHeight"></param>
    /// <param name="TableWidth"></param>
    /// <remarks>
    /// Must be abstract
    /// </remarks>
    public record Dimensions(double TableHeight, double TableWidth);

    public interface IPosition
    {
        double x { get; init; }
        double y { get; init; }
    }

    public interface IBall
    {
        event EventHandler<IPosition> NewPositionNotification;
        double Diameter { get; }
    }
}