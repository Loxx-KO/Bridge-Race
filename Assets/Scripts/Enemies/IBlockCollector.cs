using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IBlockCollector
{
    public void DropBlocks(Vector3 direction);
    public void PlaceBlock(SlopeStepController step);
    public void CollectBlock(BlockController block);
}

