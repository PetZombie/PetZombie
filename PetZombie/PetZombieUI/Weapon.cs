using System;
using CocosSharp;

namespace PetZombieUI
{
    public interface Weapon
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

