
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;

namespace SpaceEngine.RenderEngine
{
    internal class WindowHandler
    {
        private string title = "SpaceEngine";
        private static GameWindow? gameWindow = null;

        private Stopwatch frameStopWatch = new Stopwatch();
        private Stopwatch secondStopWatch = new Stopwatch();
        private float delta = 0f;
        private int framesLastSecond = 0;
        private int framesCurrentSecond = 0;

        private float testDelta1 = 0f;
        private float testDelta2 = 0f;
        public WindowHandler(Vector2i resoltion)
        {
            GameWindowSettings gws = GameWindowSettings.Default;
            NativeWindowSettings nws = NativeWindowSettings.Default;

            nws.API = ContextAPI.OpenGL;
            //nws.APIVersion = Version.Parse("4.1");
            nws.AutoLoadBindings = true;
            nws.Title = title;
            nws.Size = resoltion;
            nws.Location = new Vector2i(1300, 200);

            gws.RenderFrequency = 1200;
            gws.UpdateFrequency = 1200;

            gameWindow = new GameWindow(gws, nws);

            secondStopWatch.Start();
            frameStopWatch.Start();
        }
        public static GameWindow getWindow()
        {
            return gameWindow;
        }
        public void update(float delta)
        {
            this.delta = (float)frameStopWatch.Elapsed.TotalSeconds;
            frameStopWatch.Restart();


            if (secondStopWatch.Elapsed.TotalMilliseconds >= 1000.0)
            {
                framesLastSecond = framesCurrentSecond;
                framesCurrentSecond = 0;
                gameWindow.Title = title + " " + framesLastSecond + " FPS";
                secondStopWatch.Restart();

    }

            framesCurrentSecond++;
            
        }
        public float getDelta()
        {
            return delta;
        }
    }
}
