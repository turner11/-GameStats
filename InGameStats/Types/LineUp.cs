using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Types
{
    public class LineUp: IComparable<LineUp>, IReadOnlyList<int>, IComparable
    {
        public ReadOnlyCollection<int> PlayerNumbers { get; }
        public IReadOnlyList<GameSnapshot> Snapshots { get; }
        public int Rate { get; }
        public TimeSpan Elapsed { get; }

        public double OffenciveRate { get; }
        public double DefenciveRate { get; }

        public double RatePerMinute => this.Rate / this.Elapsed.TotalMinutes;
        

        public int TeamScore => this.Snapshots.Sum(sn => sn.TeamScore);
        public int OponentScore => this.Snapshots.Sum(sn => sn.OponentScore);


        public int Count => this.PlayerNumbers.Count;
        public int this[int index] => this.PlayerNumbers[index];

        public LineUp(IEnumerable<GameSnapshot> snapShots, params int[] players)
        {
            this.PlayerNumbers = players.OrderBy(n => n).ToList().AsReadOnly();

            //var ps = new int[] { 5, 2, 22, 55, 8 };
            //if(ps.Intersect(this.PlayerNumbers).Count() ==5)
            //    1.ToString();


            this.Snapshots = this.GetRelevantSnapshots(snapShots);
            this.Rate = this.Snapshots.Sum(sn => sn.ScoreDiff);

            var totalSeconds = this.Snapshots.Sum(sn => sn.Elapsed?.TotalSeconds ?? 0);
            this.Elapsed = TimeSpan.FromSeconds(totalSeconds);

            this.OffenciveRate = this.Snapshots.Sum(sn => sn.TeamScore) / this.Elapsed.TotalMinutes;
            this.DefenciveRate = this.Snapshots.Sum(sn => sn.OponentScore) / this.Elapsed.TotalMinutes;


        }

        private ReadOnlyCollection<GameSnapshot> GetRelevantSnapshots(IEnumerable<GameSnapshot> snapShots)
        {
            var playerNumbers = this.ToList();
            var relevantSnapshots = snapShots.Where(sn=> sn.PlayerNumbers.Intersect(playerNumbers).Count() == playerNumbers.Count).ToList();
            return relevantSnapshots.AsReadOnly();
        }

        public static LineUp GetBestLineup(int playerCount, IEnumerable<GameSnapshot> snapshots, TimeSpan? minTime=null)
        {
            minTime = minTime ?? TimeSpan.FromSeconds(0);
            var lineups = LineUp.GetLineups(playerCount, snapshots, 
                                            filter:lu=> !double.IsNaN(lu.RatePerMinute) & lu.Elapsed >= minTime,
                                            orderBy: lu => lu);
            var bestLinup = lineups.FirstOrDefault();
            return bestLinup;

        }
        public static IList<LineUp> GetLineups(int playerCount, IEnumerable<GameSnapshot> snapshots)
        {
            return GetLineups(playerCount, snapshots, filter:null, orderBy:null);
        }

        public static IList<LineUp> GetLineups(int playerCount, IEnumerable<GameSnapshot> snapshots, 
                                                            Predicate<LineUp> filter = null, 
                                                            Func<LineUp, object> orderBy = null)
        {
            IEnumerable<LineUp> combos = LineUp.GetAllPlayersCombo(snapshots, playerCount);
            if (filter != null)
                combos = combos.Where(lu => filter(lu)).ToList();

            if (orderBy != null)
                combos = combos.OrderBy(lu => orderBy(lu)).ToList();
            else
                combos = combos.OrderBy(lu => lu).ToList();

            var lineups = combos.ToList();
            return lineups;
        }

        private static List<LineUp> GetAllPlayersCombo(IEnumerable<GameSnapshot> snapShots, int length)
        {
            var combos = snapShots.Select(sn => GetKCombs(sn.PlayerNumbers, length)).ToList();

            IEqualityComparer<List<int>> listComparer = new ListComparer();
            List<List<int>> combosLists = combos.SelectMany(arr => arr.Select(v => v.ToList()).ToList()).Distinct(comparer: listComparer).ToList();

            //var players = combosLists.Select(c => LineUp.PlayersData.Where(p => c.Contains(p.Number)).OrderBy(p => p.Number).ToList()).ToList();
            List<int[]> playerNumers = combosLists.Select(lst => lst.OrderBy(n => n).ToArray()).ToList();
            var players = playerNumers.Select(nums => new LineUp(snapShots, players:nums)).ToList();
            return players;
        }

        public static LineUp GetOffensiveLineup(IEnumerable<GameSnapshot> snapshots, int playerCount=5, TimeSpan? minTime=null)
        {
            minTime = minTime ?? TimeSpan.FromSeconds(0);
            IList<LineUp> allLineUps = LineUp.GetLineups(playerCount, snapshots, 
                filter:new Predicate<LineUp>(lu=> lu.Elapsed >= minTime));

            
            var lineup = allLineUps.OrderByDescending(lu => lu.TeamScore).FirstOrDefault();
            return lineup;
        }


        public static LineUp GetDeffensiveLineup(IEnumerable<GameSnapshot> snapshots, int playerCount=5, TimeSpan? minTime = null)
        {
            minTime = minTime ?? TimeSpan.FromSeconds(0);
            IList<LineUp> allLineUps = LineUp.GetLineups(playerCount, snapshots,
                filter: new Predicate<LineUp>(lu => lu.Elapsed >= minTime));


            var lineup = allLineUps.OrderBy(lu => lu.OponentScore).FirstOrDefault();
            return lineup;
        }



        static IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
            //static IEnumerable<IEnumerable<PlayerViewModel>> GetKCombs(IEnumerable<PlayerViewModel> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            var ks = GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));

            //ks = ks.Select(arr => arr.ToList()).ToList();
            return ks;
        }

        public override string ToString()
        {
            return $"{String.Join(",", this.PlayerNumbers)}; Time: {this.Elapsed}; RPM: {this.RatePerMinute}";
        }

        public int CompareTo(LineUp other)
        {
            if (other == null)
            {
                throw new ArgumentException("Need an instance of LineUp for comparison.");
            }
            return -1* this.RatePerMinute.CompareTo(other.RatePerMinute);
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as LineUp);
        }

        public IEnumerator<int> GetEnumerator()
        {
            return this.PlayerNumbers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class ListComparer : IEqualityComparer<List<int>>
    {
        public bool Equals(List<int> x, List<int> y)
        {
            if (x.Count != y.Count)
                return false;
            for (int i = 0; i < x.Count; i++)
                if (x[i] != y[i]) return false;
            return true;
        }
        public int GetHashCode(List<int> x) => x.Count;
    }
}


