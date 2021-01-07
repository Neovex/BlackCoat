using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gemmy
{
    class Grid
    {
        private int _Columns;
        private Row[] _Rows;

        public Row[] Rows { get { return _Rows; } }

        public Grid(int columns, int rows)
        {
            _Columns = columns;
            _Rows = new Row[rows];
            for (int i = 0; i < rows; i++) _Rows[i] = new Row(_Columns);
        }

        public Boolean IsBlockFree(int column, int row)
        {
            if (row < 0 || row >= _Rows.Length) return false;
            return _Rows[row].IsBlockFree(column);
        }

        public void SetBlock(Block block)
        {
            _Rows[block.Row].SetBlock(block);
        }

        public void ClearFullLinesAndMoveRemains(Action<int> pointsDelegate)
        {
            for (int i = _Rows.Length - 1; i > -1; i--)
            {
                var row = _Rows[i];
                if (row.IsFull)
                {
                    // clear
                    row.Clear(pointsDelegate);
                    // update array (except first row)
                    for (int j = i; j > 0; j--)
                    {
                        _Rows[j] = _Rows[j - 1];
                        _Rows[j].MoveDown();
                    }
                    // Create new row for first index
                    _Rows[0] = new Row(_Columns);

                    // repeat row scan
                    i++;
                }
            }
        }

        public void Clear()
        {
            foreach (var row in _Rows)
            {
                row.Clear();
            }
        }
    }
}