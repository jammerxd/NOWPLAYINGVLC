using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NOWPLAYINGVLC
{
    public interface IMainWindowCallbacks
    {
        void ShowMessage(string Title, string Message, Boolean exit);
        void ChangeDimensions(int Width, int Height);
    }
}
