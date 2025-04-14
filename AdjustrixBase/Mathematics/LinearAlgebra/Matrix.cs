using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf.Content.Objects;

namespace AdjustrixBase.Mathematics.LinearAlgebra
{
    public class Matrix
    {
        private decimal[,] data;

        public int Rows { get; }

        public int Columns { get; }

        public Matrix(int rows, int columns)
        {
            if (rows <= 0 || columns <= 0) throw new ArgumentException("Matrix dimensions must be positive");
            data = new decimal[rows, columns];
            Rows = rows; 
            Columns = columns;
        }

        public decimal this[int row, int column]
        {
            get { return data[row, column]; } 
            set { data[row, column] = value; }
        }

        public Matrix Multiply(Matrix other)
        {
            if (this.Columns != other.Rows)
                throw new ArgumentException("Matrix dimensions are not compatible for multiplication");
            Matrix result = new(this.Rows, other.Columns);
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < other.Columns; j++)
                {
                    decimal sum = 0;
                    for (int k = 0; k < this.Columns; k++)
                    {
                        sum += this[i, k] * other[k, j];
                    }
                    result[i, j] = sum;
                }
            }
            return result;
        }

        /// <summary>
        /// Multiplies this matrix by a vector.
        /// </summary>
        public Vector Multiply(Vector v)
        {
            if (this.Columns != v.Size)
                throw new ArgumentException("Matrix and Vector dimensions are not compatible for multiplication");
            Vector result = new(this.Rows);
            for (int i = 0; i < this.Rows; i++)
            {
                decimal sum = 0;
                for (int j = 0; j < this.Columns; j++)
                {
                    sum += this[i, j] * v[j];
                }
                result[i] = sum;
            }
            return result;
        }

        /// <summary>
        /// Returns the transpose of this matrix.
        /// </summary>
        public Matrix Transpose()
        {
            Matrix result = new(Columns, Rows);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result[j, i] = this[i, j];
                }
            }
            return result;
        }

        public Matrix PseudoInverse()
        {
            // To compute the SVD of the matrix to find Sigma.
            var (u, s, v) = Decomposer.Decompose(data);

            // To take the reciprocal of each non-zero element on the diagonal.
            var len = s.Length;

            var sigma = new decimal[len];
            for (var i = 0; i < len; i++)
            {
                sigma[i] = Math.Abs(s[i]) < (decimal)1e-28 ? (decimal)0 : (decimal)(1 / s[i]);
            }

            // To construct a diagonal matrix based on the vector result.
            var diag = ToDiagonalMatrix(sigma);

            // To construct the pseudo-inverse using the computed information above.
            var matinv = ToMatrix(u).Multiply(diag).Multiply(ToMatrix(v).Transpose());

            // To Transpose the result matrix.
            return matinv.Transpose();
        }

        public Vector Row(int i)
        {
            Vector v = new(Columns);
            for (int col = 0; col < Columns; col++)
            {
                v[col] = this[i, col];
            }
            return v;
        }
        private static Matrix ToDiagonalMatrix(decimal[] data)
        {
            Matrix m = new(data.Length, data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    if (i == j)
                    {
                        m[i, j] = data[i];
                        continue;
                    }
                    m[i, j] = 0;
                }
            }
            return m;
        }

        private static Matrix ToMatrix(decimal[,] data)
        {
            Matrix m = new(data.GetLength(0), data.GetLength(1));
            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Columns; j++)
                {
                    m[i, j] = data[i, j];
                }
            }
            return m;
        }

        public static Matrix operator *(Matrix left, decimal right)
        {
            Matrix m = new(left.Rows, left.Columns);
            for (int i = 0; i < left.Rows; i++)
            {
                for (int j = 0; j < left.Columns; j++)
                {
                    m[i, j] = left[i, j] * right;
                }
            }
            return m;
        }

        public static Matrix operator *(decimal left, Matrix right)
        {
            Matrix m = new(right.Rows, right.Columns);
            for (int i = 0; i < right.Rows; i++)
            {
                for (int j = 0; j < right.Columns; j++)
                {
                    m[i, j] = right[i, j] * left;
                }
            }
            return m;
        }

        public void PrintMatrix()
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    Console.Write($"{data[i, j]} ");
                }
                Console.WriteLine();
            }
        }

        public void SaveToFile(string filePath)
        {
            string str = "";
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    str += $"{data[i, j]} ";
                }
                str += "\n";
            }
            File.WriteAllText(filePath, str);
        }
    }
}
