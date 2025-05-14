//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {
    public Ball(Data.IBall ball)
    {
      ball.NewPositionNotification += RaisePositionChangeEvent;
            ball.NewPositionNotification += (sender, position) => collisions(position, ball);
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    private void collisions(IVector Position, Data.IBall ball)
    {
        bool bounceX = (Position.x + ball.Velocity.x <= 0) || (Position.x + ball.Velocity.x >= 800 - 20);
        bool bounceY = (Position.y + ball.Velocity.y <= 0) || (Position.y + ball.Velocity.y >= 420 - 20);

        // Odwróć prędkość jeśli kolizja
        if (bounceX)
        {
            ball.SetVelocty(-ball.Velocity.x, ball.Velocity.y);
        }

        if (bounceY) ball.SetVelocty(ball.Velocity.x, -ball.Velocity.y);




    }

        #endregion private
    }
}