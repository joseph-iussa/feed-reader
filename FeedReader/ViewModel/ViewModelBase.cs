using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using FeedReader.Interaction;
using FeedReader.View;

namespace FeedReader.ViewModel
{
    abstract class ViewModelBase : INotifyPropertyChanged, IDialogCloser
    {
        public InteractionService InteractionService { get; private set; }

        protected ViewModelBase()
        {
            InteractionService = new InteractionService();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DialogClosingEventArgs> DialogClosing;

        protected virtual void CloseDialog(DialogClosingEventArgs e)
        {
            if (DialogClosing != null)
            {
                DialogClosing(this, e);
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}