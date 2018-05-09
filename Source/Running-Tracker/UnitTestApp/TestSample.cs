using System;
using NUnit.Framework;
using Running_Tracker.Model;
using Running_Tracker.Persistence;

using System.Collections.Generic;
using Android.Locations;
using Android.Content;
using Moq;

namespace UnitTestApp
{
    [TestFixture]
    public class TestsSample
    {
        private Mock<IRunningTrackerDataAccess> _mock;
        private RunningTrackerModel _model;
        private LocationManager _locationManager;


        private bool gpsready = false;
        private bool distancewarning = false;
        private bool fastwarning = false;
        private bool slowwarning = false;
        private bool timewarning = false;
        private bool userstopped = false;


        [SetUp]
        public void Setup()
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

            WarningValues warningValues = new WarningValues();

            _mock.SetupGet(mock => mock.CurrentWarningValues).Returns(warningValues);

            _model = new RunningTrackerModel(_mock.Object);


            _model.GpsReady += Model_GPS_Ready;
            _model.Warning += Model_Warning;
            _model.UserStopped += Model_UserStopped;
        }


        [Test]
        public void DefaultPersonalDatas()
        {
            var runnings = _model.LoadPreviousRunnings();
            Assert.AreEqual(runnings[0].PersonalInformation.Sex, Gender.Male);
            Assert.AreEqual(runnings[0].PersonalInformation.Height, 170);
            Assert.AreEqual(runnings[0].PersonalInformation.Weight, 70);

            Assert.AreEqual(runnings[0].Distance, 0);
            Assert.IsTrue(runnings[0].Open);

        }


        [Test]
        public void CustomPersonalDatas()
        {
            var runnings = _model.LoadPreviousRunnings();
            Assert.AreEqual(runnings[1].PersonalInformation.Sex, Gender.Female);
            Assert.AreEqual(runnings[1].PersonalInformation.Height, 150);
            Assert.AreEqual(runnings[1].PersonalInformation.Weight, 50);

            Assert.AreEqual(runnings[1].Distance, 0);
            Assert.IsTrue(runnings[1].Open);
        }

        [Test]
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

        [Test]
        [ExpectedException(typeof(GpsNotReadyException))]
        public void StartRunWithoutCalbrationTest()
        {
            _model.StartRunning();

        }

        [Test]
        [ExpectedException(typeof(GpsNotReadyException))]
        public void GpsNotCalibratingIfAccuracyMoreThanSix()
        {
            Location location = new Location(LocationManager.GpsProvider)
            {
                Latitude = 51.20958,
                Longitude = 2.92372,
                Accuracy = 7.0f,
                Time = DateTime.Now.Ticks,
                Altitude = 1.0,
                Speed = 0.0f,
                Bearing = 0.0f,
                Provider = LocationManager.GpsProvider,
            };

            _model.Calibrate();

            for (int i = 0; i < 10; i++)
            {
                _model.ChangeLocation(location);
            }

            _model.StartRunning();

        }

        [Test]
        public void GPSCalibratingIfAccuracyLessThanSix()
        {
            gpsready = false;
            Location location = new Location(LocationManager.GpsProvider)
            {
                Latitude = 51.20958,
                Longitude = 2.92372,
                Accuracy = 5.0f,
                Time = DateTime.Now.Ticks,
                Altitude = 1.0,
                Speed = 0.0f,
                Bearing = 0.0f,
                Provider = LocationManager.GpsProvider,
            };

            _model.Calibrate();

            for (int i =0; i<=5; ++i)
            {
                _model.ChangeLocation(location);
            }
            _model.ChangeLocation(location);


            Assert.IsTrue(gpsready);
        }


