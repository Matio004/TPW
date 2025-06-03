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
namespace TP.ConcurrentProgramming.BusinessLogic.Test

{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void MoveTestMethod()
    {
            LoggerFixture logger = new LoggerFixture();
            DataBallFixture dataBallFixture = new DataBallFixture();
            object BallLock = new();
            Ball newInstance = new(dataBallFixture, new List<Ball> { }, BallLock, logger);
    }

    #region testing instrumentation

    private class DataBallFixture : Data.IBall
    {
      public Data.IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public Data.IVector Position { get => throw new NotImplementedException(); }
            public double Diameter { get => throw new NotImplementedException(); }
            public double Mass { get => throw new NotImplementedException(); }

      public event EventHandler<Data.IVector>? NewPositionNotification;

            public void Start() { throw new NotImplementedException(); }


            internal void Move()
      {
        NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
      }
    }

    private class VectorFixture : Data.IVector
    {
      internal VectorFixture(double X, double Y)
      {
        x = X; y = Y;
      }

      public double x { get; set; }
      public double y { get; set; }
    }

     private class LoggerFixture : ILogger
     {
         public void Dispose()
         {
            
         }

         public void Log(string message, int threadId, Data.IVector position, Data.IVector velocity)
         {
                
         }
         public void Stop()
         {
           
         }
     }

    #endregion testing instrumentation
  }
}