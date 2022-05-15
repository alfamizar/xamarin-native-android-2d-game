using Android.Content;
using Android.Graphics;
using Android.Util;

namespace Xamarin_Game
{
    class GameObject
    {
        public Bitmap Bitmap;
        public DisplayMetrics Metrics;
        public int Width;
        public int Height;
        public int X;
        public int Y;
        public int Speed;
        public int DisplayX;
        public int DisplayY;
        public readonly float DisplayWidthSizeEqualizer;
        public readonly float DisplayHeightSizeEqualizer;

        public GameObject(Context context)
        {
            Metrics = context.Resources.DisplayMetrics;
            DisplayX = Metrics.WidthPixels;
            DisplayY = Metrics.HeightPixels;

            // Asuming game always running in landscape mode
            // Base display resolution is Full HD and it will be scaled to any size and resolution
            DisplayWidthSizeEqualizer = DisplayX / 1920f;
            DisplayHeightSizeEqualizer = DisplayY / 1080f;
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