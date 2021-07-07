using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Model
{
    public class Chunk
    {
        public int sizeX = 0;
        public int sizeY = 0;
        public List<BlockType> blocks = new List<BlockType>();
    }
}
