using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug;

namespace Xamarin_Game
{
    class GameView : SurfaceView, ISurfaceHolderCallback
    {
        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;

        ISurfaceHolder surfaceHolder;

        bool isRunning;
        int displayX, displayY;
        int score = 0;
        float rX, rY;

        Paint scorePaint = new Paint();
        Background background;
        Hero hero;
        List<Bird> birds = new List<Bird>();
        List<Stone> stones = new List<Stone>();

        const int BIRDS_MAX_COUNT = 4;

        public GameView(Context context) : base(context)
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

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

        public void Run()
        {
            /*            Stopwatch stopWatch = new Stopwatch();

                        while (isRunning)
                        {
                            stopWatch.Reset();
                            stopWatch.Start();
                            Update();
                            Render();
                            stopWatch.Stop();
                            Debug.WriteLine(stopWatch.ElapsedMilliseconds);
                            if (stopWatch.ElapsedMilliseconds < 17)
                                Thread.Sleep((int)(17 - stopWatch.ElapsedMilliseconds));
                        }*/
            double MS_PER_UPDATE = 10;
            double previous = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            double lag = 0.0;

            while (isRunning)
            {
                double current = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                double elapsed = current - previous;
                previous = current;
                lag += elapsed;
                //processInput();
                while (lag >= MS_PER_UPDATE)
                {
                    Update();
                    lag -= MS_PER_UPDATE;
                }

                //Render(lag / MS_PER_UPDATE);
                Render();
            }
        }

        public void Update()
        {
            //Thread.Sleep(34);
            List <Bird> birdsToBeRemoved = new List<Bird>();
            List<Stone> stonesToBeRemoved = new List<Stone>();

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
                            Debug.WriteLine("Intersected");
                            score++;
                            birdsToBeRemoved.Add(bird);
                            stonesToBeRemoved.Add(stone);
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
                        stonesToBeRemoved.Add(stone);
                    }
                }
            }

            for (int i = 0; i < birdsToBeRemoved.Count; i++)
            {
                if (birds.Count > 0)
                {
                    birds.Remove(birdsToBeRemoved.ElementAt(i));
                }
            }

            for (int i = 0; i < stonesToBeRemoved.Count; i++)
            {
                if (stones.Count > 0)
                {
                    stones.Remove(stonesToBeRemoved.ElementAt(i));
                }
            }
        }

        private void Render(double interpolate = 1)
        {
            if (!surfaceHolder.Surface.IsValid)
            {
                return;
            }

            Canvas canvas = surfaceHolder.LockCanvas();

            canvas.DrawBitmap(background.Bitmap, background.X, background.Y, null);

            for (int i = 0; i < birds.Count; i++)
            {
                Bird bird = birds.ElementAt(i);
                canvas.DrawBitmap(bird.Bitmap, (float)(bird.X * interpolate), bird.Y, null);
            }

            canvas.DrawBitmap(hero.Bitmap, (float)(hero.X * interpolate), hero.Y, null);

            if (stones.Count > 0)
            {
                for (int i = 0; i < stones.Count; i++)
                {
                    Stone stone = stones.ElementAt(i);
                    canvas.DrawBitmap(stone.Bitmap, (float)(stone.X * interpolate), stone.Y, null);
                }
            }

            canvas.DrawText(score.ToString(), 5, 35, scorePaint);

            surfaceHolder.UnlockCanvasAndPost(canvas);
        }

        private void GenerateBirds()
        {
            while (isRunning)
            {
                if (birds.Count < BIRDS_MAX_COUNT)
                {
                    birds.Add(new Bird(Context, new Random().Next(0, 4)));
                }
                Thread.Sleep(2500);
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.ActionMasked == MotionEventActions.Down)
            {
                Debug.WriteLine("Action Down");
                if (e.GetX() > 0 & e.GetX() < displayX / 3)
                {
                    Debug.WriteLine("Action Down Left 1/3 Screen");
                    hero.IsMoveLeft = true;
                    hero.IsMoveRight = false;
                }
                else if (e.GetX() > (displayX / 3 * 2) & e.GetX() < displayX)
                {
                    Debug.WriteLine("Action Down Right 1/3 Screen");
                    hero.IsMoveLeft = false;
                    hero.IsMoveRight = true;
                }
                else
                {
                    Stone stone = new Stone(Context, hero);
                    stones.Add(stone);
                }
            }
            else if (e.ActionMasked == MotionEventActions.Up)
            {
                Debug.WriteLine("Action Up");
                hero.IsMoveLeft = false;
                hero.IsMoveRight = false;
            }
            return true;
        }

        public void Resume()
        {
            isRunning = true;
            Task gameLoopTask = Task.Run(() =>
            {                
                Run();
            }, cancellationToken);

            Task birdsGenerationTask = Task.Run(() =>
            {
                GenerateBirds();
            }, cancellationToken);
        }

        public void Pause()
        {
            isRunning = false;
        }


        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {

        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            Pause();
        }
    }
}