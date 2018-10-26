using System;
using System.Collections.Generic;
using System.ComponentModel;
using Interfaces;
using System.Linq;
using System.Text;

using Types;
using System.Windows.Media.Imaging;

namespace ViewModels
{
    public class PlayerViewModel: IComparable<PlayerViewModel>, IComparable
    {
        private PlayerData _player;


        private BitmapImage _image;
        public BitmapImage Image
        {
            get
            {
                if (this._image == null)
                {
                    this._image = this.ImageProvider.GetPlayerImage(this.Number);
                }
                return this._image;
                //ret = new BitmapImage(new Uri(uri));

            }

        }

        [DisplayName("Number")]
        public int Number { get { return this._player.Number; } }

        [DisplayName("Minutes")]
        public TimeSpan Minutes { get { return this._player.Minutes; } }

        [DisplayName("+-")]
        public int Rate { get { return this._player.Rate; } }

        [DisplayName("+- Per Minute")]
        public double RatePerMinute
        {
            get
            {
                double r = 0;
                if (Minutes.TotalMinutes > 0)
                {
                    r = Rate / Minutes.TotalMinutes;
                }
                return r;
            }
        }

        public IPlayerImageProvider ImageProvider { get; }

        public PlayerViewModel(PlayerData player, IPlayerImageProvider imageProvider=null)
        {
            this._player = player;
            this.ImageProvider = imageProvider;
        }

        public int CompareTo(PlayerViewModel other)
        {
            return this.Number.CompareTo(other.Number);
        }

        public int CompareTo(object obj)
        {
            var other = obj as PlayerViewModel;
            if (obj is null)
            {
                throw new ArgumentException("Need an instance of PlayerViewModel for comparison");
            }
            return this.CompareTo(other);
        }

        public override string ToString()
        {
            return $"#{this.Number}; Minutes: {this.Minutes}; Rate: {this.Rate}; Rate PM: {this.RatePerMinute}";
        }
    }
}
