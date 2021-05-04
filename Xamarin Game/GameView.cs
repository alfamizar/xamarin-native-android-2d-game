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
using System.Threading;

namespace Xamarin_Game
{
    class GameView : SurfaceView, ISurfaceHolderCallback
    {
        Context context;
        Thread gameThread, renderThread, birdsGeneratorThread;
        ISurfaceHolder surfaceHolder;
        bool isRunning;
        int displayX, displayY;
        int score = 0;
        Paint scorePaint = new Paint();
        float rX, rY;
        Background background;
        Hero hero;
        List<Bird> birds = new List<Bird>();
        List<Stone> stones = new List<Stone>();

        int BIRDS_MAX_COUNT = 4;


        public GameView(Context context) : base(context)
        {

            this.context = context;

            var metrics = Resources.DisplayMetrics;
            displayX = metrics.WidthPixels;
            displayY = metrics.HeightPixels;
            rX = displayX / 1920f;
            rY = displayY / 1080f;

            surfaceHolder = Holder;
            surfaceHolder.AddCallback(this);

            background = new Background(context);

            for (int i = 0; i < BIRDS_MAX_COUNT; i++)
            {
                birds.Add(new Bird(context, i));
            }

            hero = new Hero(context);

            scorePaint.TextSize = 30;
            scorePaint.Color = Color.Red;
        }
        override
        public void Draw(Canvas canvas)
        {

            canvas.DrawBitmap(background.Bitmap, background.X, background.Y, null);
            for (int i = 0; i < birds.Count; i++)
            {
                Bird bird = birds.ElementAt(i);
                canvas.DrawBitmap(bird.Bitmap, bird.X, bird.Y, null);
            }
            canvas.DrawBitmap(hero.Bitmap, hero.X, hero.Y, null);

            if (stones.Count > 0)
            {
                for (int i = 0; i < stones.Count; i++)
                {
                    Stone stone = stones.ElementAt(i);
                    canvas.DrawBitmap(stone.Bitmap, stone.X, stone.Y, null);
                }
            }

            canvas.DrawText(score.ToString(), 5, 35, scorePaint);

        }

        public void Run()
        {
            Canvas canvas = null;
            while (isRunning)
            {
                if (surfaceHolder.Surface.IsValid)
                {
                    canvas = surfaceHolder.LockCanvas();
                    Draw(canvas);
                    surfaceHolder.UnlockCanvasAndPost(canvas);
                }
                Thread.Sleep(17);
            }
        }



        public void Update()
        {
            while (isRunning)
            {
                List<Bird> birdsTrash = new List<Bird>();
                List<Stone> stonesTrash = new List<Stone>();
                for (int i = 0; i < birds.Count; i++)
                {
                    Bird bird = birds.ElementAt(i);
                    bird.MoveObject();

                    if (stones.Count > 0)
                    {
                        for (int j = 0; j < stones.Count; j++)
                        {
                            Stone stone = stones.ElementAt(j);
                            if (Rect.Intersects(stone.GetColisionShape(), bird.GetColisionShape()))
                            {
                                Console.WriteLine("Intersected");
                                score++;
                                birdsTrash.Add(bird);
                                stonesTrash.Add(stone);
                            }
                        }
                    }
                }

                hero.MoveObject();

                if (stones.Count > 0)
                {
                    for (int i = 0; i < stones.Count; i++)
                    {
                        Stone stone = stones.ElementAt(i);
                        stone.MoveObject();
                        if (stone.Y + Height < 0)
                        {
                            stonesTrash.Add(stone);
                        }
                    }
                }
                for (int i = 0; i < birdsTrash.Count; i++)
                {
                    if (birds.Count > 0)
                    {
                        birds.Remove(birdsTrash.ElementAt(i));
                    }
                }

                for (int i = 0; i < stonesTrash.Count; i++)
                {
                    if (stones.Count > 0)
                    {
                        stones.Remove(stonesTrash.ElementAt(i));
                    }
                }
                Thread.Sleep(17);
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            Resume();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            Pause();
        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.ActionMasked == MotionEventActions.Down)
            {
                Console.WriteLine("Action Down");
                if (e.GetX() > 0 & e.GetX() < displayX / 3)
                {
                    Console.WriteLine("Action Down Left 1/3 Screen");
                    hero.IsMoveLeft = true;
                    hero.IsMoveRight = false;
                }
                else if (e.GetX() > (displayX / 3 * 2) & e.GetX() < displayX)
                {
                    Console.WriteLine("Action Down Right 1/3 Screen");
                    hero.IsMoveLeft = false;
                    hero.IsMoveRight = true;
                }
                else
                {
                    Stone stone = new Stone(context, hero);
                    stones.Add(stone);
                }
            }
            else if (e.ActionMasked == MotionEventActions.Up)
            {
                Console.WriteLine("Action Up");
                hero.IsMoveLeft = false;
                hero.IsMoveRight = false;
            }

            return true;
        }

        private void GenerateBirds()
        {
            while (isRunning)
            {
                if (birds.Count < BIRDS_MAX_COUNT)
                {
                    birds.Add(new Bird(context, new Random().Next(0, 4)));
                }
                Thread.Sleep(2500);
            }
        }

        public void Resume()
        {
            isRunning = true;

            gameThread = new Thread(new ThreadStart(Update));
            renderThread = new Thread(new ThreadStart(Run));
            birdsGeneratorThread = new Thread(new ThreadStart(GenerateBirds));

            gameThread.Start();
            renderThread.Start();
            birdsGeneratorThread.Start();
        }

        public void Pause()
        {
            bool retry = true;
            while (retry)
            {
                try
                {
                    isRunning = false;
                    gameThread.Join();
                    renderThread.Join();
                    birdsGeneratorThread.Join();
                    retry = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}