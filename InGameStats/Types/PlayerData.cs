using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;


namespace Types
{
    public class PlayerData
    {
        public int Number { get; }
        public TimeSpan Minutes { get; private set; }
        public int Rate { get; private set; }

        public PlayerData(int number)
        {
            this.Number = number;
        }


        public static IList<PlayerData> GetData(IEnumerable<GameSnapshot> snapShotsCollection)
        {
            var snapShots = snapShotsCollection.ToList();
            var playerNumber = snapShots.SelectMany(sn => sn.PlayerNumbers).Distinct().OrderBy(n => n).ToList();
            List<PlayerData> players = playerNumber.Select(n => PlayerData.FromSnapShots(n, snapShots)).ToList();
            players = players.Where(p => p != null).ToList();
            return players;


        }

        private static PlayerData FromSnapShots(int playerNumber, List<GameSnapshot> snapShots)
        {
            if (snapShots.Count == 0)
                return null;
            var player = new PlayerData(playerNumber);
            var relevantSnapshots = snapShots.Where(sn => sn.PlayerNumbers.Contains(player.Number)).ToList();

            for (int i = 0; i < relevantSnapshots.Count; i++)
            {
                var snp = relevantSnapshots[i];
                player.Rate += snp.ScoreDiff;
                player.Minutes += snp.Elapsed ?? TimeSpan.FromSeconds(0);
            }


            return player;

        }

        public override bool Equals(object obj)
        {
            return obj != null && this.Number == (obj as PlayerData).Number;
        }
        public override int GetHashCode()
        {
            return this.Number;
        }
        public override string ToString()
        {
            return $"#{this.Number}; Minutes: {this.Minutes}; Rate: {this.Rate};";
        }
        

    }
}
