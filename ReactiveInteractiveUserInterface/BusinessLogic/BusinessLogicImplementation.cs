//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        internal List<Ball> LogicBalls { get; } = new List<Ball>();
        internal readonly object CollisionLock = new object();

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        public BusinessLogicImplementation() : this(null)
        { }



        public override void Stop()
        {
            layerBellow.Stop();
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(BusinessLogicImplementation));

            BusinessLogicAbstractAPI.GetDimensions = new Dimensions(GetDimensions.TableHeight, GetDimensions.TableWidth);

            LogicBalls.Clear();

            layerBellow.Start(numberOfBalls, (startingPosition, databall) =>
            {
                var logicBall = new Ball(databall, this);
                LogicBalls.Add(logicBall);

                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }

        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBellow;

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }
    }
}