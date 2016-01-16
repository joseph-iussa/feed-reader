using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FeedReader.View;

namespace FeedReader.Interaction
{
    public delegate bool RequestConfirmationDelegate(string message, string caption);

    public delegate bool? ShowDialogDelegate(string title, object viewModel);

    public delegate void ShowMessageDelegate(string message);

    class InteractionService
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

        public RequestConfirmationDelegate RequestConfirmation { get; set; }
        public ShowDialogDelegate ShowDialog { get; set; }
        public ShowMessageDelegate ShowMessage { get; set; }

        public InteractionService()
        {
            RequestConfirmation = DefaultRequestConfirmation;
            ShowDialog = DefaultShowDialog;
            ShowMessage = DefaultShowMessage;
        }
    }
}