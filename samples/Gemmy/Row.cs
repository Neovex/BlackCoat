using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gemmy
{
    class Row
    {
        private Block[] _Blocks;

        public Boolean IsFull { get { return !_Blocks.Any(b => b == null); } }


        public Row(int columns)
        {
            _Blocks = new Block[columns];
        }


        public Boolean IsBlockFree(int column)
        {
            if (column < 0 || column >= _Blocks.Length) return false;
            return _Blocks[column] == null;
        }

        public void SetBlock(Block block)
        {
            _Blocks[block.Column] = block;
        }

        public void Clear(Action<int> pointsDelegate = null)
        {
            for (int i = 0; i < _Blocks.Length; i++)
            {
                if (_Blocks[i] != null)
                {
                    _Blocks[i].Destroy(pointsDelegate);
                    _Blocks[i] = null;
                }
            }
        }

        public void MoveDown()
        {
            foreach (var block in _Blocks)
            {
                if(block != null) block.Row++;
            }
        }
    }
}