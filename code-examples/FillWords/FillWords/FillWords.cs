using System;
using System.Collections.Generic;

namespace FillWords
{
    public class FillWords
    {
        private readonly Field   _field;
        private readonly IDrawer _drawer;
        
        private readonly int _fieldWidth;
        private readonly int _fieldHeight;

        private int _hoverX;
        private int _hoverY;

        private readonly CellState[,] _cellStates;

        private bool   _isSelectionModeOn;
        private string _word = String.Empty;
        private readonly List<int[]> _wordCoords = new List<int[]>();

        private enum CellState
        {
            None,
            Selected,
            Guessed
        }

        public FillWords(IDrawer drawer, int fieldWidth, int fieldHeight)
        {
            _drawer = drawer;
            
            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;

            _field = new Field(fieldWidth, fieldHeight);
            _cellStates = new CellState[_field.GetFieldWidth(), _field.GetFieldHeight()];
        }

        public void Draw()
        {
            _drawer.Draw(this, _hoverX, _hoverY);
        }

        public void SetHover(int deltaX, int deltaY)
        {
            if(_hoverX + deltaX < _fieldWidth && _hoverX + deltaX >= 0) _hoverX += deltaX;
            if(_hoverY + deltaY < _fieldHeight && _hoverY + deltaY >= 0) _hoverY += deltaY;

            if (_isSelectionModeOn) SelectHoveredCell();
        }

        public void SwitchSelectionMode()
        {
            _isSelectionModeOn = !_isSelectionModeOn;

            if (_isSelectionModeOn)
            {
                SelectHoveredCell();
            }
            else
            {
                ProcessWord();
                ResetSelection();
            }
        }
        
        public int GetFieldHeight()
        {
            return _field.GetFieldHeight();
        }

        public int GetFieldWidth()
        {
            return _field.GetFieldWidth();
        }

        public bool IsCellSelected(int x, int y)
        {
            return _cellStates[x, y] == CellState.Selected;
        }
        
        public bool IsCellGuessed(int x, int y)
        {
            return _cellStates[x, y] == CellState.Guessed;
        }
        
        public bool IsCellHovered(int x, int y)
        {
            // return _cellStates[x, y] == CellState.Hover;
            return x == _hoverX && y == _hoverY;
        }

        public char GetLetter(int x, int y)
        {
            return _field.GetLetter(x, y);
        }

        private void ProcessWord()
        {
            bool isSuccess = _field.CheckWord(_word, _wordCoords, out string message);
            if (!isSuccess) _drawer.DrawMessage(message);
            else            SetWordToGuessed();
        }
        
        private void SetWordToGuessed()
        {
            foreach (int[] coord in _wordCoords)
            {
                _cellStates[coord[0], coord[1]] = CellState.Guessed;
            }
        }
        
        private void SelectHoveredCell()
        {
            _cellStates[_hoverX, _hoverY] = CellState.Selected;
            AccumulateWord();
        }

        private void AccumulateWord()
        {
            _word += _field.GetLetter(_hoverX, _hoverY);
            _wordCoords.Add(new []{ _hoverX, _hoverY });
        }

        private void ResetSelection()
        {
            ResetWord();
            
            for (int i = 0; i < 6; i++)
            for (int j = 0; j < 6; j++)
            {
                if(_cellStates[i, j] == CellState.Selected) 
                    _cellStates[i, j] = CellState.None;
            }
        }

        private void ResetWord()
        {
            _word = String.Empty;
            _wordCoords.Clear();
        }
    }
}