using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdjustrixBase.Mathematics.Operations;

namespace AdjustrixBase.Mathematics.LinearAlgebra
{
    //todo: remove if found obsolete
    /// <summary>
    /// A class for Single Value Decomposition using decimal values
    /// </summary>
    public static class Decomposer
    {
        /// <summary>
        /// Computes the singular value decomposition of the given matrix.
        /// The method returns a tuple (U, S, V) where:
        ///   - U is an m x r matrix whose columns are the left singular vectors,
        ///   - S is an r-element array of singular values,
        ///   - V is an n x r matrix whose columns are the right singular vectors.
        /// Here r = min(m, n).
        /// </summary>
        /// <param name="matrix">The m x n matrix to decompose.</param>
        /// <param name="epsilon">The convergence tolerance (default 1E-5).</param>
        /// <param name="maxIterations">The maximum iterations for each power iteration (default 100).</param>
        /// <returns>A tuple (U, S, V) representing the decomposition.</returns>
        public static (decimal[,] U, decimal[] S, decimal[,] V) Decompose(decimal[,] matrix, decimal epsilon = 1e-28m, int maxIterations = 100)
        {
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);
            int numValues = Math.Min(m, n);
            decimal[] sigmas = new decimal[numValues];
            decimal[,] U = new decimal[m, numValues];
            decimal[,] V = new decimal[n, numValues];

            // Work on a copy of the input matrix so that we can "deflate" it.
            decimal[,] remaining = DecimalMatrixOperations.CopyMatrix(matrix);

            // For each singular triplet...
            for (int i = 0; i < numValues; i++)
            {
                // Get the dominant right singular vector of 'remaining' using power iteration.
                decimal[] v = Decompose1D(remaining, epsilon, maxIterations);
                // Compute the corresponding left singular vector as u = matrix * v.
                decimal[] u = DecimalMatrixOperations.MultiplyVector(matrix, v);
                // The singular value is the norm of u.
                decimal sigma = DecimalMatrixOperations.Magnitude(u);

                // If sigma is too small, treat the singular vectors as zero.
                if (sigma < epsilon)
                {
                    u = new decimal[m]; // all zeros
                    v = new decimal[n]; // all zeros
                }
                else
                {
                    // Normalize u so that matrix * v = sigma * u.
                    u = DecimalMatrixOperations.ScaleVector(u, 1 / sigma);
                }

                // Store the computed singular triplet.
                for (int row = 0; row < m; row++)
                {
                    U[row, i] = u[row];
                }
                for (int row = 0; row < n; row++)
                {
                    V[row, i] = v[row];
                }
                sigmas[i] = sigma;

                // Deflate: Remove the rank-1 contribution sigma * (u ⊗ v)
                // (The outer product u ⊗ v forms an m x n matrix.)
                decimal[,] contrib = DecimalMatrixOperations.OuterProduct(u, v);
                // Multiply the outer product by sigma.
                for (int r = 0; r < m; r++)
                {
                    for (int c = 0; c < n; c++)
                    {
                        contrib[r, c] *= sigma;
                    }
                }
                remaining = DecimalMatrixOperations.Subtract(remaining, contrib);
            }

            return (U, sigmas, V);
        }

