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

        private readonly Dictionary<int, Bitmap> imageByName = new Dictionary<int, Bitmap>()
        {
            { 1, null }
        };
        public Colors GetPlayerBackgroundImage(int playerNumber)
        {
            throw new System.NotImplementedException();
        }

        public BitmapImage GetPlayerImage(int playerNumber)
        {
            

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

            BitmapImage bitmapImage = ImageUtils.BitMapToBitMapImage(resized);
            return bitmapImage;
        }


        

      

       

        
    }
}