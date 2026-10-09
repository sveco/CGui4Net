using CGui.Gui.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CGui.Tests
{
  [TestClass]
  public class SizeChangeDetectorTests
  {
    [TestMethod]
    public void SameSize_NeverReports()
    {
      var detector = new SizeChangeDetector(120, 40);

      for (int i = 0; i < 10; i++)
      {
        Assert.IsFalse(detector.Poll(120, 40));
      }
    }

    [TestMethod]
    public void NewSize_IsReportedOnce_AfterItWasSeenTwiceInARow()
    {
      var detector = new SizeChangeDetector(120, 40);

      Assert.IsFalse(detector.Poll(100, 30), "first sight, may still be dragged");
      Assert.IsTrue(detector.Poll(100, 30), "settled");
      Assert.IsFalse(detector.Poll(100, 30), "already reported");
    }

    [TestMethod]
    public void SizeThatKeepsChanging_IsReportedOnlyWhenItSettles()
    {
      var detector = new SizeChangeDetector(120, 40);

      Assert.IsFalse(detector.Poll(119, 40));
      Assert.IsFalse(detector.Poll(118, 40));
      Assert.IsFalse(detector.Poll(117, 41));
      Assert.IsFalse(detector.Poll(116, 41));
      Assert.IsTrue(detector.Poll(116, 41));
    }

    [TestMethod]
    public void OnlyHeightChanged_IsReported()
    {
      var detector = new SizeChangeDetector(120, 40);

      detector.Poll(120, 25);

      Assert.IsTrue(detector.Poll(120, 25));
    }

    [TestMethod]
    public void BackToTheAppliedSizeBeforeSettling_IsNotReported()
    {
      var detector = new SizeChangeDetector(120, 40);

      Assert.IsFalse(detector.Poll(100, 30));
      Assert.IsFalse(detector.Poll(120, 40));
      Assert.IsFalse(detector.Poll(120, 40));
      Assert.IsFalse(detector.Poll(100, 30), "the half seen size must not count towards a later change");
      Assert.IsTrue(detector.Poll(100, 30));
    }

    [TestMethod]
    public void Accept_ASizeSetByTheApplication_IsNotReported()
    {
      var detector = new SizeChangeDetector(120, 40);
      detector.Poll(100, 30);

      detector.Accept(100, 30);

      Assert.IsFalse(detector.Poll(100, 30));
      Assert.IsFalse(detector.Poll(100, 30));
    }

    [TestMethod]
    public void OnePoll_ReportsImmediately()
    {
      var detector = new SizeChangeDetector(120, 40, stablePolls: 1);

      Assert.IsTrue(detector.Poll(100, 30));
    }
  }
}
