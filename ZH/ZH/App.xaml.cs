using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ZH.Model;
using ZH.ViewModel;

namespace ZH
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private GameModel _model;
        private GameViewModel _viewModel;
        private MainWindow _view;
        private DispatcherTimer _timer;
        private bool isStoped;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // modell létrehozása
            isStoped = false;
            _model = new GameModel();
            _model.GameOver += new EventHandler<ModelEventArgs>(Model_GameOver);
            _model.NewGame(10);

            // nézemodell létrehozása
            _viewModel = new GameViewModel(_model);
            _viewModel.NewGame += ViewModel_NewGame;
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);


            // nézet létrehozása
            _view = new MainWindow();
            _view.KeyDown += _view_KeyDown;
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); // eseménykezelés a bezáráshoz
            _view.Show();

            // időzítő létrehozása
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(Timer_Tick);
            //_timer.Start();
        }

        private void _view_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Space && !isStoped)
            {
                _timer.Start();
                isStoped = true;
            }
            else
            {
                _timer.Stop();
                isStoped = false;
            }
        }

        private void ViewModel_ExitGame(object sender, EventArgs e)
        {
            _view.Close();
        }

        private void View_Closing(object sender, CancelEventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Game", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // töröljük a bezárást

                if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                    _timer.Start();
            }
        }
        private void ViewModel_NewGame(object sender, int e)
        {
            _model.NewGame(e);
          //  _timer.Start();
        }

        private void Model_GameOver(object sender, ModelEventArgs e)
        {
            _timer.Stop();
            MessageBox.Show("nyertel ugyi vagy");
            _model.NewGame(_viewModel.GridSize);
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            _model.AdvanceTime();
            _viewModel.RefreshTable();
        }
    }
}
