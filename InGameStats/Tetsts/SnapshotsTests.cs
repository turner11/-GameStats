using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Types;
using System.Linq;

namespace Tetsts
{
    using Snapshots = GameSnapshot;
    [TestClass]
    public class SnapshotsTests
    {

        const string CSV_3_SNAPSHOTS =
                @"#1,#2,#3,#4,#5,Points,Points Against,Quarter,Time Left
                    1,2,3,4,5   ,0,0,1,10:00
                    1,2,3,4,6   ,1,0,2,10:00
                    1,2,3,4,7   ,2,3,4,0:00";

       
        private static (Snapshots sn1, Snapshots sn2, Snapshots sn3) Get3Snapshots()
        {
            List<Snapshots> snapshots = GetSnapshots();
            var sn1 = snapshots[0];
            var sn2 = snapshots[1];
            var sn3 = snapshots[2];
            return (sn1, sn2, sn3);
        }

        private static List<Snapshots> GetSnapshots()
        {
            return CsvImporter.GetGameSnapShots(CSV_3_SNAPSHOTS);
        }

        [TestMethod]
        [Description("Get Correct number of snapshots")]
        public void NumberOfSnapshots()
        {
            List<GameSnapshot> snapshots = GetSnapshots();

            Assert.AreEqual(3, snapshots.Count, "Got unexxpected number of snapshots");
        }

        //[TestMethod]
        //[Description("Get Correct Score diff of game")]
        //public void GameScoreDiff()
        //{
        //    (Snapshots sn1, Snapshots sn2, Snapshots sn3) = Get3Snapshots();

        //    Assert.AreEqual(sn1.GameScoreDiff, 0, "Got bad Game score diff value");
        //    Assert.AreEqual(sn2.GameScoreDiff, 1, "Got bad Game score diff value");
        //    Assert.AreEqual(sn3.GameScoreDiff, -1, "Got bad Game score diff value");
        //}

        [TestMethod]
        [Description("Get Player nunmbers")]
        public void PlayerNumbers()
        {
            (Snapshots sn1, Snapshots sn2, Snapshots sn3) = Get3Snapshots();

            Assert.IsTrue(sn1.PlayerNumbers.SequenceEqual(Enumerable.Range(1, 5)), "Got bad player numbers");
            Assert.IsTrue(sn3.PlayerNumbers.SequenceEqual(new int[] { 1, 2, 3, 4, 7 }), "Got bad player numbers");
        }



        [TestMethod]
        [Description("Get Correct Score diff of snapshot")]
        public void ScoreDiff()
        {
            (Snapshots sn1, Snapshots sn2, Snapshots sn3) = Get3Snapshots();


            Assert.AreEqual(sn1.ScoreDiff, 1, "Got bad Snapshot score diff value");
            Assert.AreEqual(sn2.ScoreDiff, -2, "Got bad Snapshot score diff value");
            Assert.AreEqual(sn3.ScoreDiff, 0, "Got bad Snapshot score diff value");
        }

        [TestMethod]
        [Description("Get Correct value of quarter for snapshots")]
        public void Quarter()
        {
            (Snapshots sn1, Snapshots sn2, Snapshots sn3) = Get3Snapshots();

            Assert.AreEqual(sn1.Quarter, 1, "Got bad Quarter");
            Assert.AreEqual(sn2.Quarter, 2, "Got bad Quarter");
            Assert.AreEqual(sn3.Quarter, 4, "Got bad Quarter");
        }



        [TestMethod]
        [Description("Get Correct Content of snapshots")]
        public void TeamScore()
        {
            (Snapshots sn1, Snapshots sn2, Snapshots sn3) = Get3Snapshots();

            Assert.AreEqual(sn1.TeamScore, 1, "Got bad TeamScore");
            Assert.AreEqual(sn2.TeamScore, 1, "Got bad TeamScore");
            Assert.AreEqual(sn3.TeamScore, 0, "Got bad TeamScore");
        }

        [TestMethod]
        [Description("Get Correct value for time left")]
        public void TimeLeft()
        {
            (Snapshots sn1, Snapshots sn2, Snapshots sn3) = Get3Snapshots();

            Assert.AreEqual(TimeSpan.FromMinutes(40), sn1.TotalTimeLeft,  "Got bad time left");
            Assert.AreEqual(TimeSpan.FromMinutes(30), sn2.TotalTimeLeft, "Got bad time left");
            Assert.AreEqual(0, sn3.TotalTimeLeft.TotalSeconds, "Got bad time left");
        }

       
    }
}
