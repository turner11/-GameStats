using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Types;

namespace ViewModels
{
    public class GameViewModel : ViewModelBase
    {

        static readonly TimeSpan LINEUPS_MIN_TIME = TimeSpan.FromMinutes(2);
        const int LINEUPS_COUT = 7;

        private readonly List<GameSnapshot> _snapshots;
        public ObservableCollection<PlayerViewModel> PlayersData { get; }

        public ObservableCollection<SnapshotViewModel> SnapShotsViewModel { get; }


        //this.OnPropertyChanged(an.Name);
        public IList<LineUpViewModel> Linups5 { get; private set; }
        public IList<LineUpViewModel> Linups4 { get; private set; }
        public IList<LineUpViewModel> Linups3 { get; private set; }
        public IList<LineUpViewModel> Linups2 { get; private set; }

        public ObservableCollection<PlayerViewModel> PlayerMinutes { get; }
        public IList<LineUpViewModel> OffensiveLineups { get; private set; }
        public IList<LineUpViewModel> DefensiveLineups { get; private set; }
        public IPlayerImageProvider ImageProvider { get; set; }



        public GameViewModel(IPlayerImageProvider imageProvider = null)
        {
            this._snapshots = new List<GameSnapshot>();
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


        public void SetSnapShots(IEnumerable<GameSnapshot> snapShots)
        {
            this._snapshots.Clear();
            this._snapshots.AddRange(snapShots);


            var players = PlayerData.GetData(snapShots);
            var playersVM = players.Select(p => new PlayerViewModel(p, this.ImageProvider)).OrderByDescending(p => p.RatePerMinute).ToList();
            this.SetPlayers(playersVM);


            this.Linups5 = this.GetLineup(5).Take(LINEUPS_COUT).ToList();//this.GetBestLineup(5);
            this.Linups4 = this.GetLineup(4).Take(LINEUPS_COUT).ToList();//this.GetBestLineup(4);
            this.Linups3 = this.GetLineup(3).Take(LINEUPS_COUT).ToList();//this.GetBestLineup(3);
            this.Linups2 = this.GetLineup(2).Take(LINEUPS_COUT).ToList();//this.GetBestLineup(2);

            this.OffensiveLineups = this.GetOffensiveLineup();
            this.DefensiveLineups = this.GetDeffensiveLineup();
            
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
                foreach (var item in an.List)
                {
                    item.PropertyChanged += (s, arg) => this.OnPropertyChanged(an.Name);
                }
                //For initial binding
                this.OnPropertyChanged(an.Name);
            }





            var playerByMinutes = playersVM.OrderByDescending(pvm => pvm.Minutes).ToList();
            SetObservableColelctionContent(this.PlayerMinutes, playerByMinutes);




            var snapshotsVM = snapShots.Select(sn => new SnapshotViewModel(sn, ImageProvider)).ToList();
            SetObservableColelctionContent(this.SnapShotsViewModel, snapshotsVM);

        }

        private IList<LineUpViewModel> GetOffensiveLineup(int playerCount = 5)
        {
            IList<LineUp> lineups = LineUp.GetOffensiveLineup(this._snapshots, playerCount, LINEUPS_MIN_TIME);
            return lineups.Select(this.LineUpToViewModel).ToList();
        }

        private IList<LineUpViewModel> GetDeffensiveLineup(int playerCount = 5)
        {
            IList<LineUp> lineups = LineUp.GetDeffensiveLineup(this._snapshots, playerCount, LINEUPS_MIN_TIME);
            return lineups.Select(this.LineUpToViewModel).ToList();
        }

        private LineUpViewModel LineUpToViewModel(LineUp lineup)
        {
            var players = lineup.PlayerNumbers.Select(number => this.PlayersData.Where(pd => pd.Number == number).FirstOrDefault()).ToList();
            return new LineUpViewModel(players, this._snapshots);
        }

        private IList<LineUpViewModel> GetLineup(int playerCount)
        {
            IList<LineUp> lineups = LineUp.GetLineups(playerCount, this._snapshots, 
                                    filter: new Predicate<LineUp>(lu => lu.Elapsed >= LINEUPS_MIN_TIME));
            List<int[]> numbers = lineups.Select(lu => lu.ToArray()).Distinct().ToList();
            List<PlayerViewModel[]> players = numbers.Select(list =>
                                list.Select(
                                    n => this.PlayersData.FirstOrDefault(p => p.Number == n)).ToArray()
                            )
                            .ToList();


            var vms = players.Select(ps => new LineUpViewModel(ps, this._snapshots)).ToList();
            vms.Sort();
            return vms;
        }

        private LineUpViewModel GetBestLineup(int playerCount)
        {

            LineUp lineup = LineUp.GetBestLineup(playerCount, this._snapshots, LINEUPS_MIN_TIME);
            List<PlayerViewModel> players = this.PlayersData.Where(pvm => lineup.Contains(pvm.Number)).ToList();
            var vm = new LineUpViewModel(players, this._snapshots);
            return vm;
        }

        private static void SetObservableColelctionContent<T>(ObservableCollection<T> observable, IEnumerable<T> collection)
        {
            observable.Clear();
            foreach (var item in collection)
            {
                observable.Add(item);
            }
        }



        public void SetPlayers(IList<PlayerViewModel> players)
        {
            this.PlayersData.Clear();
            foreach (var player in players)
            {
                this.PlayersData.Add(player);
            }
        }









    }
}

