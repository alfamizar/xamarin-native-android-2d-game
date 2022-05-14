﻿using Android.Content;
using Android.Graphics;

namespace Xamarin_Game
{
    class Stone : GameObject
    {
        public Stone(Context context, Hero hero) : base(context)
        {
            Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.stone);
            Width = Metrics.WidthPixels / 22;
            Height = Width * Bitmap.Height / Bitmap.Width;
            Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

            X = (hero.X + ((hero.Width - Width) / 2));
            Y = hero.Y - Height;

            Speed = (int)(12 * Metrics.WidthPixels / 1920f);
        }

        public override void MoveObject()
        {
            Y -= Speed;
        }
    }
}