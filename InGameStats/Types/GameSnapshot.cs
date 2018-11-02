using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Types
{
    public class GameSnapshot
    {
        private int? SnapshotScoreDiff { get; }
        public ReadOnlyCollection<int> PlayerNumbers { get; }

        public TimeSpan TotalTimeLeft
        {
            get
            {
                return GetTotalTimeLeft(this.Quarter, TimeLeft);
            }
        }
        

        public int ScoreDiff
        {
            get { return this.SnapshotScoreDiff ?? 0; }
        }
        
        
        public TimeSpan? Elapsed { get; }
        
        public int Quarter { get; }
        public TimeSpan TimeLeft { get; }
        public int TeamScore { get; }
        public int OponentScore { get; }

        private GameSnapshot(IList<int> playerNumber, 
                            int quarter, 
                            TimeSpan timeLeft, 
                            int teamScore, 
                            int oponentScore,
                            TimeSpan? elapsed, 
                            int snapshotScoreDiff)
            
        {
            
            this.PlayerNumbers = playerNumber.ToList().AsReadOnly();
            this.SnapshotScoreDiff = snapshotScoreDiff;
            this.Elapsed = elapsed;
            this.Quarter = quarter;
            this.TimeLeft = timeLeft;
            this.TeamScore = teamScore;
            this.OponentScore = oponentScore;
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

                var currTimeLeft = GetTotalTimeLeft(entry.Quarter, entry.TimeLeft);
                var nextTimeLeft = GetTotalTimeLeft(nextEntry.Quarter, nextEntry.TimeLeft);
                TimeSpan? elapsed = currTimeLeft  - nextTimeLeft;


                var nextDiff = nextEntry.TeamScore - nextEntry.OponentScore;
                var currDiff = entry.TeamScore - entry.OponentScore;
                int snapshotScoreDiff = nextDiff - currDiff ;


                var currScore = nextEntry.TeamScore - entry.TeamScore;
                var currOponentScore = nextEntry.OponentScore - entry.OponentScore;

                var snapshot = new GameSnapshot(entry.PlayerNumbers, 
                                                entry.Quarter, 
                                                entry.TimeLeft,
                                                currScore,
                                                currOponentScore,
                                                elapsed, 
                                                snapshotScoreDiff: snapshotScoreDiff);
                
                snapshots.Add(snapshot);
            }

          
            return snapshots;
        }

     

        public override string ToString()
        {
            return $"Quarter:{this.Quarter} {this.TimeLeft}; {this.TeamScore}:{this.OponentScore}; ({String.Join(",", this.PlayerNumbers)})";
        }

        public static TimeSpan GetTotalTimeLeft(int quarter, TimeSpan TimeLeftInQuarter)
        {
            var minutes = (4 - quarter) * 10.0 + TimeLeftInQuarter.TotalMinutes;
            return TimeSpan.FromMinutes(minutes);
        }
    }
}
