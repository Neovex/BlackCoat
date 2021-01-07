using System;
using System.Linq;

using BlackCoat;
using BlackCoat.Entities;

namespace Gemmy
{
    class BlockSet
    {
        private Core _Core;
        private Grid _Grid;
        private BlockType _Type;
        private Block[] _Blocks = new Block[4];
        private Int32 _Frame = 0;

        public BlockSet(Grid grid, Core core)
        {
            _Core = core;
            _Grid = grid;
            _Type = (BlockType)core.Random.Next(1, 6);
        }

        public Boolean Spawn(BlockType type, Layer layer, TextureLoader textureLoader)
        {
            _Type = type;
            var color = ((int)type) % 4 + 1;
            var config = BlockConfig.GetConfig(_Type, _Frame);
            for (int i = 0; i < _Blocks.Length; i++)
            {
                var block = new Block(_Core, color, textureLoader)
                {
                    Column = config[i].X + 6,
                    Row = config[i].Y
                };
                layer.Add(block);
                _Blocks[i] = block;
                if (!_Grid.IsBlockFree(block.Column, block.Row)) return false; // Game Over condition
            }
            return true;
        }

        public void Destroy()
        {
            foreach (var block in _Blocks.Where(b => b != null))
            {
                block.Destroy();
            }
        }

        public Boolean Move(int col, int row)
        {
            var moveIsOk = true;
            foreach (var block in _Blocks)
            {
                moveIsOk &= _Grid.IsBlockFree(block.Column + col, block.Row + row);
            }

            if (moveIsOk)
            {
                // move all blocks
                foreach (var block in _Blocks)
                {
                    block.Column += col;
                    block.Row += row;
                }
            }
            return moveIsOk;
        }

        public void Rotate()
        {
            do
            {
                var oldconfig = BlockConfig.GetConfig(_Type, _Frame);
                var config = BlockConfig.GetConfig(_Type, ++_Frame);
                for (var i = 0; i < _Blocks.Length; i++)
                {
                    var block = _Blocks[i];
                    block.Column += config[i].X - oldconfig[i].X;
                    block.Row += config[i].Y - oldconfig[i].Y;
                }
            }
            while (_Blocks.Any(block => !_Grid.IsBlockFree(block.Column, block.Row)));
        }

        public void Freeze()
        {
            foreach (var block in _Blocks)
            {
                _Grid.SetBlock(block);
            }
        }
    }
}