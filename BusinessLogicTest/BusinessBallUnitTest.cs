//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void MoveTestMethod()
    {
      DataBallFixture dataBallFixture = new DataBallFixture();
      Ball newInstance = new(dataBallFixture, new List<Data.IBall> { });
      int numberOfCallBackCalled = 0;
      
      
    }

    #region testing instrumentation

    private class DataBallFixture : Data.IBall
    {
      public Data.IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public double Diameter { get => throw new NotImplementedException(); }
            public double Mass { get => throw new NotImplementedException(); }

      public event EventHandler<Data.IVector>? NewPositionNotification;

            public Data.IVector getPos()
            {
                throw new NotImplementedException();
            }

            public void setPos(double x, double y)
            {
                throw new NotImplementedException();
            }

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

    #endregion testing instrumentation
  }
}