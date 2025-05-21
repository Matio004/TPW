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
    public Ball(Data.IBall ball, List<Data.IBall> ballList)
    {
      ball.NewPositionNotification += RaisePositionChangeEvent;
            ball.NewPositionNotification += (sender, position) => collisions(position, ball);
            this.ballList = ballList;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        private List<Data.IBall> ballList;

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    private void collisions(IVector Position, Data.IBall ball)
    {
            bool lockWasTaken = false;
            try
            {
                Monitor.Enter(this, ref lockWasTaken);
                if (!lockWasTaken)
                    throw new ArgumentException();
                bool bounceX = (Position.x + ball.Velocity.x <= 0) || (Position.x + ball.Velocity.x >= 800 - 20 - 8);
                bool bounceY = (Position.y + ball.Velocity.y <= 0) || (Position.y + ball.Velocity.y >= 420 - 20 - 8);

                // Odwróć prędkość jeśli kolizja
                if (bounceX)
                {
                    ball.SetVelocty(-ball.Velocity.x, ball.Velocity.y);
                }

                if (bounceY) ball.SetVelocty(ball.Velocity.x, -ball.Velocity.y);

                foreach (Data.IBall other in ballList)
                {
                    if (ReferenceEquals(other, ball)) continue;

                    double x1 = ball.getPos().x;
                    double y1 = ball.getPos().y;

                    double x2 = other.getPos().x;
                    double y2 = other.getPos().y;

                    double dx = x1 - x2;
                    double dy = y1 - y2;

                    double euclideanDistance = Math.Sqrt(dx * dx + dy * dy);
                    double minDistance = (20 + 20) / 2;

                    if (euclideanDistance > 0 && euclideanDistance < minDistance)
                    {

                        double nx = dx / euclideanDistance;
                        double ny = dy / euclideanDistance;

                        //Velocity
                        double v1x = ball.Velocity.x;
                        double v1y = ball.Velocity.y;
                        double v2x = other.Velocity.x;
                        double v2y = other.Velocity.y;

                        //Mass
                        double m1 = 1;
                        double m2 = 1;

                        //product of Velocity and normal
                        double v1n = v1x * nx + v1y * ny;
                        double v2n = v2x * nx + v2y * ny;

                        double v1nAfter = (v1n * (m1 - m2) + 2 * m2 * v2n) / (m1 + m2);
                        double v2nAfter = (v2n * (m2 - m1) + 2 * m1 * v1n) / (m1 + m2);

                        double dv1x = (v1nAfter - v1n) * nx;
                        double dv1y = (v1nAfter - v1n) * ny;
                        double dv2x = (v2nAfter - v2n) * nx;
                        double dv2y = (v2nAfter - v2n) * ny;

                        ball.SetVelocty(v1x + dv1x, v1y + dv1y);
                        other.SetVelocty(v2x + dv2x, v2y + dv2y);

                        double overlap = minDistance - euclideanDistance;
                        if (overlap > 0)
                        {
                            double adjust = overlap * 0.5;
                            ball.getPos().x += nx * adjust;
                            ball.getPos().y += ny * adjust;
                            other.getPos().x -= nx * adjust;
                            other.getPos().y -= ny * adjust;
                        }
                    }

                }

            }
            finally
            {
                if (lockWasTaken)
                    Monitor.Exit(this);
            }



        }

        #endregion private
    }
}