        [Test]
        public void UserStoppedTest()
        {
            Assert.IsFalse(userstopped);
            Location location = new Location(LocationManager.GpsProvider)
            {
                Latitude = 51.20958,
                Longitude = 2.92372,
                Accuracy = 5.0f,
                Time = DateTime.Now.Ticks,
                Altitude = 1.0,
                Speed = 0.0f,
                Bearing = 0.0f,
                Provider = LocationManager.GpsProvider,
            };

            _model.Calibrate();

            for (int i = 0; i <= 7; ++i)
            {
                _model.ChangeLocation(location);
            }

            _model.StartRunning();
            for (int i = 0; i < 10; i++)
            {
                float lat = 51.20958f + (i * 0.0001f);
                Location location2 = new Location(LocationManager.GpsProvider)
                {
                    Latitude = lat,
                    Longitude = 2.92372,
                    Accuracy = 5.0f,
                    Time = DateTime.Now.Ticks,
                    Altitude = 1.0,
                    Speed = 5.0f,
                    Bearing = 0.0f,
                    Provider = LocationManager.GpsProvider,
                };
                _model.ChangeLocation(location2);
            }

            Location location3 = new Location(LocationManager.GpsProvider)
            {
                Latitude = 51.21f,
                Longitude = 2.92372,
                Accuracy = 5.0f,
                Time = DateTime.Now.Ticks,
                Altitude = 1.0,
                Speed = 0.0f,
                Bearing = 0.0f,
                Provider = LocationManager.GpsProvider,
            };

            for (int i = 0; i <= 10; ++i)
            {
                _model.ChangeLocation(location3);
            }

            Assert.IsTrue(userstopped);
        }


        [Test]
        public void FastWarningTest()
        {
            Assert.IsFalse(fastwarning);
            Location location = new Location(LocationManager.GpsProvider)
            {
                Latitude = 51.20958,
                Longitude = 2.92372,
                Accuracy = 5.0f,
                Time = DateTime.Now.Ticks,
                Altitude = 1.0,
                Speed = 0.0f,
                Bearing = 0.0f,
                Provider = LocationManager.GpsProvider,
            };

            _model.Calibrate();

            for (int i = 0; i <= 7; ++i)
            {
                _model.ChangeLocation(location);
            }

            _model.StartRunning();
            for (int i = 0; i < 10; i++)
            {
                float lat = 51.20958f + (i * 0.0001f);
                Location location2 = new Location(LocationManager.GpsProvider)
                {
                    Latitude = lat,
                    Longitude = 2.92372,
                    Accuracy = 5.0f,
                    Time = DateTime.Now.Ticks,
                    Altitude = 1.0,
                    Speed = 20.0f,
                    Bearing = 0.0f,
                    Provider = LocationManager.GpsProvider,
                };
                _model.ChangeLocation(location2);
            }

            Assert.IsTrue(fastwarning);
        }

        [Test]
        public void SlowWarningTest()
        {
            Assert.IsFalse(slowwarning);
            Location location = new Location(LocationManager.GpsProvider)
            {
                Latitude = 51.20958,
                Longitude = 2.92372,
                Accuracy = 5.0f,
                Time = DateTime.Now.Ticks,
                Altitude = 1.0,
                Speed = 0.0f,
                Bearing = 0.0f,
                Provider = LocationManager.GpsProvider,
            };

            _model.Calibrate();

            for (int i = 0; i <= 7; ++i)
            {
                _model.ChangeLocation(location);
            }

            _model.StartRunning();
            for (int i = 0; i < 10; i++)
            {
                float lat = 51.20958f + (i * 0.000001f);
                Location location2 = new Location(LocationManager.GpsProvider)
                {
                    Latitude = lat,
                    Longitude = 2.92372,
                    Accuracy = 5.0f,
                    Time = DateTime.Now.Ticks,
                    Altitude = 1.0,
                    Speed = 0.5f,
                    Bearing = 0.0f,
                    Provider = LocationManager.GpsProvider,
                };
                _model.ChangeLocation(location2);
            }

            Assert.IsTrue(slowwarning);
        }


        private void Model_GPS_Ready(object sender, PositionArgs e)
        {
            gpsready = true;
        }

        private void Model_Warning(object sender, WarningArgs e)
        {

            switch (e.Warning)
            {
                case WarningType.Distance:
                    distancewarning = true;
                    break;
                case WarningType.SpeedFast:
                    fastwarning = true;
                    break;
                case WarningType.SpeedSlow:
                    slowwarning = true;
                    break;
                case WarningType.Time:
                    timewarning = true;
                    break;
            }
        }

        private void Model_UserStopped(object sender, PositionArgs e)
        {
            userstopped = true;
        }

    }
}