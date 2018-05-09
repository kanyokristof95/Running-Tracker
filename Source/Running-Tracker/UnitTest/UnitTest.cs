using Microsoft.VisualStudio.TestTools.UnitTesting;
using Running_Tracker.Model;
using Running_Tracker.Persistence;
using Moq;
using System.Collections.Generic;
using Android.Locations;
using System;


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

            RunningData testData = new RunningData(new PersonalData());
            testData.AddLocation(new LocationData(20, 10, 0, 100, 50, 40));
            testData.AddLocation(new LocationData(100, 110, 0, 100, 50, 40));
            testRunningData.Add(testData);

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

            Assert.AreEqual(runnings[0].Distance, 0);
            Assert.IsTrue(runnings[0].Open);

        }


        [TestMethod]
        public void CustomPersonalDatas()
        {
            var runnings = _model.LoadPreviousRunnings();
            Assert.AreEqual(runnings[1].PersonalInformation.Sex, Gender.Female);
            Assert.AreEqual(runnings[1].PersonalInformation.Height, 150);
            Assert.AreEqual(runnings[1].PersonalInformation.Weight, 50);

            Assert.AreEqual(runnings[1].Distance, 0);
            Assert.IsTrue(runnings[1].Open);
        }

        [TestMethod]
        public void LocationTest()
        {
            var runnings = _model.LoadPreviousRunnings();
            Assert.AreEqual(runnings[2].Distance, 200);
            Assert.AreEqual(runnings[2].Up, 100);
            Assert.AreEqual(runnings[2].Down, 80);
            Assert.AreEqual(runnings[2].MinLatitude, 10);
            Assert.AreEqual(runnings[2].MaxLatitude, 110);
            Assert.AreEqual(runnings[2].MinLongitude, 20);
            Assert.AreEqual(runnings[2].MaxLongitude, 100);
            Assert.IsTrue(runnings[2].Open);
        }

        [TestMethod]
        [ExpectedException(typeof(GpsNotReadyException))]
        public void StartRunWithoutCalbrationTest()
        {
            _model.StartRunning();

        }

        [TestMethod]
        public void RunningData()
        {
            var runnings = _model.LoadPreviousRunnings();
            Assert.AreEqual(runnings[2].Distance, 200);
            Assert.AreEqual(runnings[2].Up, 100);
            Assert.AreEqual(runnings[2].Down, 80);
            Assert.AreEqual(runnings[2].MinLatitude, 10);
            Assert.AreEqual(runnings[2].MaxLatitude, 110);
            Assert.AreEqual(runnings[2].MinLongitude, 20);
            Assert.AreEqual(runnings[2].MaxLongitude, 100);
            Assert.IsTrue(runnings[2].Open);
        }


        #endregion
    }
}
