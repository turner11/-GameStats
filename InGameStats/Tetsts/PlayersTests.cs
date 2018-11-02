using System;
using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Types;
using System.Linq;
using NUnit.Framework;

namespace Tetsts
{
    

    [TestFixture]
    public class PlayersTests
    {
        const string CSV_3_SNAPSHOTS =
                @"#1,#2,#3,#4,#5,Points,Points Against,Quarter,Time Left
                    1,2,3,4,5   ,0,0,1,10:00
                    1,2,3,4,6   ,1,0,2,10:00
                    1,2,3,4,7   ,1,2,4,0:00";

        private static IList<PlayerData> GetPlayers()
        {
            var snapshots = CsvImporter.GetGameSnapShots(CSV_3_SNAPSHOTS);
            var players = PlayerData.GetData(snapshots);
            
            return players;
        }       


        [Test(Description = "Get Correct number of players")]
        public void NumberOfPlayers()
        {
            var players = PlayersTests.GetPlayers();
            Assert.AreEqual(7, players.Count);
        }


        
        [Description("Get Correct number of players")]
        [TestCase(1,-1,Description ="Player 1 Rate test")]
        [TestCase(2, -1, Description = "Player 2 Rate test")]
        [TestCase(3, -1, Description = "Player 3 Rate test")]
        [TestCase(4, -1, Description = "Player 4 Rate test")]
        [TestCase(5, 1, Description = "Player 5 Rate test")]
        [TestCase(6, -2, Description = "Player 6 Rate test")]
        [TestCase(7, 0, Description = "Player 7 Rate test")]
        public void PlayerRate(int playerNumber, int expectedRate)
        {
            var players = PlayersTests.GetPlayers();
            var player = players.FirstOrDefault(p => p.Number == playerNumber);
            Assert.AreEqual(expectedRate, player.Rate, $"Got wrong rate for player {playerNumber}");
        }
    }
        
}
