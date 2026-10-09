using CGui.Gui;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CGui.Tests
{
  [TestClass]
  public class StringExtensionsTests
  {
    // colour tags as the console wrapper understands them
    private const string Red = "\x1b[f:Red]";
    private const string Reset = "\x1b[Reset]";

    [TestMethod]
    public void VisibleLength_IgnoresColourTags()
    {
      Assert.AreEqual(5, (Red + "Hello" + Reset).VisibleLength());
      Assert.AreEqual(5, "Hello".VisibleLength());
    }

    [TestMethod]
    public void TruncateVisible_ShortTextIsUnchanged()
    {
      Assert.AreEqual("Hello", "Hello".TruncateVisible(5));
      Assert.AreEqual("Hello", "Hello".TruncateVisible(50));
      Assert.AreEqual("", "".TruncateVisible(3));
      Assert.IsNull(((string)null).TruncateVisible(3));
    }

    [TestMethod]
    public void TruncateVisible_CutsLongText()
    {
      Assert.AreEqual("Hello", "Hello world".TruncateVisible(5));
      Assert.AreEqual("", "Hello".TruncateVisible(0));
    }

    [TestMethod]
    public void TruncateVisible_ColourTagsDoNotCountAndAreKept()
    {
      var result = (Red + "Hello" + Reset + " world").TruncateVisible(7);

      Assert.AreEqual(Red + "Hello" + Reset + " w", result);
    }

    [TestMethod]
    public void TruncateVisible_KeepsATrailingResetTag()
    {
      var result = (Red + "Hello world" + Reset).TruncateVisible(5);

      Assert.AreEqual(Red + "Hello" + Reset, result);
    }

    [TestMethod]
    public void PadRightVisible_PadsToTheVisibleWidth()
    {
      var result = (Red + "Hi" + Reset).PadRightVisible(6, '.');

      Assert.AreEqual(Red + "Hi" + Reset + "....", result);
      Assert.AreEqual(6, result.VisibleLength());
    }

    [TestMethod]
    public void PadLeftVisible_PadsToTheVisibleWidth()
    {
      var result = (Red + "Hi" + Reset).PadLeftVisible(6, '.');

      Assert.AreEqual("...." + Red + "Hi" + Reset, result);
      Assert.AreEqual(6, result.VisibleLength());
    }

    [TestMethod]
    public void PadBothVisible_CentresTheVisibleText()
    {
      var result = (Red + "Hi" + Reset).PadBothVisible(6, '.');

      Assert.AreEqual(6, result.VisibleLength());
      Assert.AreEqual("..Hi..", result.Replace(Red, "").Replace(Reset, ""));
    }

    [TestMethod]
    public void Padding_TextLongerThanTheWidthIsLeftAlone()
    {
      Assert.AreEqual("Hello", "Hello".PadRightVisible(3, '.'));
      Assert.AreEqual("Hello", "Hello".PadLeftVisible(3, '.'));
      Assert.AreEqual("Hello", "Hello".PadBothVisible(3, '.'));
    }

    [TestMethod]
    public void Split_WrapsAtTheChunkSize_AndAcceptsANarrowOne()
    {
      var lines = "one two three four five six".Split(10);

      foreach (var line in lines)
      {
        Assert.IsTrue(line.VisibleLength() <= 14, "'" + line + "' is too long");
      }
    }
  }
}
