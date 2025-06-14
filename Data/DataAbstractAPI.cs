﻿//____________________________________________________________________________________________________________________________________
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
  public abstract class DataAbstractAPI : IDisposable
  {
    #region Layer Factory

    public static DataAbstractAPI GetDataLayer()
    {
      return modelInstance.Value;
    }

    #endregion Layer Factory

    #region public API

    public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler, ILogger logger);

    #endregion public API

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #region private

    private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

    #endregion private
  }

  public interface IVector
  {
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    double x { get; set; }

    /// <summary>
    /// The y component of the vector.
    /// </summary>
    double y { get; set; }
  }

  public interface IBall
  {
    event EventHandler<IVector> NewPositionNotification;

     IVector  Velocity { get; set; }

        IVector Position { get; }
    public void SetVelocty(double x, double y)
        {
            Velocity = new Vector(x, y);
        }
        public void Start();
        public int getId();
  }
    public interface ILogger : IDisposable
    {
        void Log(string message, int threadId, IVector position, IVector velocity);
        static ILogger CreateDefaultLogger() => Logger.LoggerInstance;
    }
}