using System;
using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Types;
using System.Linq;
using NUnit.Framework;


namespace Tetsts
{
    [TestFixture]
    public class LineupTests
    {
        const string CSV_3_SNAPSHOTS =
                @"#1,#2,#3,#4,#5,Points,Points Against,Quarter,Time Left
                    1,2,3,4,5   ,0,0,1,10:00
                    1,2,3,4,6   ,1,0,2,10:00
                    1,2,3,4,7   ,1,2,3,10:00
                    1,2,3,4,8   ,10,3,4,0:00";


        private static LineUp GetBestLineups(int playerCount)
        {
            IList<GameSnapshot> snapshots = CsvImporter.GetGameSnapShots(CSV_3_SNAPSHOTS);
            var lineup = LineUp.GetBestLineup(playerCount, snapshots);            
            return lineup;
        }


        private static IList<GameSnapshot> GetSnapshots()
        {
            IList<GameSnapshot> snapshots = CsvImporter.GetGameSnapShots(CSV_3_SNAPSHOTS);
            return snapshots;
        }

        [Test(Description = "Best Line up")]
        public void BestLineup()
        {
            var lineup = LineupTests.GetBestLineups(5);

            var expected = new int[] { 1, 2, 3, 4, 7 };
            CollectionAssert.AreEquivalent(expected, lineup);
        }


        [Test(Description = "Best offensive Line up")]
        public void OffensiveLineup()
        {
            var snapshots = LineupTests.GetSnapshots();
            var lineup = LineUp.GetOffensiveLineup(snapshots, playerCount: 5);
            

            var expected = new int[] { 1, 2, 3, 4, 7 };
            CollectionAssert.AreEquivalent(expected, lineup);
        }


        [Test(Description = "Best deffensive Line up")]
        public void DeffensiveLineup()
        {
            var snapshots = LineupTests.GetSnapshots();
            var lineup = LineUp.GetDeffensiveLineup(snapshots, playerCount: 5, minTime:TimeSpan.FromMinutes(1));


            var expected = new int[] { 1, 2, 3, 4, 5 };
            CollectionAssert.AreEquivalent(expected, lineup);
        }


    }
       
}
