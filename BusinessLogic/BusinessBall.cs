﻿//____________________________________________________________________________________________________________________________________
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
    public Ball(Data.IBall ball, List<Ball> ballList, Object BallLock)
    {
      ball.NewPositionNotification += RaisePositionChangeEvent;
            this.ballList = ballList;
            this.BallLock = BallLock;
            dataBall = ball;
        }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        private List<Ball> ballList;
        private Data.IBall dataBall;
        private readonly object BallLock;

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
            lock (BallLock) {
                bool bounceX = (e.x  <= 0) || (e.x >= 800 - 20 - 8);
                bool bounceY = (e.y <= 0) || (e.y >= 420 - 20 - 8);

                // Odwróć prędkość jeśli kolizja
                if (bounceX)
                {
                    dataBall.SetVelocty(-dataBall.Velocity.x, dataBall.Velocity.y);
                }

                if (bounceY) dataBall.SetVelocty(dataBall.Velocity.x, -dataBall.Velocity.y);

                

                foreach (Ball other in ballList)
                {
                    if (ReferenceEquals(other, this)) continue;

                    double x1 = dataBall.Position.x;
                    double y1 = dataBall.Position.y;

                    double x2 = other.dataBall.Position.x;
                    double y2 = other.dataBall.Position.y;

                    double dx = x1 - x2;
                    double dy = y1 - y2;

                    double euclideanDistance = Math.Sqrt(dx * dx + dy * dy);
                    double minDistance = (dataBall.Diameter + other.dataBall.Diameter) / 2;

                    if (euclideanDistance > 0 && euclideanDistance < minDistance)
                    {

                        double nx = dx / euclideanDistance;
                        double ny = dy / euclideanDistance;

                        //Velocity
                        double v1x = dataBall.Velocity.x;
                        double v1y = dataBall.Velocity.y;
                        double v2x = other.dataBall.Velocity.x;
                        double v2y = other.dataBall.Velocity.y;

                        //Mass
                        double m1 = dataBall.Mass;
                        double m2 = other.dataBall.Mass;

                        //product of Velocity and normal
                        double v1n = v1x * nx + v1y * ny;
                        double v2n = v2x * nx + v2y * ny;

                        double v1nAfter = (v1n * (m1 - m2) + 2 * m2 * v2n) / (m1 + m2);
                        double v2nAfter = (v2n * (m2 - m1) + 2 * m1 * v1n) / (m1 + m2);

                        double dv1x = (v1nAfter - v1n) * nx;
                        double dv1y = (v1nAfter - v1n) * ny;
                        double dv2x = (v2nAfter - v2n) * nx;
                        double dv2y = (v2nAfter - v2n) * ny;

                        dataBall.SetVelocty(v1x + dv1x, v1y + dv1y);
                        other.dataBall.SetVelocty(v2x + dv2x, v2y + dv2y);

                        double overlap = minDistance - euclideanDistance;
                        if (overlap > 0)
                        {
                            double adjust = overlap * 0.5;
                            dataBall.Position.x += nx * adjust;
                            dataBall.Position.y += ny * adjust;
                            other.dataBall.Position.x -= nx * adjust;
                            other.dataBall.Position.y -= ny * adjust;
                        }
                    }

                }

            }
            
            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }


        #endregion private
    }
}