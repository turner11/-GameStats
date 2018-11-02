using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Types
{
    public class GameSnapshot:RawEntry
    {
        private int? SnapshotScoreDiff { get; }
        
        public int ScoreDiff
        {
            get { return this.SnapshotScoreDiff ?? 0; }
        }


        public int GameScoreDiff
        {
            get { return this.TeamScore - this.OponentScore; }
        }


        public TimeSpan? Elapsed { get; }

        private GameSnapshot(IList<int> playerNumber, int quarter, TimeSpan timeLeft, int teamScore, int oponentScore, TimeSpan? elapsed, int snapshotScoreDiff)
            :base(playerNumber, quarter, timeLeft, teamScore, oponentScore)
        {
            
            this.Elapsed = elapsed;
            this.SnapshotScoreDiff = snapshotScoreDiff;
        }

        public static List<GameSnapshot> Factory(IList<RawEntry> entries)
        {
            var snapshots = new List<GameSnapshot>();
            if (entries.Count == 0)
                return snapshots;


            var expandedEntries = entries.Select(e => e).ToList();
            expandedEntries.Add(expandedEntries.LastOrDefault()); //For making the last one count as if no change...

            for (int i = 1; i < expandedEntries.Count; i++)
            {
                var entry = expandedEntries[i-1];
                var nextEntry = expandedEntries[i];
                TimeSpan? elapsed = entry.TotalTimeLeft - nextEntry.TotalTimeLeft;


                var nextDiff = nextEntry.TeamScore - nextEntry.OponentScore;
                var currDiff = entry.TeamScore - entry.OponentScore;
                int snapshotScoreDiff = nextDiff - currDiff ;

                var snapshot = new GameSnapshot(entry.PlayerNumbers, entry.Quarter, entry.TimeLeft, entry.TeamScore, entry.OponentScore,
                                elapsed, snapshotScoreDiff: snapshotScoreDiff);
                
                snapshots.Add(snapshot);
            }

            //RawEntry lastEntry = entries.LastOrDefault();
            //var lastSnashot = new GameSnapshot(lastEntry.PlayerNumbers,
            //                                    lastEntry.Quarter,
            //                                    lastEntry.TimeLeft,
            //                                    lastEntry.TeamScore,
            //                                    lastEntry.OponentScore,
            //                                    TimeSpan.FromSeconds(0),
            //                                    snapshotScoreDiff: 0);

            //snapshots.Add(lastSnashot);
            return snapshots;
        }

        public static GameSnapshot GetBeginingOfGameSnapshot()
        {
            return new GameSnapshot(new int[0], 1, TimeSpan.FromMinutes(10), 0, 0, TimeSpan.FromSeconds(0),0);
        }



        public override string ToString()
        {
            return $"Quarter:{this.Quarter} {this.TimeLeft}; {this.TeamScore}:{this.OponentScore}; ({String.Join(",", this.PlayerNumbers)})";
        }
    }
}
