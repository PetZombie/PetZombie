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

        private Block FindBlock(PetZombie.Block block)
        {
            var foundBlock = this.Blocks.Find(b => 
            {
                return (b.Position == block.Position) ? true : false;
            });

            return foundBlock;
        }

        private List<PetZombieUI.Block> GetNeighbors(PetZombieUI.Block block)
        {
            var neighbors = new List<PetZombieUI.Block>();

            foreach (var neighbor in base.GetNeighbors(block))
            {
                var neighborBlock = FindBlock(neighbor);

                if (neighborBlock != null)
                    neighbors.Add(neighborBlock);
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
            var foundBlock = Blocks.Find(block => 
            {
                return (block.WorldRectangle.ContainsPoint(point)) ? true : false;
            });

            return foundBlock;
        }

        public Tuple<List<PetZombieUI.Block>, List<PetZombieUI.Block>> ReplaceBlocks(PetZombieUI.Block block1,
            PetZombieUI.Block block2)
        {
            var tuple = base.ReplaceBlocks(block1, block2);

            if (tuple != null)
            {
                var removedBlocks = new List<PetZombieUI.Block>();
                var movedBlocks = new List<PetZombieUI.Block>();

                foreach (var block in tuple.Item1)
                    removedBlocks.Add(FindBlock(block));

                foreach (var block in tuple.Item2)
                    movedBlocks.Add(FindBlock(block));

                return new Tuple<List<Block>, List<Block>>(removedBlocks, movedBlocks);
            }

            return null;
        }
    }
}

