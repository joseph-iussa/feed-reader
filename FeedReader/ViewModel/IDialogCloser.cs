using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedReader.ViewModel
{
    interface IDialogCloser
    {
        event EventHandler<DialogClosingEventArgs> DialogClosing;
    }
}