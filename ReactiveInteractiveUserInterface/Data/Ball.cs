//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        private bool _isMoving = false;
        private readonly object _lockGuard = new object();
        private IVector _position;
        private IVector _velocity;

        public event EventHandler<IVector>? NewPositionNotification;

        public double Mass { get; }
        public double Radius { get; }

        public IVector Position
        {
            get { lock (_lockGuard) return _position; }
            private set { lock (_lockGuard) _position = value; }
        }

        public IVector Velocity
        {
            get { lock (_lockGuard) return _velocity; }
            set { lock (_lockGuard) _velocity = value; }
        }

        internal Ball(Vector initialPosition, Vector initialVelocity, double mass, double radius)
        {
            _position = initialPosition;
            _velocity = initialVelocity;
            Mass = mass;
            Radius = radius;
        }

        internal async Task StartMovingAsync(CancellationToken cancellationToken)
        {
            _isMoving = true;

            while (_isMoving && !cancellationToken.IsCancellationRequested)
            {
                Move();
                await Task.Delay(16, cancellationToken).ConfigureAwait(false);
            }
        }

        private void Move()
        {
            lock (_lockGuard)
            {
                _position = new Vector(_position.x + _velocity.x, _position.y + _velocity.y);
            }
            NewPositionNotification?.Invoke(this, _position);
        }

        public void Dispose()
        {
            _isMoving = false;
        }
    }
}