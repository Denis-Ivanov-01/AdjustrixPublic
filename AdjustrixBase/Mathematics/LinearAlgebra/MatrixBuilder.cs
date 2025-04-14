using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustrixBase.Mathematics.LinearAlgebra
{
    public class MatrixBuilder
    {
        public Matrix Dense(int rows, int columns)
        {
            return new Matrix(rows, columns);
        }
    }
}
