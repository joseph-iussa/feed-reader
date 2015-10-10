using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using FeedReader.Model;
using FeedReader.Repository;
using FeedReader.ViewModel;

namespace FeedReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private DataRepository repo;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            repo = new DataRepository(new DB());

            MainWindow mainWindow = new MainWindow();
            MainWindow.DataContext = new MainWindowViewModel(repo);
            mainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            repo.Dispose();
        }
    }
}