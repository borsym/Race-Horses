using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using ZH.Model;

namespace ZH.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private GameModel _model;
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand NewGameCommand10 { get; private set; }
        public DelegateCommand NewGameCommand15 { get; private set; }
        public DelegateCommand NewGameCommand20 { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }
        public ObservableCollection<ModelField> Fields { get; set; }
        public Int32 GameStepCount { get { return _model.GameStepCount; } }
        public String GameTime { get { return TimeSpan.FromSeconds(_model.GameTime).ToString("g"); } }
        public Int32 GridSize { get; private set; }

        public event EventHandler<int> NewGame;
        public event EventHandler ExitGame;

        public GameViewModel(GameModel model)
        {
            GridSize = 10;
            // játék csatlakoztatása
            _model = model;
            _model.GameAdvanced += new EventHandler<ModelEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<ModelEventArgs>(Model_GameOver);
            _model.GameCreated += new EventHandler<ModelEventArgs>(Model_GameCreated);

            // parancsok kezelése
            NewGameCommand = new DelegateCommand(param => OnNewGame(GridSize));
            NewGameCommand10 = new DelegateCommand(param => OnNewGame(10));
            NewGameCommand15 = new DelegateCommand(param => OnNewGame(15));
            NewGameCommand20 = new DelegateCommand(param => OnNewGame(20));


            // játéktábla létrehozása
            Fields = new ObservableCollection<ModelField>();
            for (Int32 i = 0; i < _model.Table.Size; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < 5; j++)
                {
                    Fields.Add(new ModelField
                    {
                        Text = String.Empty,
                        X = i,
                        Y = j,
                        Number = i * 5 + j,
                        StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                        
                    });
                }
            }
            RefreshTable();
        }

        public void RefreshTable()
        {
            foreach (ModelField field in Fields) // inicializálni kell a mezőket is
            {
                field.Text = !_model.Table.IsEmpty(field.X, field.Y) ? "/o|\n  |----|\\\n  |----| *\n  ||  ||" : String.Empty;

                field.Type = _model.Table[field.X, field.Y];
            }

            OnPropertyChanged("GameTime");
        }

        private void StepGame(int index)
        {
            ModelField field = Fields[index];

            _model.Step(field.X, field.Y);

            field.Text = _model.Table[field.X, field.Y] > 0 ? "Paci" : String.Empty; // visszaírjuk a szöveget
            OnPropertyChanged("GameStepCount"); // jelezzük a lépésszám változást
            field.Type = _model.Table[field.X, field.Y];
            field.Text = !_model.Table.IsEmpty(field.X, field.Y) ? "Paci" : String.Empty;
        }


       
        private void OnNewGame(int size)
        {
            Debug.Write("ramkattolt" + size +"\n");
            Fields.Clear();
            GridSize = size;
            OnPropertyChanged("GridSize");
            if (NewGame != null)
                NewGame(this, size);
            for (Int32 i = 0; i < size; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < 5; j++)
                {
                    Fields.Add(new ModelField
                    {
                        Text = String.Empty,
                        X = i,
                        Y = j,
                        Number = i * 5 + j,
                        StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                    });
                }
            }
            RefreshTable();

        }

        private void Model_GameCreated(object sender, ModelEventArgs e)
        {
            RefreshTable();
        }

        private void Model_GameOver(object sender, ModelEventArgs e)
        {
            Debug.Write("vege");
        }

        private void Model_GameAdvanced(object sender, ModelEventArgs e)
        {
            OnPropertyChanged("GameTime");

        }
        private void OnExitGame()
        {
            if (ExitGame != null)
                ExitGame(this, EventArgs.Empty);
        }
    }
}
