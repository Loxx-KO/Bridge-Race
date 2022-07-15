using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public interface IPlayer
    {
        public void CollectBlock(IBlockController block);
        public void DropBlocks(IBlockController block);
        public void PlaceBlock(IBlockController block);
    }

