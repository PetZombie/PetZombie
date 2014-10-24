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

        private CCPoint GetPriorityDirection(Block block, CCPoint delta)
        {
            float additionValue;
            var absX = Math.Abs(delta.X);
            var absY = Math.Abs(delta.Y);

            if (absX > absY)
            {
                if (delta.X > 0)
                    additionValue = block.Size.Width;
                else
                    additionValue = -block.Size.Width;

                return new CCPoint(delta.X + additionValue, 0);
            }
            else if (absY > absX)
            {
                if (delta.Y > 0)
                    additionValue = block.Size.Height;
                else
                    additionValue = -block.Size.Height;

                return new CCPoint(0, delta.Y + additionValue);
            }

            return new CCPoint();
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

