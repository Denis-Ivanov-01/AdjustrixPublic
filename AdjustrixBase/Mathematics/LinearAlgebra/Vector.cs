using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel.Internals;

namespace AdjustrixBase.Mathematics.LinearAlgebra
{
    public class Vector : IEnumerable<decimal>
    {
        private decimal[] data;
        public int Size { get; }

        /// <summary>
        /// Constructs a vector of the specified size.
        /// </summary>
        public Vector(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Vector size must be positive.");
            Size = size;
            data = new decimal[size];
        }

        /// <summary>
        /// Indexer for accessing vector elements.
        /// </summary>
        public decimal this[int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }

        /// <summary>
        /// Multiplies this vector (treated as a column vector) by another vector (computes the dot product).
        /// </summary>
        public decimal Multiply(Vector other)
        {
            if (this.Size != other.Size)
                throw new ArgumentException("Vectors must be of the same size for dot product.");
            decimal sum = 0;
            for (int i = 0; i < Size; i++)
            {
                sum += this[i] * other[i];
            }
            return sum;
        }

        /// <summary>
        /// Multiplies this vector (treated as a row vector) by a matrix.
        /// (Thus, the vector’s size must equal the matrix’s row count.)
        /// </summary>
        public Vector Multiply(Matrix m)
        {
            if (this.Size != m.Rows)
                throw new ArgumentException("Vector size must match matrix row count for multiplication (vector treated as row vector).");
            Vector result = new Vector(m.Columns);
            for (int j = 0; j < m.Columns; j++)
            {
                decimal sum = 0;
                for (int i = 0; i < this.Size; i++)
                {
                    sum += this[i] * m[i, j];
                }
                result[j] = sum;
            }
            return result;
        }

        /// <summary>
        /// Computes the dot product of this vector with another vector.
        /// The vectors must have the same size.
        /// </summary>
        public decimal DotProduct(Vector other)
        {
            if (this.Size != other.Size)
                throw new ArgumentException("Vectors must have the same length");

            decimal result = 0;
            for (int i = 0; i < this.Size; i++)
            {
                result += this[i] * other[i];
            }
            return result;
        }

        public static Vector operator -(Vector v)
        {
            Vector vector = new(v.Size);
            for (int i = 0; i < v.Size; i++)
            {
                vector[i] = -v[i];
            }
            return vector;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the elements of the vector.
        /// </summary>
        public IEnumerator<decimal> GetEnumerator()
        {
            foreach (var value in data)
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
