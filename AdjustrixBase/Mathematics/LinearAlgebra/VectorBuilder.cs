using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustrixBase.Mathematics.LinearAlgebra
{
    public class VectorBuilder
    {
        public Vector Dense(int size)
        {
            return new Vector(size);
        }
    }
}
