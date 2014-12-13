using System;
//using SQLite;

namespace PetZombie
{
    public class ZombiePet
    {
        int satiety;
        int maxSat;
        int satietyIncrement;
        string name;
        State currentState;

        public int Satiety
        {
            get{return this.satiety; }
        }

        public string Name
        {
            get{ return this.name; }
        }

        public State CurrentState
        {
            get
            { 
                if (this.satiety > 70)
                    return State.Happy;
                if (this.satiety > 30)
                    return State.Normal;
                return State.Angry;
            }
        }

        public ZombiePet(string name, int satiety=100, int satietyIncrement=20, int maxSat=100, State state=State.Happy)
        {
            this.satiety = satiety;
            this.maxSat = maxSat;
            this.name = name;
            this.satietyIncrement = satietyIncrement;
            this.currentState = state;
        }

        public void Eat()
        {
            this.satiety += satietyIncrement;
        }
    }
}

