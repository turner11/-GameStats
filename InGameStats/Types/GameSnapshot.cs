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
        public int Quarter { get; }
        public TimeSpan TimeLeft { get; }
        public TimeSpan TotalTimeLeft
        {
            get
            {
                return GetTotalTimeLeft(this.Quarter, TimeLeft);
            }
        }

        internal static TimeSpan GetTotalTimeLeft(int quarter, TimeSpan TimeLeftInQuarter)
        {
            var minutes = (4 - quarter) * 10.0 + TimeLeftInQuarter.TotalMinutes;
            return TimeSpan.FromMinutes(minutes);
        }

        public int TeamScore { get; }
        public int OponentScore { get; }

        public int ScoreDiff
        {
            get { return this.SnapshotScoreDiff ?? 0; }
        }


        public int GameScoreDiff
        {
            get { return this.TeamScore - this.OponentScore; }
        }


        public TimeSpan? Elapsed { get; }

        public GameSnapshot(IList<int> playerNumber, int quarter, TimeSpan timeLeft, int teamScore, int oponentScore, TimeSpan? elapsed, int? snapshotScoreDiff = null)
        {
            this.PlayerNumbers = (playerNumber ?? new int[0]).ToList().AsReadOnly();
            this.Quarter = quarter;
            this.TimeLeft = timeLeft;
            this.TeamScore = teamScore;
            this.OponentScore = oponentScore;
            this.Elapsed = elapsed;
            this.SnapshotScoreDiff = snapshotScoreDiff;
        }

        public static GameSnapshot GetBeginingOfGameSnapshot()
        {
            return new GameSnapshot(new int[0], 1, TimeSpan.FromMinutes(10), 0, 0, TimeSpan.FromSeconds(0));
        }
        public override string ToString()
        {
            return $"Quarter:{this.Quarter} {this.TimeLeft}; {this.TeamScore}:{this.OponentScore}; ({String.Join(",", this.PlayerNumbers)})";
        }
    }
}
