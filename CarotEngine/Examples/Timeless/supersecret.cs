using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using pr2.CarotEngine;
using pr2.Common;

namespace Timeless {

	partial class Timeless
    {
        int[] skewlines = new int[100];

        unsafe void SuperSecretThingy(int x, int y1, int y, Image src, Image dest)
        {
            int xofs = x < 0 ? (256 - x) % 256 : x % 256;
            int rot = x < 0 ? (320 - x) % 320 : x % 320;
            int yofs = (y1) % 256;
            ushort ofs = 0;

            using (Image.LockCache srclock = src.Lock(), destlock = dest.Lock())
            {
                int* s = srclock.data;
                int* d = destlock.data; //+ pitch???

                int pitch = dest.Width;
                int pitch199 = pitch * 199;
                int chunkindex;
                int lutindex;
                int startofs = (yofs << 8) | xofs;
                for (int i = 0; i < 4; i++)
                {
                    chunkindex = rot + i;
                    for (int xx = 0; xx < 80; xx++)
                    {
                        lutindex = (chunkindex % 320) * 100;
                        ofs = (ushort)startofs;
                        int topofs = xx * 4 + i;
                        int botofs = xx * 4 + i + pitch199;
                        for (int yy = 0; yy < 100; yy++)
                        {
                            ofs += (ushort)(SuperSecretTable[lutindex] + skewlines[yy]);
                            lutindex++;
                            d[topofs] = s[ofs];
                            d[botofs] = s[ofs];
                            topofs += pitch;
                            botofs -= pitch;
                        }
                        chunkindex += 4;
                    }
                }
            }
        }



    }

}
