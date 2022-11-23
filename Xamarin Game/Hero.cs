using Android.Content;
using Android.Graphics;

namespace Xamarin_Game
{
    internal class Hero : GameObject
    {
        public Hero(Context context) : base(context)
        {
            Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.slingshot2);
            Width = Metrics.WidthPixels / 14;
            Height = Width * Bitmap.Height / Bitmap.Width;
            Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

            X = (DisplayX - Width) / 2;
            Y = DisplayY - Height;
            // (Metrics.WidthPixels / 1920f) - screen ration to make speed the same on all devices.
            Speed = (int)(6 * DisplayWidthSizeEqualizer);
        }

        public override void MoveObject()
        {
            if (IsMovingLeft & !IsMovingRight)
            {
                X -= Speed;
                if (X <= 0)
                {
                    X = 0;
                }
            }
            else if (!IsMovingLeft & IsMovingRight)
            {
                X += Speed;
                if ((X + Width) > DisplayX)
                {
                    X = DisplayX - Width;
                }
            }
        }

        public bool IsMovingLeft { get; set; }
        public bool IsMovingRight { get; set; }
    }
}