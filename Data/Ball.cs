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
using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        private readonly object velocityLock = new();
        private readonly object positionLock = new();
        private IVector velocity { get; set; }
        private IVector position { get; set; }
        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            position = initialPosition;
            velocity = initialVelocity;
            Diameter = 20;
            Mass = 1;

            thread = new Thread(MoveLoop)
            {
                IsBackground = true
            };
            
        }

        internal void Stop()
        {
            isRunning = false;
            thread.Join(); // Bezpieczne zakończenie wątku
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;


        public IVector Velocity {
            get
            {
                lock (velocityLock)
                {
                    return velocity;
                }
            }
            set
            {
                lock (velocityLock)
                {
                    velocity = (Vector)value;
                }

            }
        }

        public IVector Position
        {
            get
            {
                lock (positionLock)
                {
                    return position;
                }
            }
        }

        public double Diameter { get; }
        public double Mass { get; }
        

        public void Start()
        {
            if (!isRunning)
            {
                isRunning = true;
                thread.Start();
            }
           
        }

        #endregion IBall

        #region private
        private readonly Thread thread;
        private volatile bool isRunning;
        private int refreshTime;
        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        private void MoveLoop()
        {
            const int maxRefreshTime = 100;
            const int minRefreshTime = 10;

            while (isRunning)
            {
                Move();

                double actualVelocity = Math.Sqrt(Velocity.x * Velocity.x + Velocity.y * Velocity.y);
                double normalizedVelocity = Math.Clamp(actualVelocity, 0.0, 1.0);
                refreshTime = Math.Clamp(
                  (int)(maxRefreshTime - normalizedVelocity * (maxRefreshTime - minRefreshTime)),
                  minRefreshTime,
                  maxRefreshTime);

                Thread.Sleep(refreshTime);
            }
        }

        private void Move()
        {
            lock (positionLock)
            {
                position = new Vector(Position.x + Velocity.x * refreshTime / 1000, Position.y + Velocity.y * refreshTime / 1000);
            }
            
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}
