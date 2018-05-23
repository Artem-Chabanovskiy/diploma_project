using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaProject.Helpers
{
    internal static class MatrixHelper
    {
        public static float[][] MatCreate(int rows, int cols)
        {
            float[][] result = new float[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new float[cols];
            return result;
        }

        public static float[][] MatFromArray(float[] arr, int rows, int cols)
        {
            float[][] result = MatCreate(rows, cols);
            int k = 0;
            for (int i = 0; i < rows; ++i)
            for (int j = 0; j < cols; ++j)
                result[i][j] = arr[k++];
            return result;
        }

        public static float[][] MatTanh(float[][] m)
        {
            int rows = m.Length; int cols = m[0].Length;
            float[][] result = MatCreate(rows, cols);
            for (int i = 0; i < rows; ++i) // Each row
            for (int j = 0; j < cols; ++j) // Each col
                result[i][j] = Tanh(m[i][j]);
            return result;
        }

        //multiplies corresponding values in two matrices that have the same shape
        public static float[][] MatHada(float[][] a, float[][] b)
        {
            int rows = a.Length; int cols = a[0].Length;
            float[][] result = MatCreate(rows, cols);
            for (int i = 0; i < rows; ++i)
            for (int j = 0; j < cols; ++j)
                result[i][j] = a[i][j] * b[i][j];
            return result;
        }

        private static float Tanh(float x)
        {
            if (x < -10.0) return -1.0f;
            if (x > 10.0) return 1.0f;
            return (float)(Math.Tanh(x));
        }

        private static float Sigmoid(float x)
        {
            if (x < -10.0) return -1.0f;
            if (x > 10.0) return 1.0f;
            float k = (float)Math.Exp(-x);
            return 1 / (1 + k);
        }
    }
}
