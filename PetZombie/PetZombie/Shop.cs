using System;

namespace PetZombie
{
    public class Shop
    {
        User user;

        public User User
        {
            get{return this.user; }
        }

        public Shop(User user)
        {
            this.user = user;
        }

        public void Buy(Weapon weapon)
        {
            user.Money -= weapon.Cost;
            foreach (Weapon w in user.Weapon)
            {
                if (w is Soporific && weapon is Soporific)
                    w.Count++;
                else
                {
                    if (w is Bomb && weapon is Bomb)
                        w.Count++;
                    else
                    {
                        if (w is Gun && weapon is Gun)
                            w.Count++;
                    }
                }
            }
        }

        public bool CanBuy(Weapon weapon)
        {
            return weapon.Cost < user.Money;
        }
    }
}

