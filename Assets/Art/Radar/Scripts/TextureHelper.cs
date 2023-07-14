using UnityEngine;

namespace Kataner
{
    public class TextureHelper
    {

        public static Texture2D CreateClearTexture(int width, int height)
        {
            Texture2D tex2D = new Texture2D(1000, 1000)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            for (int xw = 0; xw < tex2D.width; xw++)
            {
                for (int yh = 0; yh < tex2D.height; yh++)
                {
                    tex2D.SetPixel(xw, yh, Color.clear);
                }
            }
            return tex2D;
        }

        public static void DrawLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col)
        {
            Vector2 t = p1;
            float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
            float ctr = 0;

            while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
            {
                t = Vector2.Lerp(p1, p2, ctr);
                ctr += frac;
                tex.SetPixel((int)t.x, (int)t.y, col);
            }
        }

        public static void DrawCircle(Texture2D tex, int cx, int cy, int r, Color col)
        {
            var y = r;
            var d = 1 / 4 - r;
            var end = Mathf.Ceil(r / Mathf.Sqrt(2));

            for (int x = 0; x <= end; x++)
            {
                tex.SetPixel(cx + x, cy + y, col);
                tex.SetPixel(cx + x, cy - y, col);
                tex.SetPixel(cx - x, cy + y, col);
                tex.SetPixel(cx - x, cy - y, col);
                tex.SetPixel(cx + y, cy + x, col);
                tex.SetPixel(cx - y, cy + x, col);
                tex.SetPixel(cx + y, cy - x, col);
                tex.SetPixel(cx - y, cy - x, col);

                d += 2 * x + 1;
                if (d > 0)
                {
                    d += 2 - 2 * y--;
                }
            }
        }


    }
}