using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class ThreeInRowGame : IGame
	{
		List<Block> blocks;
		public int target;
		public int stepsCount;
		List<Weapon> weapons;

		public ThreeInRowGame (int blocksCount = 60, int target = 5, int steps = 25)
		{
			this.blocks = GenerateBlock (blocksCount);
			this.target = target;
			this.stepsCount = steps;
			this.weapons = new List<Weapon> ();
		}

		private List<Block> GenerateBlock (int count)
		{
			return new List<Block> ();
		}

		public void MoveBlocks (Coordinates coord1, Coordinates coord2)
		{
			Block block1 = blocks.Find (delegate (Block block) {
				return block.Coordinates.x == coord1.x && block.Coordinates.y == coord1.y;
			});
			Block block2 = blocks.Find (delegate (Block block) {
				return block.Coordinates.x == coord2.x && block.Coordinates.y == coord2.y;
			});
			if (block1 != null && block2 != null) {
				if (this.DeleteCheck ())
					this.DeleteBlocks ();
			}
		}
			
		private bool DeleteCheck ()
		{
			return false;
		}

		private void DeleteBlocks ()
		{
		}

		public void UseWeapon (Coordinates weaponCoord, Coordinates blockCoord)
		{
			Weapon curWeapon = null;
			foreach (Weapon weapon in this.weapons) 
			{
				if (weaponCoord.x >= weapon.Coordinates.x && weaponCoord.x <= weapon.Coordinates.x + weapon.Size) {
					curWeapon = weapon;
					break;
				}
			}
			Block block = this.blocks.Find (delegate (Block b) {
				return b.Coordinates.x == blockCoord.x && b.Coordinates.y == blockCoord.y;
			});

			if (curWeapon != null && block != null) 
			{
			}
		}

		private void GetWeaponType(Coordinates weaponCoord)
		{

		}
	}
}

