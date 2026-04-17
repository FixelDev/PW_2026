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
    public Ball(Data.IBall ball)
    {
      ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
            var dimensions = BusinessLogicAbstractAPI.GetDimensions;
            double new_x = e.x;
            double new_y = e.y;
            if (sender is Data.IBall dataBall)
            {
                if(e.x < 0)
                {
                    new_x = 0 ;
                }
                if(e.x > dimensions.TableWidth - dimensions.BallDimension )
                {
                    new_x = dimensions.TableWidth - dimensions.BallDimension - 8;
                }
                if (e.y < 0 )
                {
                    new_y = 0;
                }
                if (e.y > dimensions.TableHeight - dimensions.BallDimension)
                {
                    new_y = dimensions.TableHeight - dimensions.BallDimension -8 ;
                }
            }
    NewPositionNotification?.Invoke(this, new Position(new_x, new_y));
    }

    #endregion private
  }
}