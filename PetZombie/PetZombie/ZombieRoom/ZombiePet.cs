using System;
//using SQLite;

namespace PetZombie
{
    public class ZombiePet
    {
        float satiety;
        int maxSat;
        int satietyIncrement;
        string name;
        State currentState;
        public string time;

        public float Satiety
        {
            get
            {
                DateTime now = DateTime.UtcNow;
                var dt = (now - DateTime.Parse(time));
                satiety = satiety - dt.Minutes - dt.Seconds*0.1f;
                time = DateTime.UtcNow.ToString();
                if (satiety < 0)
                    satiety = 0;
                return satiety; 
            }
            set{
                time = DateTime.UtcNow.ToString();
                this.satiety = value;
            }
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

        public ZombiePet(string name, float satiety=100, int satietyIncrement=20, int maxSat=100, State state=State.Happy)
        {
            this.satiety = satiety;
            this.maxSat = maxSat;
            this.name = name;
            this.satietyIncrement = satietyIncrement;
            this.currentState = state;
            time = DateTime.UtcNow.ToString();
        }

        public void Eat()
        {
            if (this.satiety >= 100)
                return;
            this.satiety += satietyIncrement;
        }
    }
}

