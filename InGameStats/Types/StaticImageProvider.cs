using Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utils;

namespace Types
{
    public class StaticImageProvider : IPlayerImageProvider
    {
        const int WIDTH = 84;
        const int HEIGHT = 105;

        private readonly Dictionary<int, BitmapImage> imageByNumberCache = new Dictionary<int, BitmapImage>();
        
        public Colors GetPlayerBackgroundImage(int playerNumber)
        {
            throw new System.NotImplementedException();
        }

        public BitmapImage GetPlayerImage(int playerNumber)
        {

            BitmapImage bitmapImage;
            if (this.imageByNumberCache.TryGetValue(playerNumber, out bitmapImage))
                return bitmapImage;

            Bitmap ret;
            try
            {
                ret = Properties.Resources.ResourceManager.GetObject($"_{playerNumber}") as Bitmap;
            }
            catch
            {
                ret = null;
            }
            ret = ret ?? Properties.Resources.default_person;

            var resized = ImageUtils.Resize(ret, WIDTH, HEIGHT);

            bitmapImage = ImageUtils.BitMapToBitMapImage(resized);
            this.imageByNumberCache[playerNumber] = bitmapImage;

            return bitmapImage;
        }


        

      

       

        
    }
}