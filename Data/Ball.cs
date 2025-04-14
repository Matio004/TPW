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
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }

    #endregion IBall

    #region private

    private Vector Position;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    internal void Move(double width, double height, double diameter)
    {
        // Sprawdź kolizje przed aktualizacją pozycji
        bool bounceX = (Position.x + Velocity.x <= 0) || (Position.x + Velocity.x >= width - diameter);
        bool bounceY = (Position.y + Velocity.y <= 0) || (Position.y + Velocity.y >= height - diameter);

        // Odwróć prędkość jeśli kolizja
        if (bounceX) Velocity = new Vector(-Velocity.x, Velocity.y);
        if (bounceY) Velocity = new Vector(Velocity.x, -Velocity.y);

        // Aktualizuj pozycję
        Position = new Vector(Position.x + Velocity.x, Position.y + Velocity.y);

        RaiseNewPositionChangeNotification();
    }

        #endregion private
    }
}