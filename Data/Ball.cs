//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
      Position = initialPosition;
      Velocity = initialVelocity;

        thread = new Thread(MoveLoop)
        {
            IsBackground = true
        };
        isRunning = true;
        thread.Start();
        }
        internal void Stop()
        {
            isRunning = false;
            thread.Join(); // Bezpieczne zakończenie wątku
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }
        private readonly Thread thread;
        private volatile bool isRunning;

        #endregion IBall

        #region private

        private Vector Position;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

        private void MoveLoop()
        {
            while (isRunning)
            {
                Move();
                Thread.Sleep(50); // Możesz też użyć TimeSpan jako stałej
            }
        }
        private void Move()
    {
      Position = new Vector(Position.x + Velocity.x, Position.y + Velocity.y);
      RaiseNewPositionChangeNotification();
    }

        #endregion private
    }
}