        /// <summary>
        /// Computes a dominant right singular vector of the input matrix using power iteration.
        /// The routine works on the symmetric matrix b = (matrixᵀ * matrix).
        /// </summary>
        private static decimal[] Decompose1D(decimal[,] matrix, decimal epsilon, int maxIterations)
        {
            int n = matrix.GetLength(1);
            // b = Transpose(matrix) * matrix
            decimal[,] b = DecimalMatrixOperations.Multiply(DecimalMatrixOperations.Transpose(matrix), matrix);

            // Initialize v as a random unit vector.
            decimal[] v = DecimalMatrixOperations.RandomUnitVector(n);

            decimal[] lastIteration = new decimal[n];
            int iterations = 0;
            while (iterations < maxIterations)
            {
                lastIteration = DecimalMatrixOperations.CopyVector(v);
                // Multiply: v = b * lastIteration.
                v = DecimalMatrixOperations.MultiplyVector(b, lastIteration);
                // Normalize v.
                decimal mag = DecimalMatrixOperations.Magnitude(v);
                if (mag > epsilon)
                {
                    v = DecimalMatrixOperations.ScaleVector(v, 1 / mag);
                }
                // Check for convergence (if the change is very small).
                decimal dot = DecimalMatrixOperations.Dot(lastIteration, v);
                if (dot >= 1 - epsilon)
                {
                    break;
                }
                iterations++;
            }
            return v;
        }
    }

    /// <summary>
    /// Helper methods for operations on decimal matrices and vectors.
    /// </summary>
    public static class DecimalMatrixOperations
    {
        public static decimal[,] Transpose(decimal[,] m)
        {
            int rows = m.GetLength(0);
            int cols = m.GetLength(1);
            decimal[,] result = new decimal[cols, rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[j, i] = m[i, j];
                }
            }
            return result;
        }

        public static decimal[,] Multiply(decimal[,] a, decimal[,] b)
        {
            int aRows = a.GetLength(0);
            int aCols = a.GetLength(1);
            int bRows = b.GetLength(0);
            int bCols = b.GetLength(1);
            if (aCols != bRows)
                throw new ArgumentException("Matrix dimensions are not compatible for multiplication.");

            decimal[,] result = new decimal[aRows, bCols];
            for (int i = 0; i < aRows; i++)
            {
                for (int j = 0; j < bCols; j++)
                {
                    decimal sum = 0;
                    for (int k = 0; k < aCols; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                    result[i, j] = sum;
                }
            }
            return result;
        }

        public static decimal[] MultiplyVector(decimal[,] m, decimal[] v)
        {
            int rows = m.GetLength(0);
            int cols = m.GetLength(1);
            if (cols != v.Length)
                throw new ArgumentException("Matrix and vector dimensions are not compatible for multiplication.");

            decimal[] result = new decimal[rows];
            for (int i = 0; i < rows; i++)
            {
                decimal sum = 0;
                for (int j = 0; j < cols; j++)
                {
                    sum += m[i, j] * v[j];
                }
                result[i] = sum;
            }
            return result;
        }

        public static decimal[,] OuterProduct(decimal[] u, decimal[] v)
        {
            int m = u.Length;
            int n = v.Length;
            decimal[,] result = new decimal[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = u[i] * v[j];
                }
            }
            return result;
        }

        public static decimal[,] Subtract(decimal[,] a, decimal[,] b)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);
            if (rows != b.GetLength(0) || cols != b.GetLength(1))
                throw new ArgumentException("Matrix dimensions must match for subtraction.");

            decimal[,] result = new decimal[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = a[i, j] - b[i, j];
                }
            }
            return result;
        }

        public static decimal Dot(decimal[] a, decimal[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vector lengths must match for dot product.");
            decimal sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i] * b[i];
            }
            return sum;
        }

        public static decimal Magnitude(decimal[] a)
        {
            decimal sumSquares = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sumSquares += a[i] * a[i];
            }
            return MathExtensions.Sqrt(sumSquares);
        }

        public static decimal[] ScaleVector(decimal[] a, decimal factor)
        {
            int n = a.Length;
            decimal[] result = new decimal[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = a[i] * factor;
            }
            return result;
        }

        public static decimal[] CopyVector(decimal[] a)
        {
            int n = a.Length;
            decimal[] result = new decimal[n];
            Array.Copy(a, result, n);
            return result;
        }

        public static decimal[,] CopyMatrix(decimal[,] m)
        {
            int rows = m.GetLength(0);
            int cols = m.GetLength(1);
            decimal[,] result = new decimal[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = m[i, j];
                }
            }
            return result;
        }

        /// <summary>
        /// Generates a random unit vector (of type decimal[]) of length n.
        /// </summary>
        public static decimal[] RandomUnitVector(int n)
        {
            decimal[] vec = new decimal[n];
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                // Generate a random number between -1 and 1.
                vec[i] = (decimal)(2 * random.NextDouble() - 1);
            }
            decimal mag = Magnitude(vec);
            if (mag > 0)
            {
                vec = ScaleVector(vec, 1 / mag);
            }
            return vec;
        }
    }
}
