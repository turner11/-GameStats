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
        public virtual int TeamScore { get; }
        public virtual int OponentScore { get; }


        

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
       
    }
}
