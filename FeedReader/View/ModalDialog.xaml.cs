using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FeedReader.ViewModel;

namespace FeedReader.View
{
    /// <summary>
    /// Interaction logic for ModalDialog.xaml
    /// </summary>
    public partial class ModalDialog : Window
    {
        private IDialogCloser dialogCloser;

        public ModalDialog()
        {
            InitializeComponent();

            DataContextChanged += SetupCloseDialogHandler;
            Closed += HandleClosed;
        }

        private void HandleClosed(object sender, EventArgs e)
        {
            if (dialogCloser != null)
            {
                dialogCloser.DialogClosing -= CloseDialog;
            }
        }

        private void SetupCloseDialogHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            dialogCloser = e.NewValue as IDialogCloser;
            if (dialogCloser != null)
            {
                dialogCloser.DialogClosing += CloseDialog;
            }
        }

        private void CloseDialog(object sender, DialogClosingEventArgs e)
        {
            DialogResult = e.DialogResult;
        }
    }
}