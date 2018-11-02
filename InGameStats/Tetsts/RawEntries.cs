using System;
using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Types;
using System.Linq;
using NUnit.Framework;


namespace Tetsts
{
    

    [TestFixture]
    public class EntryiesTests
    {
        const string CSV_3_SNAPSHOTS =
                @"#1,#2,#3,#4,#5,Points,Points Against,Quarter,Time Left
                    1,2,3,4,5   ,0,0,1,10:00
                    1,2,3,4,6   ,1,0,2,10:00
                    1,2,3,4,7   ,1,2,4,0:00";


        private static (RawEntry e1, RawEntry e2, RawEntry e3) GetEntries()
        {
            IList<RawEntry> snapshots = CsvImporter.GetEntries(CSV_3_SNAPSHOTS);
            var e1 = snapshots[0];
            var e2 = snapshots[1];
            var e3 = snapshots[2];
            return (e1, e2, e3);
        }
      


        
        [Test(Description="Get Correct Scores")]
        public void ScoreDiff()
        {
            (RawEntry e1, RawEntry e2, RawEntry e3) = GetEntries();

            

            Assert.AreEqual(e1.TeamScore, 0, "Got bad Snapshot score diff value");
            Assert.AreEqual(e2.TeamScore, 1, "Got bad Snapshot score diff value");
            Assert.AreEqual(e3.OponentScore, 2, "Got bad Snapshot score diff value");
        }


        [Test(Description = "Get Correct number of Entries")]
        public void NumberOfPlayers()
        {
            var entries = CsvImporter.GetEntries(CSV_3_SNAPSHOTS);
            Assert.AreEqual(3, entries.Count);
        }


        
        [Description("Get Correct number of players occurances")]
        [TestCase(1,3,Description ="Player 1 Occurances test")]
        [TestCase(2, 3, Description = "Player 2 Occurances test")]
        [TestCase(3, 3, Description = "Player 3 Occurances test")]
        [TestCase(4, 3, Description = "Player 4 Occurances test")]
        [TestCase(5, 1, Description = "Player 5 Occurances test")]
        [TestCase(6, 1, Description = "Player 6 Occurances test")]
        [TestCase(7, 1, Description = "Player 7 Occurances test")]
        [TestCase(8, 0, Description = "Player 8 Occurances test")]
        [TestCase(-1, 0, Description = "Player -1 Occurances test")]
        public void PlayerRate(int playerNumber, int expectedOccurances)
        {
            (RawEntry e1, RawEntry e2, RawEntry e3) = EntryiesTests.GetEntries();
            var entries = new List<RawEntry> { e1, e2, e3 };
            var occurances = entries.SelectMany(e => e.PlayerNumbers).Count(n=> n==playerNumber);
            Assert.AreEqual(occurances, expectedOccurances, $"Got wrong occurances for player {playerNumber}");
        }
    }
        
}
