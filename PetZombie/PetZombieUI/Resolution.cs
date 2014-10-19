using System;

using CocosSharp;

namespace PetZombieUI
{
    public static class Resolution
    {
        /*public static CCSize DesignResolution
        {
            get;
            set;
        }*/

        public static CCSize DeviceResolution
        {
            get;
            set;
        }

        private static CCPoint scale;

        public static CCPoint Scale
        {
            get { return scale; }
        }
    }
}

