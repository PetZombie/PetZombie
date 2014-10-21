using System;
using CocosSharp;

namespace PetZombieUI
{
    public class Block : PetZombie.Block
    {
        private CCSprite sprite;

        public CCSprite Sprite
        {
            get { return sprite; }
        }

        public CCRect Rectangle
        {
            get 
            { 
                return GetRectangle();
            }
        }

        public CCSize Size
        {
            get;
            private set;
        }

        public Block(string fileName, CCPoint position, CCSize size) : base(Converter.PointToPosition(position))
        {
            sprite = new CCSprite(fileName);
            sprite.Position = position;
            sprite.ScaleTo(size);
            this.Size = size;
        }

        private CCRect GetRectangle()
        {
            var x = sprite.PositionX - Size.Width*sprite.AnchorPoint.X;
            var y = sprite.PositionY - Size.Height*sprite.AnchorPoint.Y;

            return new CCRect(x, y, Size.Width, Size.Height);
        }
    }
}

