using OpenTK.Windowing.Common;
using SpaceEngine.GameWorld;
using OpenTK.Mathematics;
using SpaceEngine.Modelling;
using SpaceEngine.RenderEngine;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace SpaceEngine.Core
{
    internal class Engine
    {
        private MasterRenderer masterRenderer;
        private WindowHandler windowHandler;
        private World world;
        private InputHandler inputHandler;
        public static float EngineDeltaClock = 0f;

        public Engine()
        {
            Vector2i resoltion = new Vector2i(1900, 1080);
            windowHandler = new WindowHandler(resoltion);
            masterRenderer = new MasterRenderer(resoltion);
            inputHandler = new InputHandler();
            world = new World();
            Model model = glLoader.loadToVAO(MeshGenerator.generateBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f)));
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
            WindowHandler.getWindow().KeyDown += delegate (KeyboardKeyEventArgs eventArgs)
            {
                inputHandler.keyDown(eventArgs);
            };
            WindowHandler.getWindow().KeyUp += delegate (KeyboardKeyEventArgs eventArgs)
            {
                inputHandler.keyUp(eventArgs);
            };
            WindowHandler.getWindow().Resize += delegate (ResizeEventArgs eventArgs)
            {
                masterRenderer.onResize(eventArgs);
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
            inputHandler.update(delta);
            EngineDeltaClock += delta;
            windowHandler.update(delta);
            world.update(delta);
            masterRenderer.update(delta);
        }

        private void render()
        {
            masterRenderer.render(world.getModelEntities(), world.getCamera());
        }
    }
}
