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
    class Background:GameObject
    {
 
        public Background(Context context):base(context)
        {
            Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.forest);
            Width = Metrics.WidthPixels;
            Height = Metrics.HeightPixels;
            Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

            X = 0;
            Y = 0;
        }

    }
}