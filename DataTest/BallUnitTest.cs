//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      Vector testinVector = new Vector(0.0, 0.0);
      Ball newInstance = new(testinVector, testinVector);
    }

    [TestMethod]
    public void Move_WithZeroVelocity_ShouldNotChangePosition()
    {
        Vector initialPosition = new(10.0, 10.0);
        Ball ball = new(initialPosition, new Vector(0.0, 0.0));
        
        IVector currentPosition = initialPosition;
        ball.NewPositionNotification += (sender, position) => currentPosition = position;
        
        ball.Move(720, 400, 20);
        
        Assert.AreEqual(initialPosition, currentPosition);
    }

    [TestMethod]
    public void Move_WithVelocity_ShouldChangePosition()
    {
        Vector initialPosition = new(10.0, 10.0);
        Vector velocity = new(5.0, 5.0);
        Ball ball = new(initialPosition, velocity);
        
        IVector currentPosition = initialPosition;
        ball.NewPositionNotification += (sender, position) => currentPosition = position;
        
        ball.Move(720, 400, 20);
        
        Assert.AreEqual(new Vector(15.0, 15.0), currentPosition);
    }

    [TestMethod]
    public void Move_WithRightWallCollision_ShouldReverseXVelocity()
    {
        double width = 100;
        double diameter = 20;
        Vector initialPosition = new(width - diameter - 1, 50);
        Vector velocity = new(5.0, 0.0);
        Ball ball = new(initialPosition, velocity);
        
        ball.Move(width, 200, diameter);
        
        Assert.AreEqual(-5.0, ball.Velocity.x);
    }

    [TestMethod]
    public void Move_WithBottomWallCollision_ShouldReverseYVelocity()
    {
        double height = 100;
        double diameter = 20;
        Vector initialPosition = new(50, height - diameter - 1);
        Vector velocity = new(0.0, 5.0);
        Ball ball = new(initialPosition, velocity);
        
        ball.Move(200, height, diameter);
        
        Assert.AreEqual(-5.0, ball.Velocity.y);
    }
  }
}