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

        public CCRect WorldRectangle
        {
            get 
            { 
                return GetWorldRectangle();
            }
        }

        public CCSize Size
        {
            get;
            private set;
        }

        public Block(PetZombie.Block block, CCSize size) : base(block.Type, block.Position, block.Cage)
        {
            string fileName = "";

            switch (block.Type)
            {
                case PetZombie.BlockType.Blue:
                    fileName = "Images/blue";
                    break;
                case PetZombie.BlockType.Green:
                    fileName = "Images/green";
                    break;
                case PetZombie.BlockType.Orange:
                    fileName = "Images/orange";
                    break;
                case PetZombie.BlockType.Red:
                    fileName = "Images/red";
                    break;
                case PetZombie.BlockType.Violet:
                    fileName = "Images/violet";
                    break;
                case PetZombie.BlockType.Zombie:
                    fileName = "Images/zombie_block";
                    break;
                case PetZombie.BlockType.Brain:
                    fileName = "Images/brain";
                    break;
                case PetZombie.BlockType.BrainInBank:
                    fileName = "Images/jar";
                    break;
                case PetZombie.BlockType.BrainInCrackedBank:
                    fileName = "Images/cracked_bank";
                    break;
            }

            if (block.Cage)
                fileName += "_cage";

            Sprite = new CCSprite(fileName);

            var x = size.Width*Sprite.AnchorPoint.X + size.Width*block.Position.ColumnIndex;
            var y = size.Width*Sprite.AnchorPoint.Y + size.Width*block.Position.RowIndex;

            Sprite.Position = new CCPoint(x, y);
            Sprite.ScaleTo(size);

            this.Size = size;
        }

        public static CCPoint GetPosition(PetZombie.Block block, CCSize size)
        {
            return new Block(block, size).Sprite.Position;
        }

        private CCRect GetWorldRectangle()
        {
            var x = Sprite.PositionWorldspace.X - Size.Width*Sprite.AnchorPoint.X;
            var y = Sprite.PositionWorldspace.Y - Size.Height*Sprite.AnchorPoint.Y;

            return new CCRect(x, y, Size.Width, Size.Height);
        }
    }
}

