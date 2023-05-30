using OpenTK.Windowing.Common;
using SpaceEngine.GameWorld;
using OpenTK.Mathematics;
using SpaceEngine.Modelling;

namespace SpaceEngine.RenderEngine
{
    internal class Engine
    {
        private MasterRenderer masterRenderer;
        private WindowHandler windowHandler;
        private World world;
        public static float EngineClock = 0;

        public Engine()
        {
            windowHandler = new WindowHandler();
            masterRenderer = new MasterRenderer();
            world = new World();
            Model model = Loader.loadToVAO(MeshGenerator.generateBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f)));
            world.addModelEntity(new ModelEntity(new Vector3(0f, 0f, -4f), new Vector3(0), model));

            WindowHandler.getWindow().Load += delegate
            {
                init();
            };
            WindowHandler.getWindow().UpdateFrame += delegate (FrameEventArgs eventArgs)
            {
                update((float)eventArgs.Time);
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

        private void update(float delta)
        {
            EngineClock += delta;
            world.update(delta);
            masterRenderer.update(delta);
        }

        private void render()
        {
            masterRenderer.render(world.getModelEntities());
        }
    }
}
