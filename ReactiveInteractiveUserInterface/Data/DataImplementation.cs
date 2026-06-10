//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private bool Disposed = false;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = new();
        private List<Task> BallTasks = new();
        private CancellationTokenSource? Cts;


        private DiagnosticsLogger? _logger;

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null) throw new ArgumentNullException(nameof(upperLayerHandler));

            Stop();
            Cts = new CancellationTokenSource();
            _logger = new DiagnosticsLogger(); 

            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new Vector(RandomGenerator.Next(100, 300), RandomGenerator.Next(100, 300));
                Vector startingVelocity = new Vector((RandomGenerator.NextDouble() - 0.5) * 5, (RandomGenerator.NextDouble() - 0.5) * 5);

                double radius = RandomGenerator.Next(10, 30);
                double mass = radius;

                Ball newBall = new Ball(startingPosition, startingVelocity, mass, radius, _logger);
                BallsList.Add(newBall);

                upperLayerHandler(startingPosition, newBall);

                BallTasks.Add(newBall.StartMovingAsync(Cts.Token));
            }
        }

        public override void Stop()
        {
            if (Cts != null)
            {
                Cts.Cancel();
                Cts.Dispose();
                Cts = null;
            }

            foreach (var ball in BallsList)
            {
                ball.Dispose();
            }

            BallsList.Clear();
            BallTasks.Clear();

           
            if (_logger != null)
            {
                _logger.Dispose();
                _logger = null;
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Stop();
                }
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(nameof(DataImplementation));
            }
        }

        public override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

    }
}