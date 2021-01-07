using System;
using SFML.System;
using BlackCoat;
using BlackCoat.Entities;

namespace Gemmy
{
    class Block : Graphic
    {
        private Int32 _Type = 0;
        private Int32 _Column = 0;
        private Int32 _Row = 0;


        public Int32 Type => _Type;

        public Int32 Column
        {
            get => _Column;
            set
            {
                _Column = value;
                Position = new Vector2f(GameScene.FIELD_X + GameScene.BLOCKSIZE * _Column, Position.Y);
            }
        }

        public Int32 Row
        {
            get => _Row;
            set
            {
                _Row = value;
                Position = new Vector2f(Position.X, GameScene.FIELD_Y + GameScene.BLOCKSIZE * _Row);
            }
        }


        public Block(Core core, Int32 color, TextureLoader texLoader):base(core)
        {
            if (color < 1 || color > 4) throw new ArgumentOutOfRangeException(nameof(color));
            _Type = color;
            Texture = texLoader.Load(String.Concat("Crystal", color), false);
        }

        public void Destroy(Action<int> pointsDelegate = null)
        {
            if (Parent != null) Parent.Remove(this);
            if(pointsDelegate != null) pointsDelegate.Invoke(_Type);
        }
    }
}