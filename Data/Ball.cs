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

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            long last = stopwatch.ElapsedMilliseconds;
            while (isRunning)
            {
                long current = stopwatch.ElapsedMilliseconds;
                int dtime = (int)(current - last);
                last = current;
                Move(dtime);
                //Thread.Sleep(5);
            }
            stopwatch.Stop();
        }

        private void Move(int dtime)
        {
            lock (positionLock)
            {
                position = new Vector(Position.x + Velocity.x * dtime / 1000, Position.y + Velocity.y * dtime / 1000);
            }
            
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}
