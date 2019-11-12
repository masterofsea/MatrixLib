using System;
using System.Numerics;
using System.Text;

namespace MatrixLib
{
    public class Matrix
    {

        private Int32[][] backArray;
        static Matrix CreateMatrix(params Int32[] shape)
        {
            if (shape.Length != 2)
                throw new ArgumentException("Matrices must be two - dimensional");
            if ((shape[0] < 1) || (shape[1] < 1))
                throw new ArgumentException("Number of dimension must be greater than zero");
            return new Matrix(shape[0], shape[1]);
        }
        public static Matrix InitializeMatrix(Int32[][] foodArr)
        {
            Int32 arrLength = foodArr[0].Length;
            for (Int32 i = 1; i < foodArr.GetLength(0); ++i)
            {
                if (foodArr[i].Length != arrLength)
                    throw new ArgumentException("The array must be rectangular");
            }

            Matrix resultMatrix = Matrix.CreateMatrix(foodArr.GetLength(0), foodArr[0].Length);
            FeedMatrix(resultMatrix, foodArr);
            return resultMatrix;
        }
        static void FeedMatrix(Matrix sourceMatrix, Int32[][] foodArr)
        {
            for(Int32 i = 0; i < sourceMatrix.Shape[0]; ++i)
            {
                for (Int32 j = 0; j < sourceMatrix.Shape[1]; ++j)
                {
                    sourceMatrix[i][j] = foodArr[i][j];
                }
            }            
        }

        private Matrix(Int32 rows, Int32 cols)
        {
            backArray = new int[rows][];
            for(Int32 i = 0; i < rows; ++i)
            {
                backArray[i] = new Int32[cols];
            }
        }

        public Int32 this[Int32 row, Int32 col]
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
        public Int32[] this[Int32 row, Boolean isRow = true]
        {
            get 
            {
                if (isRow)
                {
                    return backArray[row];
                }
                else
                {
                    Int32[] column = new Int32[backArray[0].Length];
                    for(Int32 i = 0; i < backArray.GetLength(0); ++i)
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

        public static Matrix Dot(Matrix leftMatrix, Matrix rightMatrix)
        {
            if (leftMatrix.Shape[1] != rightMatrix.Shape[0])
                throw new ArgumentException("Shapes of matrices aren`t aligned");            
            Matrix resultMatrix = new Matrix(leftMatrix.Shape[0], rightMatrix.Shape[1]);
            rightMatrix = Matrix.Transpose(rightMatrix);
            

            for (Int32 row = 0; row < leftMatrix.Shape[0]; ++row)
            {
                for (Int32 col = 0; col < rightMatrix.Shape[0]; ++col)
                {
                    resultMatrix[row, col] = Matrix.VecMul(leftMatrix[row], rightMatrix[col]);
                }
            }

            return resultMatrix;
        }

        private static Int32 VecMul(Int32[] vec0, Int32[] vec1)
        {
            if (vec0.Length != vec1.Length) 
                throw new ArgumentException("Vectors must be the same size for multiplication");
            if (vec0.Length < 4) 
                return vec0[0] * vec1[0] + vec0[1] * vec1[1] + vec0[2] * vec1[2];

            Int32 vectorSize = Vector<int>.Count;
            Int32 ptr = vectorSize;

            var tmpVector0 = new Vector<Int32>(vec0);
            var tmpVector1 = new Vector<Int32>(vec1);            
            Int32 result = Vector.Dot(tmpVector0, tmpVector1);

            for (ptr = vectorSize; ptr < (vec0.Length - vectorSize + 1); ptr += vectorSize)
            {
                tmpVector0 = new Vector<Int32>(vec0, ptr);
                tmpVector1 = new Vector<Int32>(vec1, ptr);
                result += Vector.Dot(tmpVector0, tmpVector1);
            }

            for (; ptr < vec0.Length; ptr++)
            {
                result += vec0[ptr] * vec1[ptr];
            }

            return result;
        }
        public static Matrix Transpose(Matrix sourceMatrix)
        {
            Int32[] shape = sourceMatrix.Shape;
            Array.Reverse(shape);
            Matrix resultMatrix = Matrix.CreateMatrix(shape);
            for (Int32 row = 0; row < shape[0]; ++row){
                for(Int32 col = 0; col < shape[1]; ++col)
                {
                    resultMatrix[row, col] = sourceMatrix[col, row];
                }
            }
            return resultMatrix;
        }

        public override String ToString()
        {
            StringBuilder ret = new StringBuilder();
            for(Int32 i = 0; i < backArray.GetLength(0); ++i)
            {
                for(Int32 j = 0; j < backArray[0].Length; ++j)
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
