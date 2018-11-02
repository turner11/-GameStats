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
            var players = new List<PlayerData>();
            var snapShots = snapShotsCollection.ToList();
            if (snapShots.Count < 1)
                return players;
            players =
                snapShots.SelectMany(snp => snp.PlayerNumbers)
                .Distinct()
                .Select(pn => new PlayerData(pn)).ToList();

            var me = players.Where(p => p.Number == 11).FirstOrDefault();
            for (int i = 0; i < snapShots.Count; i++)
            {
                var snp = snapShots[i];
                GameSnapshot prevSnap = i > 0 ?
                                        snapShots[i - 1]
                                        : GameSnapshot.GetBeginingOfGameSnapshot();

                var currPlayers = players.Where(p => snp.PlayerNumbers.Contains(p.Number)).ToList();
                var subbedOutPlayers = players.Where(p => prevSnap.PlayerNumbers.Contains(p.Number))
                                              .Where(p => !currPlayers.Contains(p)).ToList();
                var playerInPrevSnap = players.Where(p => prevSnap.PlayerNumbers.Contains(p.Number)).ToList();


                
                foreach (var player in playerInPrevSnap)
                {
                    player.Rate += snp.ScoreDiff;
                }

                ////For end of game
                //if (snp.TotalTimeLeft.TotalMinutes == 0)
                //{
                //    foreach (var player in currPlayers)
                //    {
                //        {
                //            player.Rate += snp.ScoreDiff;
                //        }
                //    }
                //}
                var timeDiff = prevSnap.Elapsed;
                if (timeDiff.HasValue)
                {
                    var nonSubbedPlayers = snp.PlayerNumbers.Intersect(prevSnap.PlayerNumbers)
                                        .Select(n => players.Where(p => p.Number == n).FirstOrDefault())
                                        .ToList();
                    var playersToIncreasMinutes = nonSubbedPlayers.Union(subbedOutPlayers).Distinct().ToList();
                    foreach (var p in playersToIncreasMinutes)
                    {
                        p.Minutes += timeDiff.Value;
                    }
                }
            }

            snapShots.ToString();

            return players;
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
