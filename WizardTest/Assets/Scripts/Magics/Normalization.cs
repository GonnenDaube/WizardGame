using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MachineLearning
{
    public class Normalization
    {
        public static List<Vector2> NormalizeData(List<Vector3> pos, int n)
        {
            //Normalizes data between 0 and 1
            //Makes sure there are only n points

            List<Vector2> extreme = FindExtreme(pos);
            List<Vector2> norm = new List<Vector2>();
            List<Vector2> output = new List<Vector2>();
            float max, min;
            float x, y;
            max = Math.Max(extreme[0].x, extreme[0].y);
            min = Math.Min(extreme[1].x, extreme[1].y);

            foreach (Vector3 p in pos)
            {
                x = (p.x - min) / (max - min);
                y = (p.y - min) / (max - min);
                norm.Add(new Vector2(x, y));
            }

            int i = 1;
            int index;
            while (norm.Count > n)
            {
                for (int j = 1; norm.Count > n && j < i + 1; j++)
                {
                    index = j * norm.Count / (i + 1);
                    norm.RemoveAt(index);
                }
                i++;
            }

            i = 1;
            while (norm.Count < n)
            {
                for (int j = 1; norm.Count < n && j < i + 1; j++)
                {
                    index = j * norm.Count / (i + 1);
                    norm.Insert(index + 1, Average(norm[index], norm[index + 1]));
                }
                i++;
            }

            for (int j = 0; j < norm.Count; j++)
            {
                norm[j] = new Vector2(norm[j].x, 1.0f - norm[j].y);
            }

            return norm;
        }

        private static Vector2 Average(Vector2 a, Vector3 b)
        {
            return new Vector2((a.x + b.x) / 2, (a.y + b.y) / 2);
        }

        private static List<Vector2> FindExtreme(List<Vector3> pos)
        {
            List<Vector2> max = new List<Vector2>();
            max.Add(new Vector2(pos[0].x, pos[0].y));
            max.Add(new Vector2(pos[0].x, pos[0].y));
            foreach (Vector3 p in pos)
            {
                if (max[0].x < p.x)
                    max[0] = new Vector2(p.x, max[0].y);
                if (max[0].y < p.y)
                    max[0] = new Vector2(max[0].x, p.y);
                if (max[1].x > p.x)
                    max[1] = new Vector2(p.x, max[1].y);
                if (max[1].y > p.y)
                    max[1] = new Vector2(max[1].x, p.y);
            }
            return max;
        }
    }
}
