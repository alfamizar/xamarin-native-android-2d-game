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
    class Bird : GameObject
    {

        int[] ducksId = { Resource.Drawable.duck0, Resource.Drawable.duck1, Resource.Drawable.duck2, Resource.Drawable.duck3 };

        public Bird(Context context, int i) : base(context)
        {
            Random random = new Random();
            int index = random.Next(0, ducksId.Length);
            Bitmap = BitmapFactory.DecodeResource(context.Resources, ducksId[index]);
            Width = Metrics.WidthPixels / 16;
            Height = Width * Bitmap.Height / Bitmap.Width;
            Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

            X = random.Next(0, DisplayX - Width);
            Y = i * Height;

            Speed = -(int)(random.Next(4, 12) * Metrics.WidthPixels / 1920f);
        }

        override
        public void MoveObject()
        {
            X += Speed;
            if (X + Width > DisplayX)
            {
                Speed *= -1;
                Bitmap = CreateFlippledBitmap(Bitmap, true, false);
            }
            else if (X < 0)
            {
                Speed *= -1;
                Bitmap = CreateFlippledBitmap(Bitmap, true, false);
            }
        }

        public Bitmap CreateFlippledBitmap(Bitmap source, bool xFlip, bool yFlip)
        {
            Matrix matrix = new Matrix();
            matrix.PostScale(xFlip ? -1 : 1, yFlip ? -1 : 1, source.Width / 2, source.Height / 2);
            return Bitmap.CreateBitmap(source, 0, 0, Width, Height, matrix, true);
        }
    }
}