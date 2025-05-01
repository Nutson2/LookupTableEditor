namespace LookupTableEditor.Extentions.Tests
{
    [TestClass()]
    public class StringExtentionsTests
    {
        [TestMethod()]
        public void ParseAsCellsTest()
        {
            var t = "12\t32\t45\r\n" + "12\t32\t45\r\n" + "12\t32\t45\r\n" + "12\t32\t45\r\n";

            var cells = t.ParseAsCells().ToList();

            Assert.AreEqual(cells.Count, 12);
        }

        [TestMethod()]
        public void ParseAsCellsWithEmptyRowTest()
        {
            var t = "12\t32\t45\r\n" + "\r\n" + "12\t32\t45\r\n" + "12\t32\t45\r\n";

            var cells = t.ParseAsCells().ToList();

            Assert.AreEqual(cells.Count, 9);
        }

        [TestMethod()]
        public void ParseAsCellsWithVariousColumnTest()
        {
            var t = "12\t32\t45\r\n" + "12\t45\r\n" + "32\t45\r\n" + "12\t32\r\n";

            var cells = t.ParseAsCells().ToList();

            Assert.AreEqual(cells.Count, 9);
        }

        [TestMethod()]
        public void ParseAsCellsWithEmptyStringTest()
        {
            var t = string.Empty;

            var cells = t.ParseAsCells().ToList();

            Assert.AreEqual(cells.Count, 0);
        }
    }
}
