using System;

namespace PetZombie
{
    public class DataServiceFactory
    {
        static IDataService data;
        public static IDataService DataService()
        {
            return data;
        }

        public static void ChangeDataService(IDataService newDataService)
        {
            data = newDataService;
        }
    }
}

