using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin_Game
{
    class GameObject
    {

        Bitmap bitmap;
        int width, height, x, y, speed;
        int displayX;
        int displayY;
        DisplayMetrics metrics;

        public GameObject(Context context)
        {
            Metrics = context.Resources.DisplayMetrics;
            DisplayX = Metrics.WidthPixels;
            DisplayY = Metrics.HeightPixels;
        }

        public virtual void MoveObject()
        {

        }

        public Rect GetColisionShape()
        {
            return new Rect(X, Y, X + Width, Y + Height);
        }

        public Bitmap Bitmap { get => bitmap; set => bitmap = value; }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public int Speed { get => speed; set => speed = value; }
        public int DisplayX { get => displayX; set => displayX = value; }
        public int DisplayY { get => displayY; set => displayY = value; }
        public DisplayMetrics Metrics { get => metrics; set => metrics = value; }
    }
}