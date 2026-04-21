//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using TP.ConcurrentProgramming.BusinessLogic;
using UnderneathLayerAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  /// <summary>
  /// Class Model - implements the <see cref="ModelAbstractApi" />
  /// </summary>
  internal class ModelImplementation : ModelAbstractApi
  {
    internal ModelImplementation() : this(null)
    { }

        public override void Stop()
        {
            layerBellow.Stop();
        }
        internal ModelImplementation(UnderneathLayerAPI underneathLayer)
    {
      layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetBusinessLogicLayer() : underneathLayer;
      eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
    }

    #region ModelAbstractApi

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(Model));
      layerBellow.Dispose();
      Disposed = true;
    }

    public override IDisposable Subscribe(IObserver<IBall> observer)
    {
      return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
    }

        public override void Start(int numberOfBalls, double viewWidth, double viewHeight, double border, double ballDiameter)
        {
 
        layerBellow.Start(numberOfBalls, ballDiameter, (pos, ball) => StartHandler(pos, ball, viewWidth, viewHeight, border));
        }


        private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball, double vWidth, double vHeight, double border)
        {
            var logicDimensions = BusinessLogicAbstractAPI.GetDimensions;

            double effectiveWidth = vWidth - (2 * border);
            double effectiveHeight = vHeight - (2 * border);

            double scaleX = effectiveWidth / logicDimensions.TableWidth;
            double scaleY = effectiveHeight / logicDimensions.TableHeight;

            double scaledDiameter = logicDimensions.BallDimension * scaleX;

            ModelBall newBall = new ModelBall(position.x, position.y, ball, scaleX, scaleY)
            {
                Diameter = scaledDiameter
            };

            BallChanged.Invoke(this, new BallChaneEventArgs() { Ball = newBall });
        }

        #endregion ModelAbstractApi

        #region API

        public event EventHandler<BallChaneEventArgs> BallChanged;

    #endregion API

    #region private

    private bool Disposed = false;
    private readonly IObservable<EventPattern<BallChaneEventArgs>> eventObservable = null;
    private readonly UnderneathLayerAPI layerBellow = null;

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    [Conditional("DEBUG")]
    internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnNumberOfBalls)
    {
      returnNumberOfBalls(layerBellow);
    }

    [Conditional("DEBUG")]
    internal void CheckBallChangedEvent(Action<bool> returnBallChangedIsNull)
    {
      returnBallChangedIsNull(BallChanged == null);
    }

    #endregion TestingInfrastructure
  }

  public class BallChaneEventArgs : EventArgs
  {
    public IBall Ball { get; init; }
  }
}