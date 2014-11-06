using System;

namespace PetZombie
{
	public class Position
	{
		int rowIndex, columnIndex;

		public int RowIndex
		{
			get{ return this.rowIndex;}
		}

		public int ColumnIndex
		{
			get{ return this.columnIndex;}
		}

		public Position (int rowIndex, int columnIndex)
		{
			this.rowIndex = rowIndex;
			this.columnIndex = columnIndex;
		}

		public Position ()
		{
			this.rowIndex = 0;
			this.columnIndex = 0;
		}
	}
}

