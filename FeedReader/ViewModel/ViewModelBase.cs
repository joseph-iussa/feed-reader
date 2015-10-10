using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using FeedReader.View;

namespace FeedReader.ViewModel
{
    // TODO: Move these to a separate Interaction Service.
    public delegate bool RequestConfirmationDelegate(string message, string caption);

    public delegate bool? ShowDialogDelegate(string title, object viewModel);

    public delegate void ShowMessageDelegate(string message);

    abstract class ViewModelBase : INotifyPropertyChanged, IDialogCloser
    {
        public static bool DefaultRequestConfirmation(string message, string caption)
        {
            return MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.Yes;
        }

        public static bool? DefaultShowDialog(string title, object viewModel)
        {
            var dialog = new ModalDialog();
            dialog.Title = title;
            dialog.DataContext = viewModel;
            dialog.Owner = Application.Current.MainWindow;

            return dialog.ShowDialog();
        }

        public static void DefaultShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        protected RequestConfirmationDelegate RequestConfirmation;
        protected ShowDialogDelegate ShowDialog;
        protected ShowMessageDelegate ShowMessage;

        protected ViewModelBase(RequestConfirmationDelegate RequestConfirmation = null,
                                ShowDialogDelegate ShowDialog = null,
                                ShowMessageDelegate ShowMessage = null)
        {
            if (RequestConfirmation == null)
            {
                this.RequestConfirmation = DefaultRequestConfirmation;
            }
            else
            {
                this.RequestConfirmation = RequestConfirmation;
            }

            if (ShowDialog == null)
            {
                this.ShowDialog = DefaultShowDialog;
            }
            else
            {
                this.ShowDialog = ShowDialog;
            }

            if (ShowMessage == null)
            {
                this.ShowMessage = DefaultShowMessage;
            }
            else
            {
                this.ShowMessage = ShowMessage;
            }
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