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
        private readonly LineUp _lineUp;

        public IReadOnlyCollection<PlayerViewModel> Players { get; }
        public IReadOnlyCollection<GameSnapshot> SnapShots { get; }
        public TimeSpan Minutes => this._lineUp.Elapsed;
        public int Rate => this._lineUp.Rate;
        public int TeamScore => this._lineUp.TeamScore;
        public int OponentScore => this._lineUp.OponentScore;



        public double RatePerMinute => this._lineUp.RatePerMinute;
        public double OffenciveRate => this._lineUp.OffenciveRate;
        public double DefenciveRate => this._lineUp.DefenciveRate;

        public LineUpViewModel(IEnumerable<PlayerViewModel> players, IEnumerable<GameSnapshot> snapShots )
        {
            this._lineUp = new LineUp(snapShots, players.Select(p => p.Number).ToArray());
            this.Players = players.OrderByDescending(p=>p.RatePerMinute).ToList().AsReadOnly();
            this.SnapShots = snapShots.Where(sn => players.All(p=> sn.PlayerNumbers.Contains(p.Number) )).ToList().AsReadOnly();
            
            
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
            return this._lineUp.CompareTo(other._lineUp);
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as LineUpViewModel);
        }
    }
}
