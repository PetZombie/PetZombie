using System;

namespace PetZombie
{
	public abstract class Weapon
	{
		public int count;

		public Weapon (int count=1)
		{
			this.count = count;
		}

		abstract public void Use ();
	}
}

