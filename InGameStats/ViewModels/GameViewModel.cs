using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Types;

namespace ViewModels
{
    public class GameViewModel: ViewModelBase
    {

       

        private readonly List<PlayerData.GameSnapshot> _snapshots;
        public ObservableCollection<PlayerViewModel> PlayersData { get; }

        public ObservableCollection<SnapshotViewModel> SnapShotsViewModel { get; }


        //this.OnPropertyChanged(an.Name);
        public LineUpViewModel Linups5 { get; private set; }
        public LineUpViewModel Linups4 { get; private set; }
        public LineUpViewModel Linups3 { get; private set; }
        public LineUpViewModel Linups2 { get; private set; }

        public ObservableCollection<PlayerViewModel> PlayerMinutes { get; }
        public LineUpViewModel OffensiveLineups { get; private set; }
        public LineUpViewModel DefensiveLineups { get; private set; }
        public IPlayerImageProvider ImageProvider { get; set; }
        


        public GameViewModel(IPlayerImageProvider imageProvider=null)
        {
            this._snapshots = new List<PlayerData.GameSnapshot>();
            this.PlayersData = new ObservableCollection<PlayerViewModel>();

            this.Linups5 = null;
            this.Linups4 = null;
            this.Linups3 = null;
            this.Linups2 = null;
            this.PlayerMinutes = new ObservableCollection<PlayerViewModel>();

            this.OffensiveLineups = null;
            this.DefensiveLineups = null;

            this.ImageProvider = imageProvider ?? new StaticImageProvider();

            this.SnapShotsViewModel = new ObservableCollection<SnapshotViewModel>();


        }


        public void SetSnapShots(IEnumerable<PlayerData.GameSnapshot> snapShots)
        {
            this._snapshots.Clear();
            this._snapshots.AddRange(snapShots);


            var players = PlayerData.GetData(snapShots);
            var playersVM = players.Select(p => new PlayerViewModel(p, this.ImageProvider)).OrderByDescending(p => p.RatePerMinute).ToList();
            this.SetPlayers(playersVM);


            this.Linups5 = this.GetBestLineup(5);
            this.Linups4 = this.GetBestLineup(4);
            this.Linups3 = this.GetBestLineup(3);
            this.Linups2 = this.GetBestLineup(2);


            var allLineUps = this.GetLineup(5);

            this.OffensiveLineups = allLineUps.OrderByDescending(lu => lu.TeamScore / lu.Minutes.TotalMinutes).FirstOrDefault();
            this.DefensiveLineups = allLineUps.OrderBy(lu => lu.OponentScore / lu.Minutes.TotalMinutes).FirstOrDefault();


            var lineUpLists = new[] {
                new {List = this.Linups5, Name = nameof(this.Linups5)},
                new {List = this.Linups4, Name = nameof(this.Linups4)},
                new {List = this.Linups3, Name = nameof(this.Linups3)},
                new {List = this.Linups2, Name = nameof(this.Linups2) },
                new {List = this.OffensiveLineups, Name = nameof(this.OffensiveLineups) },
                new {List = this.DefensiveLineups, Name = nameof(this.DefensiveLineups) },


            };

            foreach (var an in lineUpLists)
            {
                an.List.PropertyChanged += (s, arg) => this.OnPropertyChanged(an.Name);
                //For initial binding
                this.OnPropertyChanged(an.Name);


            }



            var playerByMinutes = playersVM.OrderByDescending(pvm => pvm.Minutes).ToList();
            SetObservableColelctionContent(this.PlayerMinutes, playerByMinutes);

           


            var snapshotsVM = snapShots.Select(sn => new SnapshotViewModel(sn, ImageProvider)).ToList();
            SetObservableColelctionContent(this.SnapShotsViewModel, snapshotsVM);

        }



        private LineUpViewModel GetBestLineup(int playerCount)
        {
            var lineups = this.GetLineup(playerCount);
            var bestLinup = lineups.FirstOrDefault();
            return bestLinup;

        }
        private IList<LineUpViewModel> GetLineup(int playerCount)
        {
            List<List<PlayerViewModel>> combos = this.GetAllPlayersCombo(this._snapshots, playerCount);
            var lineups = combos.Select(c => new LineUpViewModel(c, this._snapshots)).ToList();
            lineups.Sort();
            return lineups;
           
           
        }

        private static void SetObservableColelctionContent<T>(ObservableCollection<T> observable, IEnumerable<T> collection)
        {
            observable.Clear();
            foreach (var item in collection)
            {
                observable.Add(item);
            }
        }

        private List<List<PlayerViewModel>> GetAllPlayersCombo(IEnumerable<PlayerData.GameSnapshot> snapShots, int length)
        {
            var combos = snapShots.Select(sn => GetKCombs(sn.PlayerNumbers, length)).ToList();

            IEqualityComparer<List<int>> listComparer = new ListComparer();
            var combosLists = combos.SelectMany(arr => arr.Select(v => v.ToList()).ToList()).Distinct(comparer: listComparer).ToList();

            var players = combosLists.Select(c => this.PlayersData.Where(p => c.Contains(p.Number)).OrderBy(p=>p.Number).ToList()).ToList();

            return players;
        }

        public void SetPlayers(IList<PlayerViewModel> players)
        {
            this.PlayersData.Clear();
            foreach (var player in players)
            {
                this.PlayersData.Add(player);
            }
        }

        static IEnumerable<IEnumerable<T>>GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        //static IEnumerable<IEnumerable<PlayerViewModel>> GetKCombs(IEnumerable<PlayerViewModel> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            var ks = GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));

            //ks = ks.Select(arr => arr.ToList()).ToList();
            return ks;
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
}

