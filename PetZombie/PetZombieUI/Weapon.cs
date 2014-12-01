using System;
using CocosSharp;

namespace PetZombieUI
{
    public interface Weapon : PetZombie.Weapon
    {
        CCSprite Sprite
        {
            get;
        }

        CCRect WorldRectangle
        {
            get ;
        }
    }
}

