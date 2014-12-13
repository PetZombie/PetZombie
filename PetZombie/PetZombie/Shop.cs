using System;

namespace PetZombie
{
    public class Shop
    {
        User user;

        public Shop(User user)
        {
            this.user = user;
        }

        public void Buy(Weapon weapon)
        {
            user.Money -= weapon.Cost;
        }

        public bool CanBuy(Weapon weapon)
        {
            return weapon.Cost < user.Money;
        }
    }
}

