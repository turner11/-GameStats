using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Types
{
    public class RawEntry: CoreRecored
    {
        public ReadOnlyCollection<int> PlayerNumbers { get; }
        public int Quarter { get; }
        public TimeSpan TimeLeft { get; }
        public int TeamScore { get; }
        public int OponentScore { get; }


        public TimeSpan TotalTimeLeft
        {
            get
            {
                return GetTotalTimeLeft(this.Quarter, TimeLeft);
            }
        }

        public RawEntry(IList<int> playerNumber, int quarter, TimeSpan timeLeft, int teamScore, int oponentScore)
        {
            this.PlayerNumbers = (playerNumber ?? new int[0]).ToList().AsReadOnly();
            this.Quarter = quarter;
            this.TimeLeft = timeLeft;
            this.TeamScore = teamScore;
            this.OponentScore = oponentScore;
        }

        public override string ToString()
        {
            return $"Quarter:{this.Quarter} {this.TimeLeft}; {this.TeamScore}:{this.OponentScore}; ({String.Join(",", this.PlayerNumbers)})";
        }

        protected static TimeSpan GetTotalTimeLeft(int quarter, TimeSpan TimeLeftInQuarter)
        {
            var minutes = (4 - quarter) * 10.0 + TimeLeftInQuarter.TotalMinutes;
            return TimeSpan.FromMinutes(minutes);
        }
    }
}
