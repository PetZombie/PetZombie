using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class BlocksDeletingEventArgs : EventArgs
    {
        List<Block> delBlocks;
        List<Block> prevMovBlocks;
        List<Block> curMovBlocks;
        List<Block> newBlocks;

        public List<Block> DelBlocks
        {
            get{ return this.delBlocks; }
        }

        public List<Block> PrevMovBlocks
        {
            get{ return this.prevMovBlocks; }
        }

        public List<Block> CurMovBlocks
        {
            get{ return this.curMovBlocks; }
        }

        public List<Block> NewBlocks
        {
            get{ return this.newBlocks; }
        }

        public BlocksDeletingEventArgs(List<Block> delBlocks, List<Block> prevMovBlocks, List<Block> curMovBlocks, List<Block> newBlocks)
        {
            this.delBlocks = new List<Block>(delBlocks);
            this.prevMovBlocks = new List<Block>(prevMovBlocks);
            this.curMovBlocks = new List<Block>(curMovBlocks);
            this.newBlocks = new List<Block>(newBlocks);
        }
    }
}

