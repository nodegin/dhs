using System;
using System.Collections.Generic;
using System.Text;

namespace DHS.Misc
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    public class ImageTools
    {
        public static void FastBlur(Bitmap SourceImage, int radius)
        {
            var rct = new Rectangle(0, 0, SourceImage.Width, SourceImage.Height);
            var dest = new int[rct.Width * rct.Height];
            var source = new int[rct.Width * rct.Height];
            var bits = SourceImage.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Marshal.Copy(bits.Scan0, source, 0, source.Length);
            SourceImage.UnlockBits(bits);

            if (radius < 1) return;

            int w = rct.Width;
            int h = rct.Height;
            int wm = w - 1;
            int hm = h - 1;
            int wh = w * h;
            int div = radius + radius + 1;
            var r = new int[wh];
            var g = new int[wh];
            var b = new int[wh];
            int rsum, gsum, bsum, x, y, i, p1, p2, yi;
            var vmin = new int[max(w, h)];
            var vmax = new int[max(w, h)];

            var dv = new int[256 * div];
            for (i = 0; i < 256 * div; i++)
                dv[i] = (i / div);

            int yw = yi = 0;

            for (y = 0; y < h; y++)
            { // blur horizontal
                rsum = gsum = bsum = 0;
                for (i = -radius; i <= radius; i++)
                {
                    int p = source[yi + min(wm, max(i, 0))];
                    rsum += (p & 0xff0000) >> 16;
                    gsum += (p & 0x00ff00) >> 8;
                    bsum += p & 0x0000ff;
                }
                for (x = 0; x < w; x++)
                {

                    r[yi] = dv[rsum];
                    g[yi] = dv[gsum];
                    b[yi] = dv[bsum];

                    if (y == 0)
                    {
                        vmin[x] = min(x + radius + 1, wm);
                        vmax[x] = max(x - radius, 0);
                    }
                    p1 = source[yw + vmin[x]];
                    p2 = source[yw + vmax[x]];

                    rsum += ((p1 & 0xff0000) - (p2 & 0xff0000)) >> 16;
                    gsum += ((p1 & 0x00ff00) - (p2 & 0x00ff00)) >> 8;
                    bsum += (p1 & 0x0000ff) - (p2 & 0x0000ff);
                    yi++;
                }
                yw += w;
            }

            for (x = 0; x < w; x++)
            { // blur vertical
                rsum = gsum = bsum = 0;
                int yp = -radius * w;
                for (i = -radius; i <= radius; i++)
                {
                    yi = max(0, yp) + x;
                    rsum += r[yi];
                    gsum += g[yi];
                    bsum += b[yi];
                    yp += w;
                }
                yi = x;
                for (y = 0; y < h; y++)
                {
                    dest[yi] = (int)(0xff000000u | (uint)(dv[rsum] << 16) | (uint)(dv[gsum] << 8) | (uint)dv[bsum]);
                    if (x == 0)
                    {
                        vmin[y] = min(y + radius + 1, hm) * w;
                        vmax[y] = max(y - radius, 0) * w;
                    }
                    p1 = x + vmin[y];
                    p2 = x + vmax[y];

                    rsum += r[p1] - r[p2];
                    gsum += g[p1] - g[p2];
                    bsum += b[p1] - b[p2];

                    yi += w;
                }
            }

            // copy back to image
            var bits2 = SourceImage.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Marshal.Copy(dest, 0, bits2.Scan0, dest.Length);
            SourceImage.UnlockBits(bits);
        }

        private static int min(int a, int b) { return Math.Min(a, b); }
        private static int max(int a, int b) { return Math.Max(a, b); }
    }
}
