using SpaceEngine.Modelling;
using SpaceEngine.RenderEngine;
using OpenTK.Mathematics;
using SpaceEngine.Core;
using OpenTK.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceEngine.Util;

namespace SpaceEngine.Entity_Component_System.Components
{
    internal class EntityManager
    {

        public static List<Entity> entities = new List<Entity>();
        public static List<Entity> flatShadingModelEntities = new List<Entity>();
        public static List<Entity> smoothShadingModelEntities = new List<Entity>();
        public static List<Entity> pointLights = new List<Entity>();
        public Entity camera { get; set; }
        public Entity? terrainChunk;
        public EntityManager() {

            Vector3 center = new Vector3 (0, 0, 0);

            camera = new Entity();
            camera.addComponent(new Transformation(new Vector3(-1f, 3f, -1f)+center, new Vector3(0.5f, MyMath.PI/2f+ MyMath.PI / 4f, 0f)));
            camera.addComponent(new InputMove());

            Entity box = new Entity();
            box.addComponent(new Transformation());
            box.addComponent(glLoader.loadToVAO(MeshGenerator.generateBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))));
            flatShadingModelEntities.Add(box);

            Random rand = new Random();
            for (int i = 0; i<100; i++)
            {
                Vector3 color = new Vector3(rand.NextSingle(), rand.NextSingle(), rand.NextSingle());
                Entity sphere = new Entity();
                sphere.addComponent(new Transformation(new Vector3(4f, 2.3f, 4f) + center, new Vector3(0f, 0f, 0f)));
                sphere.addComponent(glLoader.loadToVAO(MeshGenerator.generateIcosahedron(0.1f, color)));
                sphere.addComponent(new PointLight(color, new Vector3(0.1f, 0f, 3f)));
                sphere.addComponent(new RandomMover());
                flatShadingModelEntities.Add(sphere);
                pointLights.Add(sphere);
            }


            loadTerrain();

        }
        public void loadTerrain()
        {
            if (terrainChunk != null)
            {
                terrainChunk.cleanUp();
                flatShadingModelEntities.Remove(terrainChunk);
                smoothShadingModelEntities.Remove(terrainChunk);
            }
            float size = 100f;
            int detail = 400;
            TerrainChunk terrain = new TerrainChunk(new Vector2(0f, 0f), size, detail);
            terrainChunk = new Entity();
            terrainChunk.addComponent(terrain.generateModel());
            terrainChunk.addComponent(new Transformation(new Vector3(-size/2f, 0f, -size / 2f), new Vector3(0f, MyMath.PI/2f, 0f), 1.0f));
            smoothShadingModelEntities.Add(terrainChunk);
        }
        public void update(float delta)
        {   
            if (InputHandler.isKeyClicked(Keys.T))
            {
                loadTerrain();
            }
            //camera.updateComponents(delta);
            foreach(Entity entity in entities)
            {
                entity.updateComponents(delta);
            }
        }
    }
}
