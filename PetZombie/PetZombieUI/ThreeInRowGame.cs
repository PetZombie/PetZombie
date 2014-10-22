using System;
using System.Collections.Generic;
using CocosSharp;

namespace PetZombieUI
{
    public class ThreeInRowGame : PetZombie.ThreeInRowGame
    {
        public new List<PetZombieUI.Block> Blocks
        {
            get;
            private set;
        }

        public ThreeInRowGame(int rowsCount, int columnsCount, CCSize blockSize) 
            : base(rowsCount, columnsCount)
        {
            Blocks = new List<PetZombieUI.Block>(rowsCount*columnsCount);

            foreach (var row in base.Blocks)
            {
                foreach (var block in row)
                {
                    Blocks.Add(new Block(block, blockSize));
                }
            }
        }
    }
}

