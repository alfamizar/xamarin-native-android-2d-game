using Android.Content;
using Android.Graphics;
using Android.Util;

namespace Xamarin_Game
{
    class GameObject
    {
        public Bitmap Bitmap;
        public int Width;
        public int Height;
        public int X;
        public int Y;
        public int Speed;
        public int DisplayX;
        public int DisplayY;
        public DisplayMetrics Metrics;

        public GameObject(Context context)
        {
            Metrics = context.Resources.DisplayMetrics;
            DisplayX = Metrics.WidthPixels;
            DisplayY = Metrics.HeightPixels;
        }

        public virtual void MoveObject()
        {
            // TODO
        }

        public virtual Rect GetColisionShape()
        {
            return new Rect(X, Y, X + Width, Y + Height);
        }
    }
}