using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class ThreeInRowGame : IGame
	{
		List<List<Block>> blocks;
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

		public void MoveBlocks (int row1, int column1, int row2, int column2)
		{
			try {
				Block block1 = this.blocks [row1] [column1];
				Block block2 = this.blocks [row2] [column2];
				if (this.DeleteCheck ())
					this.DeleteBlocks ();
			} catch (Exception e) {
			}
		}

		private bool DeleteCheck ()
		{
			return false;
		}

		private void DeleteBlocks ()
		{
		}

		public void UseWeapon (Weapon weapon, int row, int column)
		{
			try {
				Block block = this.blocks [row] [column];
				weapon.Use ();
			} catch (Exception e) {
			}
		}

	}
}

