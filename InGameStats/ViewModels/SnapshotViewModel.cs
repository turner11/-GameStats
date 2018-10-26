using Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Utils;
using static Types.PlayerData;

namespace ViewModels
{
    public class SnapshotViewModel
    {
        private readonly GameSnapshot _snapshot;
        private readonly IPlayerImageProvider _imageProvider;
        private readonly List<BitmapImage> _players;


        public BitmapImage Player1 => this._players[0];
        public BitmapImage Player2 => this._players[1];
        public BitmapImage Player3 => this._players[2];
        public BitmapImage Player4 => this._players[3];
        public BitmapImage Player5 => this._players[4];

        public int TeamScore => this._snapshot.TeamScore;
        public int OponentScore => this._snapshot.OponentScore;
        public int Quarter => this._snapshot.Quarter;
        public string Elapsed => this._snapshot.Elapsed?.ToString("mm':'ss") ?? "?";
        public int? ScoreDiff => this._snapshot.ScoreDiff;



        public SnapshotViewModel(GameSnapshot snapshot, IPlayerImageProvider imageProvider)
        {
            this._snapshot = snapshot;
            this._imageProvider = imageProvider;
            var playerNumbers = this._snapshot.PlayerNumbers.Select(v => v).Take(5).ToList();
            playerNumbers.AddRange(Enumerable.Repeat(-1, 5 - playerNumbers.Count));
            this._players = playerNumbers
                            .Select(n => new { Number = n, Image = this._imageProvider.GetPlayerImage(n) })
                            .OrderBy(an => an.Number)
                            .Select(an => this.GetPlayerImage(an.Image, an.Number))
                            .ToList();
            

        }

        private BitmapImage GetPlayerImage(BitmapImage image, int numbner)
        {
            var text = $"#{numbner}";
            var bitmap = ImageUtils.BitmapImage2Bitmap(image);
            float width = 150;
            float height = 50;
            float x = 0;
            float y = (float)image.Height - height;
            var rectf = new RectangleF(x,y, width, height );
            Graphics g = Graphics.FromImage(bitmap);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(text, new Font("Tahoma", 28), Brushes.DarkBlue, rectf);

            g.Flush();

            BitmapImage bitmapImage = ImageUtils.BitMapToBitMapImage(bitmap);
            return bitmapImage;
        }

        
    }
}
