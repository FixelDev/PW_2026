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
    internal class Ball : IBall
    {
        private readonly Data.IBall _dataBall;
        private readonly BusinessLogicImplementation _logic;

        public event EventHandler<IPosition>? NewPositionNotification;

        public double Diameter => _dataBall.Radius * 2;
        internal Data.IBall DataBall => _dataBall;

        public Ball(Data.IBall dataBall, BusinessLogicImplementation logic)
        {
            _dataBall = dataBall;
            _logic = logic;
            _dataBall.NewPositionNotification += RaisePositionChangeEvent;
        }

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            var dimensions = BusinessLogicAbstractAPI.GetDimensions;
            double radius = _dataBall.Radius;

            lock (_logic.CollisionLock)
            {
                double newVx = _dataBall.Velocity.x;
                double newVy = _dataBall.Velocity.y;

                if (e.x <= 0 || e.x >= dimensions.TableWidth - Diameter)
                    newVx = -newVx;

                if (e.y <= 0 || e.y >= dimensions.TableHeight - Diameter)
                    newVy = -newVy;

                if (newVx != _dataBall.Velocity.x || newVy != _dataBall.Velocity.y)
                {
                    _dataBall.Velocity = new Data.Vector(newVx, newVy);
                }

                double center1X = e.x + radius;
                double center1Y = e.y + radius;

                foreach (var otherBall in _logic.LogicBalls.Where(b => b != this))
                {
                    double center2X = otherBall.DataBall.Position.x + otherBall.DataBall.Radius;
                    double center2Y = otherBall.DataBall.Position.y + otherBall.DataBall.Radius;

                    double dx = center1X - center2X;
                    double dy = center1Y - center2Y;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    if (distance <= radius + otherBall.DataBall.Radius)
                    {
                        ResolveCollision(_dataBall, otherBall.DataBall);
                    }
                }
            }

            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }

        private void ResolveCollision(Data.IBall b1, Data.IBall b2)
        {
            double c1x = b1.Position.x + b1.Radius;
            double c1y = b1.Position.y + b1.Radius;

            double c2x = b2.Position.x + b2.Radius;
            double c2y = b2.Position.y + b2.Radius;

            double dx = c1x - c2x;
            double dy = c1y - c2y;
            double distanceSquared = dx * dx + dy * dy;

            if (distanceSquared == 0) return; 

            double dvx = b1.Velocity.x - b2.Velocity.x;
            double dvy = b1.Velocity.y - b2.Velocity.y;
            
            double dotProduct = dx * dvx + dy * dvy;

            if (dotProduct > 0) return;

            double massSum = b1.Mass + b2.Mass;
            double collisionScale = dotProduct / distanceSquared;

            double collisionWeight1 = (2 * b2.Mass / massSum) * collisionScale;
            double collisionWeight2 = (2 * b1.Mass / massSum) * collisionScale;

            b1.Velocity = new Data.Vector(
                b1.Velocity.x - collisionWeight1 * dx,
                b1.Velocity.y - collisionWeight1 * dy
            );

            b2.Velocity = new Data.Vector(
                b2.Velocity.x + collisionWeight2 * dx,
                b2.Velocity.y + collisionWeight2 * dy
            );
        }
    }
}