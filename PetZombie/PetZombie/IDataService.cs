using System;

namespace PetZombie
{
    public interface IDataService
    {
        void Write(User user);
        User Read();
    }
}

