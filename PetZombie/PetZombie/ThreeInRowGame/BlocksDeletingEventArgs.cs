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
        List<Block> initPositionsOfNewBlocks;

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

        public List<Block> InitPositionsOfNewBlocks
        {
            get{ return this.initPositionsOfNewBlocks; }
        }

        public BlocksDeletingEventArgs(List<Block> delBlocks, List<Block> prevMovBlocks, List<Block> curMovBlocks)
        {
            this.delBlocks = new List<Block>(delBlocks);
            this.prevMovBlocks = new List<Block>(prevMovBlocks);
            this.curMovBlocks = new List<Block>(curMovBlocks);
        }
    }
}

