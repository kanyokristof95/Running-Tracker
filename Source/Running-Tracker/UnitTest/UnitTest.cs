using Microsoft.VisualStudio.TestTools.UnitTesting;
using Running_Tracker.Model;
using Running_Tracker.Persistence;
using Moq;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        #region Field

        private RunningTrackerModel model;

        private int value; // Test

        #endregion
        private Mock<IRunningTrackerDataAccess> _mock;
        private RunningTrackerModel _model;

        #region Initialize

        /// <summary>
        /// Initialize the model
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _mock = new Mock<IRunningTrackerDataAccess>();
            List<RunningData> testRunningData = new List<RunningData>();
            testRunningData.Add(new RunningData(new PersonalData()));
            testRunningData.Add(new RunningData(new PersonalData(Gender.Female, 150, 50)));
            _mock.Setup(mock => mock.LoadPreviousRunnings()).Returns(testRunningData);

            _model = new RunningTrackerModel(_mock.Object);
        }
        #endregion

        #region  Tests

        [TestMethod]
        public void DefaultPersonalDatas()
        {
            var runnings = _model.LoadPreviousRunnings();
            Assert.AreEqual(runnings[0].PersonalInformation.Sex, Gender.Male);
            Assert.AreEqual(runnings[0].PersonalInformation.Height, 170);
            Assert.AreEqual(runnings[0].PersonalInformation.Weight, 70);
        }


        [TestMethod]
        public void CustomPersonalDatas()
        {
            var runnings = _model.LoadPreviousRunnings();
            Assert.AreEqual(runnings[1].PersonalInformation.Sex, Gender.Female);
            Assert.AreEqual(runnings[1].PersonalInformation.Height, 150);
            Assert.AreEqual(runnings[1].PersonalInformation.Weight, 50);
        }



        #endregion
    }
}
