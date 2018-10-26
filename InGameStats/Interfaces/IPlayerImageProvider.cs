using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Interfaces
{
    public interface IPlayerImageProvider
    {
        BitmapImage GetPlayerImage(int playerNumber);
        Colors GetPlayerBackgroundImage(int playerNumber);

    }
}