using System;
using CocosSharp;

namespace PetZombieUI
{
    public class Block : PetZombie.Block
    {
        public CCSprite Sprite
        {
            get;
            private set;
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

        public Block(PetZombie.Block block, CCSize size) : base(block.Position)
        {
            string fileName = "";

            var x = size.Width*0.5f + size.Width*block.Position.X;
            var y = size.Width*0.5f + size.Width*block.Position.Y;

            switch (block.Type)
            {
                case PetZombie.BlockType.Blue:
                    fileName = "Images/blue_ellipse_block";
                    break;
                case PetZombie.BlockType.Green:
                    fileName = "Images/green_ellipse_block";
                    break;
                case PetZombie.BlockType.Orange:
                    fileName = "Images/orange_ellipse_block";
                    break;
                case PetZombie.BlockType.Red:
                    fileName = "Images/red_ellipse_block";
                    break;
                case PetZombie.BlockType.Violet:
                    fileName = "Images/violet_ellipse_block";
                    break;
            }

            Sprite = new CCSprite(fileName);
            Sprite.Position = new CCPoint(x, y);
            Sprite.ScaleTo(size);
            this.Size = size;
        }

        private CCRect GetRectangle()
        {
            var x = Sprite.PositionX - Size.Width*Sprite.AnchorPoint.X;
            var y = Sprite.PositionY - Size.Height*Sprite.AnchorPoint.Y;

            return new CCRect(x, y, Size.Width, Size.Height);
        }
    }
}

