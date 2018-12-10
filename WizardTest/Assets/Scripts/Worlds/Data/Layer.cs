using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldData
{
    public class Layer : MonoBehaviour
    {
        private const float ratio = 16.0f / 9.0f;
        private const float smoothness = 20.0f;
        private const float maxY = 5.0f;
        private const float minY = -5.0f;

        public float Ratio
        {
            get
            {
                return ratio;
            }
        }

        public List<float> x;
        public List<float> y;
        public float size;
        public List<float> color;
        public string color_id;
        public List<Sprite> sprites;
        public List<Portal> portals;

        public void Copy(Layer l)
        {
            this.x = new List<float>(l.x);
            this.y = new List<float>(l.y);
            this.size = l.size;
            this.color = new List<float>(l.color);
            this.color_id = l.color_id;
            this.sprites = new List<Sprite>(l.sprites);
        }

        private static List<Vector2> CurveSegment(float x1, float x2, float x3, float y1, float y2, float y3)
        {
            float a = ((y1 - y2) * (x2 - x3) - (y2 - y3) * (x1 - x2)) / ((x1 * x1 - x2 * x2) * (x2 - x3) - (x2 * x2 - x3 * x3) * (x1 - x2));
            float b = ((y1 - y2) * (x2 - x3) - a * (x1 * x1 - x2 * x2) * (x2 - x3)) / ((x1 - x2) * (x2 - x3));
            float c = y1 - a * x1 * x1 - b * x1;

            List<Vector2> ret = new List<Vector2>();
            float step = (x3 - x1) / smoothness;
            Vector2 temp;
            float x, y;
            for (int i = 1; i <= smoothness; i++)
            {
                x = x1 + i * step;
                y = a * x * x + b * x + c;
                temp = new Vector2(x, Mathf.Clamp(y, -5.0f, 5.0f));
                ret.Add(temp);
            }
            return ret;
        }

        public static List<Vector2> GetCurvedLine(List<float> lx, List<float> ly)
        {
            List<Vector2> ret = new List<Vector2>();
            List<Vector2> segment;
            float x1, x2, x3, y1, y2, y3;
            //add first two points
            ret.Add(new Vector2(ratio * (lx[0] / 10.0f), (100 - ly[0]) / 10.0f - 5.0f));
            ret.Add(new Vector2(ratio * (lx[1] / 10.0f), (100 - ly[1]) / 10.0f - 5.0f));

            for (int i = 1; i < lx.Count - 3; i += 2)
            {
                x1 = ratio * (lx[i] / 10.0f);
                x2 = ratio * (lx[i + 1] / 10.0f);
                x3 = ratio * (lx[i + 2] / 10.0f);
                y1 = (100 - ly[i]) / 10.0f - 5.0f;
                y2 = (100 - ly[i + 1]) / 10.0f - 5.0f;
                y3 = (100 - ly[i + 2]) / 10.0f - 5.0f;
                //convert each 3 points to a curve
                segment = CurveSegment(x1, x2, x3, y1, y2, y3);

                ret.AddRange(segment);
            }

            //add last point
            int lastIndex = lx.Count - 1;
            ret.Add(new Vector2(ratio * (lx[lastIndex] / 10.0f), (100 - ly[lastIndex]) / 10.0f - 5.0f));

            return ret;
        }

        public static List<int> GenerateTriangles(List<Vector2> line)
        {
            List<int> triangles = new List<int>();
            int i, i1, i2;
            float m, n;
            i = 0;
            bool[] flags = new bool[line.Count];//initial value is false
            while (numActive(flags) > 2)
            {
                i1 = adjIndex(i, flags);
                i2 = adjIndex(i1, flags);
                if (i1 == i2)
                    break;
                if (i1 == -1 || i2 == -1)
                {
                    i = 0;
                    i1 = adjIndex(i, flags);
                    i2 = adjIndex(i1, flags);
                }
                m = (line[i2].y - line[i].y) / (line[i2].x - line[i].x);
                n = line[i].y - m * line[i].x;
                if (line[i1].y >= m * line[i1].x + n)
                {
                    triangles.Add(i);
                    triangles.Add(i1);
                    triangles.Add(i2);
                    flags[i1] = true;
                }
                else
                {
                    i = i1;
                }
            }
            return triangles;
        }

        private static int adjIndex(int i, bool[] flags)
        {
            for (int j = i + 1; j < flags.Length; j++)
            {
                if (!flags[j])
                    return j;
            }
            return -1;
        }

        private static int numActive(bool[] flags)
        {
            int c = 0;
            for (int i = 0; i < flags.Length; i++)
            {
                if (!flags[i])
                    c++;
            }
            return c;
        }
    }
}