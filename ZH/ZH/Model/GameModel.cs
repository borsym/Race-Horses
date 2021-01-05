using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ZH.Model
{
    public class GameModel
    {
        private ModelTable _table;
        private Int32 _gameStepCount;
        private Int32 _gameTime;
        private int counter;



        public Int32 GameStepCount { get { return _gameStepCount; } }
        public Int32 GameTime { get { return _gameTime; } }
        public ModelTable Table { get { return _table; } }

        public Boolean IsGameOver { get { return (_gameTime == 99); } }

        public event EventHandler<ModelEventArgs> GameAdvanced;
        public event EventHandler<ModelEventArgs> GameOver;
        public event EventHandler<ModelEventArgs> GameCreated;

        public GameModel()
        {
            _table = new ModelTable();
        }

        public void NewGame(int size)
        {
            _table = new ModelTable(size);
            counter = 2;
            _gameStepCount = 0;
            _gameTime = 0;
            GenerateFields();
            OnGameCreated();
        }

        public void AdvanceTime()
        {
            if (IsGameOver)
                return;

            MoveHorses();
            
            _gameTime++;
            OnGameAdvanced();

            if (IsGameOver2()) 
                OnGameOver(false);
        }
        //-1 => 0
        private bool IsGameOver2()
        {
            for (int i = 0; i < 5; ++i)
            {
                if (_table.GetValue(0, i) == 0) return false;
            }
            return true;
        }
        private void MoveHorses()
        {
            Random rand = new Random();
            double random = rand.NextDouble();
            for (int i = 0; i < _table.Size; i++)
            {
                for(int j = 0; j < 5; j++)
                {
                    random = rand.NextDouble();
                   // Debug.Write(random + "\n");
                    if(random <= 0.3)
                    {
                        if(_table.GetValue(0,j) == 1)
                        {
                            _table.SetValue(0, j, counter);
                            ++counter;
                        }
                        if(_table.GetValue(0,j) != 1 && i + 2 < _table.Size && _table.GetValue(i,j) == 1)
                        {
                            _table.SetValue(i, j, 0);
                            _table.SetValue(i + 2, j, 1);
                        }
                        
                    } 
                    else
                    {
                        if (i - 1 >= 0 && _table.GetValue(i, j) == 1)
                        {
                            _table.SetValue(i - 1, j, 1);
                            _table.SetValue(i, j, 0);
                        }
                       
                    }
                    
                }
            }   
        }

        public void Step(Int32 x, Int32 y) // ha tikre mozog a pálya előre akkor hasznos és akkor paraméter se kell
        {
            if (IsGameOver) // ha már vége a játéknak, nem játszhatunk
                return;

            _table.StepValue(x, y);

            _gameStepCount++; // lépésszám növelés

            OnGameAdvanced();

            //if (_table.IsFilled)
            //{
            //    OnGameOver(true);
            //}
        }

        private void GenerateFields()
        {
            for(Int32 i = 0; i < _table.Size; i++)
            {
                for (Int32 j = 0; j < 5; j++)
                {
                    _table.SetValue(i, j, 0);
                }
            }

            for(int i = 0; i < 5; i++)
            {
                _table.SetValue(_table.Size - 1, i, 1);
            }
        }

        private void OnGameAdvanced()
        {
            if (GameAdvanced != null)
                GameAdvanced(this, new ModelEventArgs(false, _gameStepCount, _gameTime));
        }
        private void OnGameOver(Boolean isWon)
        {
            if (GameOver != null)
                GameOver(this, new ModelEventArgs(isWon, _gameStepCount, _gameTime));
        }
        private void OnGameCreated()
        {
            if (GameCreated != null)
                GameCreated(this, new ModelEventArgs(false, _gameStepCount, _gameTime));
        }
    }
}
