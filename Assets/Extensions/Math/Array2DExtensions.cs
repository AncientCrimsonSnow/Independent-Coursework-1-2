using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Extensions.Math
{
    public static class Array2DExtensions
    {
        private static readonly int DataXY = Shader.PropertyToID("$_Data[{x}/{y] + ]");

        public static int GetSingleIndex(int x, int y, int sizeX)
        {
            return sizeX * y + x;
        }

        public static int2 Get2DIndex(int i, int sizeX)
        {
            return new int2 { x = i % sizeX, y = i / sizeX };
        }
        
        public static T GetElementAt<T>(this T[,] array, int x, int y)
        {
            return array[y, x];
        }
        public static void SetElementAt<T>(this T[,] array, int x, int y, T value)
        {
            array[y, x] = value;
        }
        
        public static bool IsValidIndex<T>(this T[,] array, int2 pos){
            return  array.IsValidIndex(pos.x, pos.y);
                
        }

        public static bool IsValidIndex<T>(this T[,] array, int x, int y)
        {
            return x >= 0 &&
                   y >= 0 &&
                   x < array.GetLength(1) &&
                   y < array.GetLength(0);
        }

        public static T GetElementAt<T>(this T[,] array, int2 pos)
        {
            return array.GetElementAt(pos.x, pos.y);
        }
        
        public static void SetElementAt<T>(this T[,] array, int2 pos, T value)
        {
            array[pos.y, pos.x] = value;
        }

        public static void PrintOnRenderTexture(this int[,] data, RenderTexture renderTexture)
        {
            var texture = new Texture2D(data.GetLength(0), data.GetLength(1));
            var max = data.Cast<int>().Prepend(0).Max();


            for (var y = 0; y < data.GetLength(0); y++)
            {
                for (var x = 0; x < data.GetLength(1); x++)
                {
                    var value = data[x, y]/(float)max;
                    texture.SetPixel(x, y, new Color(value, value, value));
                }
            }
            texture.Apply();
            Graphics.Blit(texture, renderTexture);
        }
        
        public static T[] Convert2DTo1D<T>(T[,] array)
        {
            var data = new T[array.GetLength(0) * array.GetLength(1)];
            for (var y = 0; y < array.GetLength(0); y++)
            {
                for (var x = 0; x < array.GetLength(1); x++)
                {
                    data[y * array.GetLength(0) + x] = array[x, y];
                }
            }

            return data;
        }
        
        public static T[,] Convert1DTo2D<T>(this T[] inputArray, int width, int height)
        {
            var result = new T[height, width];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    result.SetElementAt(x,y, inputArray[y * width + x]);
                }
            }
            return result;
        }
    }
}