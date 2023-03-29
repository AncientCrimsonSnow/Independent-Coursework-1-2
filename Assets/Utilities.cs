using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utilities
{
    public static Vector2 GetRandomPosInDonut(Vector2 center, float minRadius, float maxRadius)
    {
        var maxDiameter = maxRadius * 2;

        Vector2 result;
        float distanceSquaredToCenter;
        do
        {
            result = GetRandomPosInRect(center, new Vector2(maxDiameter, maxDiameter));
            distanceSquaredToCenter = result.DistanceSquared(center);
        }
        while (distanceSquaredToCenter > maxRadius * maxRadius || distanceSquaredToCenter < minRadius * minRadius);
        
        return result;
    }

    public static Vector2 GetRandomPosInRect(Vector2 center, Vector2 size)
    {
        return new Vector2( 
            Random.Range(-size.x/2, size.x/2) + center.x,
            Random.Range(-size.y/2, size.y/2) + center.y);
    }

    public static Vector3 ToVector3(this Vector2 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }
    
    public static float DistanceSquared(this Vector2 v, Vector2 v1)
    {
        var xDelta = v1.x - v.x;
        var yDelta = v1.y - v.y;
            
        return xDelta * xDelta + yDelta * yDelta;
    }
    
    public static Vector2[] GetEvenlySpacedPositions(Vector2 topLeftCorner, Vector2 botRightCorner, int count)
    {
        var positions = new Vector2[count];
        
        var center = (topLeftCorner + botRightCorner) / 2;
        
        var width = botRightCorner.x - topLeftCorner.x;
        var height = topLeftCorner.y - botRightCorner.y;
        
        var gridSize = GetGridSize(width, height, count);

        // Bestimme die Anzahl der Zeilen und Spalten, so dass der Abstand zwischen den Punkten so groß wie möglich ist
        var rows = (int)Mathf.Sqrt(count);
        var columns = count / rows;

        // Berechne das x- und y-Padding aus dem Abstand zwischen den Punkten
        var xPadding = width / gridSize.x;
        var yPadding = height / gridSize.y;

        // Bestimme die Startposition in x- und y-Richtung
        var startX = center.x - (width / 2) + xPadding;
        var startY = center.y + (height / 2) - yPadding;

        // Fülle das Array mit den berechneten Positionen
        var index = 0;
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                var x = startX + j * xPadding * 2;
                var y = startY - i * yPadding * 2;
                positions[index++] = new Vector2(x, y);
            }
        }

        return positions;
    }

    public static Vector2Int GetGridSize(float width, float height, int count)
    {
        var rows = (int)Mathf.Ceil(Mathf.Sqrt(count * height / width));
        var columns = (int)Mathf.Ceil(Mathf.Sqrt(count * width / height));
        while (rows * columns < count)
        {
            if (rows * columns <= count)
                rows++;
            if (rows * columns <= count)
                columns++;
        }

        while (rows >= 1 && columns >= 1 &&
               (rows - 1) * columns >= count ||
               rows * (columns - 1) >= count)
        {
            if((rows - 1) * columns >= count)
                rows--;
            if(rows * (columns - 1) >= count)
                columns--;
        }
        return new Vector2Int(columns, rows);
    }

    public static int[,] GetClosePositionIndices(Vector2[] positions, int count)
    {
        var result = new int[positions.Length,count];
        var tree = new KDTree(positions);
        for (var i = 0; i != positions.Length; i++)
        {
            var nearestNeighbors = tree.FindNearestNeighbors(positions[i], count);
            for (var j = 0; j != count; j++)
            {
                result[i,j] = Array.IndexOf(positions, nearestNeighbors[j]);
            }
        }
        return result;
    }
    
    public static int[,] MyGetClosePositionIndices(Vector2[] positions, int count)
    {
        var result = new int[positions.Length,count];
        for (var i = 0; i != positions.Length; i++)
        {
            var allNeighbours = new int[positions.Length -1];
            var resultNeighbours = new int[count];
            var temp = 0;
            for (var j = 0; j != positions.Length; j++)
            {
                if (i == j)
                {
                    temp = 1;
                    continue;
                }
                allNeighbours[j-temp] = Array.IndexOf(positions, positions[j]);
            }
            
            Array.Sort(allNeighbours, (a, b) => positions[i].DistanceSquared(positions[a]).CompareTo(positions[i].DistanceSquared(positions[b])));
            Array.Copy(allNeighbours, resultNeighbours, count);
            for (var j = 0; j != resultNeighbours.Length; j++)
            {
                result[i,j] = resultNeighbours[j];
            }
        }
        return result;
    }

    public static Color[] GetMaxDistributedColors(int count)
    {
        var result = new Color[count];
        var phi = (1 + Mathf.Sqrt(5))/2;
        for (var i = 0; i != count; i++)
        {
            var n = i * phi - Mathf.Floor((i * phi));
            result[i] = Color.HSVToRGB(n, 1, 1);
        }
        return result;
    }
}