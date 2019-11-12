using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib
{
    public class DoubleMatrix
    {

        private Double[][] backArray;
        static DoubleMatrix CreateDoubleMatrix(params Int32[] shape)
        {
            if (shape.Length != 2)
                throw new ArgumentException("Matrices must be two - dimensional");
            if ((shape[0] < 1) || (shape[1] < 1))
                throw new ArgumentException("Number of dimension must be greater than zero");
            return new DoubleMatrix(shape[0], shape[1]);
        }
        public static DoubleMatrix InitializeDoubleMatrix(Double[][] foodArr)
        {
            Int32 arrLength = foodArr[0].Length;
            for (Int32 i = 1; i < foodArr.GetLength(0); ++i)
            {
                if (foodArr[i].Length != arrLength)
                    throw new ArgumentException("The array must be rectangular");
            }

            DoubleMatrix resultDoubleMatrix = DoubleMatrix.CreateDoubleMatrix(foodArr.GetLength(0), foodArr[0].Length);
            FeedDoubleMatrix(resultDoubleMatrix, foodArr);
            return resultDoubleMatrix;
        }
        static void FeedDoubleMatrix(DoubleMatrix sourceDoubleMatrix, Double[][] foodArr)
        {
            for (Int32 i = 0; i < sourceDoubleMatrix.Shape[0]; ++i)
            {
                for (Int32 j = 0; j < sourceDoubleMatrix.Shape[1]; ++j)
                {
                    sourceDoubleMatrix[i][j] = foodArr[i][j];
                }
            }
        }

        private DoubleMatrix(Int32 rows, Int32 cols)
        {
            backArray = new Double[rows][];
            for (Int32 i = 0; i < rows; ++i)
            {
                backArray[i] = new Double[cols];
            }
        }

        public Double this[Int32 row, Int32 col]
        {
            get
            {
                return backArray[row][col];
            }
            set
            {
                backArray[row][col] = value;
            }
        }
        public Double[] this[Int32 row, Boolean isRow = true]
        {
            get
            {
                if (isRow)
                {
                    return backArray[row];
                }
                else
                {
                    Double[] column = new Double[backArray[0].Length];
                    for (Int32 i = 0; i < backArray.GetLength(0); ++i)
                    {
                        column[i] = backArray[i][row];
                    }
                    return column;
                }
            }
        }


        public Int32[] Shape
        {
            get
            {
                Int32[] shape = new Int32[2] { backArray.GetLength(0), backArray[0].Length };
                return shape;
            }
        }
        
        public static DoubleMatrix Dot(DoubleMatrix leftDoubleMatrix, DoubleMatrix rightDoubleMatrix)
        {
            if (leftDoubleMatrix.Shape[1] != rightDoubleMatrix.Shape[0])
                throw new ArgumentException("Shapes of matrices aren`t aligned");
            DoubleMatrix resultDoubleMatrix = new DoubleMatrix(leftDoubleMatrix.Shape[0], rightDoubleMatrix.Shape[1]);
            rightDoubleMatrix = DoubleMatrix.Transpose(rightDoubleMatrix);


            Parallel.For(0, leftDoubleMatrix.Shape[0], row =>
            {
                for (Int32 col = 0; col < rightDoubleMatrix.Shape[0]; ++col)
                {
                    resultDoubleMatrix[row, col] = DoubleMatrix.VecMul(leftDoubleMatrix[row], rightDoubleMatrix[col]);
                }
            });

            return resultDoubleMatrix;
        }

        private static Double VecMul(Double[] vec0, Double[] vec1)
        {
            if (vec0.Length != vec1.Length)
                throw new ArgumentException("Vectors must be the same size for multiplication");
            if (vec0.Length < 4)
                return vec0[0] * vec1[0] + vec0[1] * vec1[1] + vec0[2] * vec1[2];

            Int32 vectorSize = Vector<Double>.Count;
            Int32 ptr = vectorSize;

            var tmpVector0 = new Vector<Double>(vec0);
            var tmpVector1 = new Vector<Double>(vec1);
            Double result = Vector.Dot(tmpVector0, tmpVector1);

            for (ptr = vectorSize; ptr < (vec0.Length - vectorSize + 1); ptr += vectorSize)
            {
                tmpVector0 = new Vector<Double>(vec0, ptr);
                tmpVector1 = new Vector<Double>(vec1, ptr);
                result += Vector.Dot(tmpVector0, tmpVector1);
            }

            for (; ptr < vec0.Length; ptr++)
            {
                result += vec0[ptr] * vec1[ptr];
            }

            return result;
        }
        public static DoubleMatrix Transpose(DoubleMatrix sourceDoubleMatrix)
        {
            Int32[] shape = sourceDoubleMatrix.Shape;
            Array.Reverse(shape);
            DoubleMatrix resultDoubleMatrix = DoubleMatrix.CreateDoubleMatrix(shape);
            for (Int32 row = 0; row < shape[0]; ++row)
            {
                for (Int32 col = 0; col < shape[1]; ++col)
                {
                    resultDoubleMatrix[row, col] = sourceDoubleMatrix[col, row];
                }
            }
            return resultDoubleMatrix;
        }

        public override String ToString()
        {
            StringBuilder ret = new StringBuilder();
            for (Int32 i = 0; i < backArray.GetLength(0); ++i)
            {
                for (Int32 j = 0; j < backArray[0].Length; ++j)
                {
                    ret.Append(backArray[i][j]);
                    ret.Append("\t");
                }
                ret.Append("\n");
            }
            return ret.ToString();
        }

    }
}
