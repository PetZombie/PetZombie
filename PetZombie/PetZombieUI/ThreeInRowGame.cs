using System;
using System.Collections.Generic;
using CocosSharp;

namespace PetZombieUI
{
    public class ThreeInRowGame : PetZombie.ThreeInRowGame
    {
        private CCSize blockSize;

        public new List<PetZombieUI.Block> Blocks
        {
            get;
            private set;
        }

        public ThreeInRowGame(int rowsCount, int columnsCount, CCSize blockSize) 
            : base(rowsCount, columnsCount, 2, 2, 2)
        {
            Blocks = new List<PetZombieUI.Block>(rowsCount*columnsCount);

            foreach (var row in base.Blocks)
            {
                foreach (var block in row)
                {
                    this.blockSize = blockSize;
                    Blocks.Add(new Block(block, blockSize));
                }
            }
        }

        private List<PetZombieUI.Block> GetNeighbors(PetZombieUI.Block block)
        {
            var neighbors = new List<PetZombieUI.Block>();

            foreach (var neighbor in base.GetNeighbors(block))
            {
                neighbors.Add(new PetZombieUI.Block(neighbor, blockSize));
            }

            return neighbors;
        }

        public PetZombieUI.Block GetReplacedBlock(PetZombieUI.Block block, CCPoint position)
        {
            foreach (var neighbor in GetNeighbors(block))
            {
                if (neighbor.WorldRectangle.ContainsPoint(position))
                {
                    return neighbor;
                }
            }

            return null;
        }

        public Block FindBlockAt(CCPoint point)
        {
            var foundBlock = Blocks.Find(
                block => 
            {
                if (block.WorldRectangle.ContainsPoint(point))
                    return true;

                return false;
            }
            );

            return foundBlock;
        }
    }
}

