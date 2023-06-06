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
        public static ComponentSystem flatShadingSystem = new ComponentSystem();
        public static ComponentSystem smoothShadingSystem = new ComponentSystem();
        public static ComponentSystem pointLightSystem = new ComponentSystem();
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

            Random rand = new Random();
            for (int i = 0; i<10; i++)
            {
                Vector3 color = new Vector3(rand.NextSingle(), rand.NextSingle(), rand.NextSingle());
                Entity sphere = new Entity();
                sphere.addComponent(new Transformation(new Vector3(4f, 2.3f, 4f) + center, new Vector3(0f, 0f, 0f)));
                sphere.addComponent(glLoader.loadToVAO(MeshGenerator.generateIcosahedron(0.1f, color, MasterRenderer.Pipeline.FLAT_SHADING)));
                sphere.addComponent(new PointLight(color, new Vector3(0.1f, 0f, 1.5f)));
                sphere.addComponent(new RandomMover());
            }
            loadTerrain();

        }
        public void loadTerrain()
        {
            if (terrainChunk != null)
            {
                terrainChunk.cleanUp();
            }
            float size = 1260f;
            int detail =800;
            TerrainChunk terrain = new TerrainChunk(new Vector2(0f, 0f), size, detail);
            terrainChunk = new Entity();
            terrainChunk.addComponent(terrain.generateModel(MasterRenderer.Pipeline.FLAT_SHADING));
            terrainChunk.addComponent(new Transformation(new Vector3(-size/2f, 0f, -size / 2f), new Vector3(0f, 0f, 0f), 1.0f));
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
