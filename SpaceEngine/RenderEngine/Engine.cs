using OpenTK.Windowing.Common;

namespace SpaceEngine.RenderEngine
{
    internal class Engine
    {
        private MasterRenderer masterRenderer;
        private WindowHandler windowHandler;

        public Engine()
        {
            windowHandler = new WindowHandler();
            masterRenderer = new MasterRenderer();

            WindowHandler.getWindow().Load += delegate
            {
                init();
            };
            WindowHandler.getWindow().UpdateFrame += delegate (FrameEventArgs eventArgs)
            {
                update();
            };
            WindowHandler.getWindow().RenderFrame += delegate (FrameEventArgs eventArgs)
            {
                render();
            };
        }

        public void run()
        {

            WindowHandler.getWindow().Run();
        }

        private void init()
        {

        }

        private void update()
        {

            //Console.WriteLine(eventArgs.Time);
        }

        private void render()
        {
            masterRenderer.render();
        }
    }
}
