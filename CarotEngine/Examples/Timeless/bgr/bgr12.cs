//using System;
//using System.Collections.Generic;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Storage;
//using pr2.CarotEngine;
//using pr2.Common;

//namespace Timeless
//{
//  partial class Timeless
//  {

//    int bgr12x, bgr12y;

//    void InitializeBGR12(int r)
//    {
//      bgrtick = 0;
//      bgrtoggle = 0;
//      bgrproc = RenderBGR12;
//      Image myimg = null;
//      switch (r)
//      {
//        case 0: myimg = evilgrad1; break;
//        case 1: myimg = evilgrad2; break;
//        case 2: myimg = evilgrad3; break;
//        case 3: myimg = evilgrad4; break;
//      }
//      Blitter b = new Blitter(bg2);
//      b.TClear(); //is this correct?
//      WrapBlit(b, myimg, 0, 0);
//      b = new Blitter(bgalpha);
//      b.TClear();
//      //b.RectFill(0, 0, 255, 255, 0);
//    }


//    //TODO - store window coords here

//    void RenderBGR12()
//    {
//      if (bgrtoggle < 2)
//      {
//        bgrtoggle++;
//        return;
//      }
//      bgrtoggle -= 2;

//      if (bgrtick > 160)
//      {
//        SetClip(0, 0, 255, 255, bgalpha);

//        bgrproc = "";
//        return;
//      }
//      int i;
//      //	if (bgrtick<80)
//      //	{
//      for (i = 0; i < 32; i++)
//      {
//        CalcBGR12ClipCoords(i, 0);
//        TBlit(bgr12x, bgr12y - 11 + bgrtick, fadedown, bgalpha);
//      }
//      //	}
//      //else if (bgrtick<160)
//      //	{
//      for (i = 0; i < 32; i++)
//      {
//        CalcBGR12ClipCoords(i, 1);
//        TBlit(bgr12x, bgr12y + 63 - (bgrtick), fadeup, bgalpha);
//      }
//      //	}
//      AlphaBlit(0, 0, bg2, bgalpha, bg);
//      SetClip(0, 0, 255, 255, bgalpha);
//      bgrtick++;
//    }
//  }
//}
