using System.Collections.Generic;

namespace FillWords
{
    public class Field
    {
        private char[,] _field;
        private readonly int _fieldWidth;
        private readonly int _fieldHeight;

        private class WordInfo
        {
            public string Word { get; set; }
            public List<int[]> Coords { get; set; }
        }

        private List<WordInfo> _words;

        public Field(int fieldWidth, int fieldHeight)
        {
            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;
            GenerateField();
        }

        private void GenerateField()
        {
            _field = new[,]
            {
                {'ч', 'о', 'б', 'р', 'а', 'з'},
                {'а', 'с', 'в', 'о', 'д', 'я'},
                {'е', 'в', 'а', 'р', 'а', 'и'},
                {'и', 'л', 'и', 'п', 'у', 'ц'},
                {'н', 'е', 'н', 'р', 'м', 'а'},
                {'с', 'у', 'ф', 'о', 'г', 'ю'}
            };

            _words = new List<WordInfo>();
            _words.Add(new WordInfo
            {
                Word = "час",
                Coords = new List<int[]> {new[] {0, 0}, new[] {1, 0}, new[] {1, 1}}
            });
            _words.Add(new WordInfo
            {
                Word = "образ",
                Coords = new List<int[]> {new[] {0, 0}, new[] {0, 1}, new[] {1, 2}, new[] {1, 3}, new[] {1, 4}, new[] {1, 5}}
            });
        }

        public char GetLetter(int x, int y)
        {
            return _field[y, x];
        }

        public int GetFieldWidth()
        {
            return _fieldWidth;
        }

        public int GetFieldHeight()
        {
            return _fieldHeight;
        }

        public bool CheckWord(string userWord, List<int[]> coords, out string message)
        {
            message = string.Empty;
            bool isFound = false;

            foreach (WordInfo wordInfo in _words)
            {
                string knownWord = wordInfo.Word;
                List<int[]> knownCoords = wordInfo.Coords;
                if (knownWord == userWord)
                {
                    isFound = true;
                    for (int i = 0; i > knownCoords.Count; i++)
                    {
                        if (knownCoords[i][0] != coords[i][0]
                            || knownCoords[i][1] != coords[i][1])
                        {
                            message = "Попробуйте по-другому";
                            return false;
                        }
                    }
                }
            }

            if (!isFound) message = $"Слово \"{userWord}\" не было загадано.";
            return isFound;
        }
    }
}