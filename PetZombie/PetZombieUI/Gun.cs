using System;
using CocosSharp;

namespace PetZombieUI
{
    public class Gun : PetZombie.Gun, Weapon
    {
        public CCSize Size
        {
            get;
            private set;
        }

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

        public Gun() : base(5)
        {
            Sprite = new CCSprite("Images/aim_bar");
            Size = new CCSize(72, 72);
        }

        private CCRect GetWorldRectangle()
        {
            var x = Sprite.PositionWorldspace.X - Size.Width*Sprite.AnchorPoint.X;
            var y = Sprite.PositionWorldspace.Y - Size.Height*Sprite.AnchorPoint.Y;

            return new CCRect(x, y, Size.Width, Size.Height);
        }
    }
}

