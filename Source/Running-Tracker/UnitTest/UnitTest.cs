using Microsoft.VisualStudio.TestTools.UnitTesting;
using Running_Tracker.Model;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        #region Field

        private RunningTrackerModel model;

        private int value; // Test

        #endregion


        #region Initialize

        /// <summary>
        /// Initialize the model
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            model = new RunningTrackerModel(null);

            value = 0; // Test
        }

        #endregion


        #region  Tests

        [TestMethod]
        public void TestMethod1()
        {
            value++; // 1
            Assert.AreEqual(value, 1);
        }

        [TestMethod]
        public void TestMethod2()
        {
            value++; // 1
            Assert.AreEqual(value, 1);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Assert.Fail();
        }

        #endregion
    }
}
