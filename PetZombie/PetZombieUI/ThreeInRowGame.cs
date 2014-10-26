using System;
using System.Collections.Generic;
using CocosSharp;

namespace PetZombieUI
{
    public class ThreeInRowGame : PetZombie.ThreeInRowGame
    {
        private CCSize blockSize;

        public new List<Block> Blocks
        {
            get;
            private set;
        }

        public ThreeInRowGame(int rowsCount, int columnsCount, CCSize blockSize) 
            : base(rowsCount, columnsCount, 2, 2, 2)
        {
            Blocks = new List<Block>(rowsCount*columnsCount);

            foreach (var row in base.Blocks)
            {
                foreach (var block in row)
                    Blocks.Add(new Block(block, blockSize));
            }

            this.blockSize = blockSize;
        }

        /*public void UpdateBlocks()
        {
            Blocks.Clear();

            foreach (var row in base.Blocks)
            {
                foreach (var block in row)
                {
                    var newBlock = FindBlock(block);

                    if (newBlock != null)
                        Blocks.Add(newBlock);
                    else
                        Blocks.Add(new Block(block, blockSize));
                }
            }
        }*/

        private Block FindBlock(PetZombie.Block block)
        {
            var foundBlock = Blocks.Find(b => 
            {
                return (b.Position == block.Position) ? true : false;
            });

            return foundBlock;
        }

        private List<Block> GetNeighbors(Block block)
        {
            var neighbors = new List<Block>();

            foreach (var neighbor in base.GetNeighbors(block))
            {
                var neighborBlock = FindBlock(neighbor);

                if (neighborBlock != null)
                    neighbors.Add(neighborBlock);
            }

            return neighbors;
        }

        public Block GetReplacedBlock(Block block, CCPoint position)
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

        public Tuple<List<Block>, List<Block>, List<Block>, int> ReplaceBlocks(Block block1, Block block2)
        {
            var tuple = base.ReplaceBlocks(block1, block2);

            if (tuple != null)
            {
                var removedBlocks = new List<Block>();
                var movedBlocks = new List<Block>();
                var newBlocks = new List<Block>();

                foreach (var block in tuple.Item1)
                    removedBlocks.Add(FindBlock(block));

                foreach (var block in tuple.Item2)
                    movedBlocks.Add(FindBlock(block));

                foreach (var block in tuple.Item3)
                    newBlocks.Add(new Block(block, blockSize));

                return new Tuple<List<Block>, List<Block>, List<Block>, int>(removedBlocks, movedBlocks, newBlocks, tuple.Item4);
            }

            return null;
        }
    }
}

