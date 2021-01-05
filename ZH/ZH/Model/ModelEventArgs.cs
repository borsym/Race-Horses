using System;
using System.Collections.Generic;
using System.Text;

namespace ZH.Model
{
    public class ModelEventArgs : EventArgs
    {
        private Int32 _gameTime;
        private Int32 _steps;
        private Boolean _isWon;
        public Int32 GameTime { get { return _gameTime; } }
        public Int32 GameStepCount { get { return _steps; } }
        public Boolean IsWon { get { return _isWon; } }
        public ModelEventArgs(Boolean isWon, Int32 gameStepCount, Int32 gameTime)
        {
            _isWon = isWon;
            _steps = gameStepCount;
            _gameTime = gameTime;
        }
    }
}
