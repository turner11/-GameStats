using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;

namespace ViewModels
{
    public class LineUpViewModel: ViewModelBase, IComparable<LineUpViewModel>, IComparable
    {
        public IReadOnlyCollection<PlayerViewModel> Players { get; }
        public IReadOnlyCollection<PlayerData.GameSnapshot> SnapShots { get; }
        public TimeSpan Minutes { get; }
        public int Rate { get; }
        public int TeamScore { get; }
        public int OponentScore { get; }

        public double RatePerMinute { get; }

        public LineUpViewModel(IEnumerable<PlayerViewModel> players, IEnumerable<PlayerData.GameSnapshot> snapShots )
        {
            this.Players = players.OrderByDescending(p=>p.RatePerMinute).ToList().AsReadOnly();
            this.SnapShots = snapShots.Where(sn => players.All(p=> sn.PlayerNumbers.Contains(p.Number) )).ToList().AsReadOnly();



            this.Minutes = SnapShots.Select(sn=> sn.Elapsed ?? TimeSpan.FromSeconds(0)).Aggregate((ts1,ts2)=> ts1+ts2);
            this.Rate = SnapShots.Select(sn => sn.ScoreDiff).Aggregate((d1, d2) => d1 + d2);
            this.TeamScore = SnapShots.Select(sn => sn.TeamScore).Aggregate((d1, d2) => d1 + d2);
            this.OponentScore = SnapShots.Select(sn => sn.OponentScore).Aggregate((d1, d2) => d1 + d2);

            this.RatePerMinute = Minutes.TotalMinutes > 0 ? Rate / Minutes.TotalMinutes : 0;
        }

        public override string ToString()
        {
            return $"{this.Players.Count} Players; {this.Minutes} Minutes; +- Per Minute: {this.RatePerMinute}";
        }

        public int CompareTo(LineUpViewModel other)
        {
            if (other == null)
            {
                throw new ArgumentException("Need an instance of LineUpViewModel for comparison.");
            }
            return -1 * this.RatePerMinute.CompareTo(other.RatePerMinute);
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as LineUpViewModel);
        }
    }
}
