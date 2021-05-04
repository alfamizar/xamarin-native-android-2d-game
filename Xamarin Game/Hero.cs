using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin_Game
{
    class Hero : GameObject
    {
        bool isMoveLeft;
        bool isMoveRight;
        public Hero(Context context) : base(context)
        {
            Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.slingshot2);
            Width = Metrics.WidthPixels / 14;
            Height = Width * Bitmap.Height / Bitmap.Width;
            Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

            X = (DisplayX - Width) / 2;
            Y = DisplayY - Height;

            Speed = (int)(6 * Metrics.WidthPixels / 1920f);
        }

        public override void MoveObject()
        {
            //Console.WriteLine("Hero Moved");
            if (isMoveLeft & !isMoveRight)
            {
                Console.WriteLine("Hero Moved left");
                X -= Speed;
                if (X <= 0)
                {
                    Console.WriteLine("Hero set x = 0");
                    X = 0;
                }
            }
            else if (!IsMoveLeft & IsMoveRight)
            {
                Console.WriteLine("Hero Moved right");
                X += Speed;
                if ((X + Width) > DisplayX)
                {
                    Console.WriteLine("Hero set x = DispalyX - Width");
                    X = DisplayX - Width;
                }
            }
        }

        public bool IsMoveLeft { get => isMoveLeft; set => isMoveLeft = value; }
        public bool IsMoveRight { get => isMoveRight; set => isMoveRight = value; }
    }
}