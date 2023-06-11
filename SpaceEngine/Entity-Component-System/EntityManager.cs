using SpaceEngine.Modelling;
using SpaceEngine.RenderEngine;
using OpenTK.Mathematics;
using SpaceEngine.Core;
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
        public static TerrainManager terrainManager = new TerrainManager();
        public static Object threadLock = new object();
        public Entity sculpture = new Entity();
        public Entity sun;
        public Entity camera { get; set; }
        public EntityManager() {

            Vector3 center = new Vector3 (0, 0, 0);

            camera = new Entity();
            camera.addComponent(new Transformation(new Vector3(-31f, 13f, -1f)+center, new Vector3(0.5f, MathF.PI/2f+ MathF.PI / 4f, 0f)));;
            camera.addComponent(new InputMove());
            camera.addComponent(new Momentum());
            //camera.addComponent(new Gravity());
            camera.addComponent(new TerrainCollider());
            camera.addComponent(new HitBox(new Vector3(-1, -1, -1), new Vector3(1)));

            Entity box = new Entity();
            box.addComponent(new Transformation());
            box.addComponent(glLoader.loadToVAO(MeshGenerator.generateBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))));

            Entity sun = new Entity();
            sun.addComponent(new Sun());
            this.sun = sun;

            Random rand = new Random();
            for (int i = 0; i<10; i++)
            {
                Vector3 color = new Vector3(rand.NextSingle(), rand.NextSingle(), rand.NextSingle());
                Entity sphere = new Entity();
                sphere.addComponent(new Transformation(new Vector3(4f, 2.3f, 4f) + center, new Vector3(0f, 0f, 0f)));
                RawModel rawModel = MeshGenerator.generateIcosahedron(new Vector3(0.5f), color, MasterRenderer.Pipeline.FLAT_SHADING);
                rawModel.setBloom(3f);
                sphere.addComponent(glLoader.loadToVAO(rawModel));
                sphere.addComponent(new PointLight(color*3f, new Vector3(0.1f, 0f, 1.5f)));
                sphere.addComponent(new RandomMover());
                sphere.addComponent(new Momentum());
                sphere.addComponent(new Gravity());
                sphere.addComponent(new TerrainCollider());
                sphere.addComponent(new HitBox(new Vector3(-1, -1, -1), new Vector3(1)));
            }
            

        }
        public void loadTerrain()
        {
            terrainManager.update(camera.getComponent<Transformation>().position);
        }
        public void update(float delta)
        {
            List<Entity> chunks = terrainManager.chunkEntities.Where(kv => kv.Key.Xy == terrainManager.fromWorldToChunkSpace(camera.getComponent<Transformation>().position.Xz)).Select(kv => kv.Value).ToList();
            if (chunks.Count > 0 )
            {
                
                //Console.WriteLine( chunks[0].getComponent<TerrainChunk>().fromGlobalWorldToLocalGrid(camera.getComponent<Transformation>().position.Xz));
            }
            loadTerrain();
            if (InputHandler.isKeyClicked(Keys.T))
            {
                terrainManager.cleanUp();
            } 
            if (InputHandler.isKeyClicked(Keys.G))
            {
                this.sculpture.cleanUp();
                sculpture = new Entity();
                sculpture.addComponent(new Transformation(new Vector3(-15f, -15, 0), new Vector3(0)));
                sculpture.addComponent(glLoader.loadToVAO(ModelGenerator.generateTree()));
                sculpture.addComponent(new TerrainCollider());

                for (int i = 0; i<100; i++)
                {
                    Entity sculpture = new Entity();
                    sculpture.addComponent(new Transformation(new Vector3(MyMath.rngMinusPlus(), -100f, MyMath.rngMinusPlus())*1000f, new Vector3(0)));
                    sculpture.addComponent(glLoader.loadToVAO(ModelGenerator.generateTree()));
                    sculpture.addComponent(new TerrainCollider());
                }
            }
            //camera.updateComponents(delta);
            lock (threadLock)
            {
                foreach (Entity entity in entities)
                {
                    entity.updateComponents(delta);
                }
            }

        }
    }
}
