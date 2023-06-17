using SpaceEngine.Modelling;
using SpaceEngine.RenderEngine;
using OpenTK.Mathematics;
using SpaceEngine.Core;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceEngine.Util;
using System.Drawing;

namespace SpaceEngine.Entity_Component_System.Components
{
    internal class EntityManager
    {

        public static List<Entity> entities = new List<Entity>();
        public static ComponentSystem flatShadingSystem = new ComponentSystem();
        public static ComponentSystem smoothShadingSystem = new ComponentSystem();
        public static ComponentSystem postGeometrySystem = new ComponentSystem();
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
            camera.addComponent(new TerrainCollider());
            camera.addComponent(new HitBox(new Vector3(-2), new Vector3(2)));

            Entity box = new Entity();
            box.addComponent(new Transformation(new Vector3(0, 15, 0), new Vector3(0)));
            box.addComponent(new Model( glLoader.loadToVAO(MeshGenerator.generateBox(new Vector3(-0.5f), new Vector3(0.5f))), MasterRenderer.Pipeline.FLAT_SHADING));
            box.addComponent(new PointLight(new Vector3(1f, 0.2f, 0.2f)*10f, new Vector3(0.1f, 0f, 1.5f)));

            Entity sun = new Entity();
            sun.addComponent(new Sun());
            this.sun = sun;


            sculpture.addComponent(new Transformation(new Vector3(-15f, -15, 0), new Vector3(0)));
            sculpture.addComponent(new Model(glLoader.loadToVAO(ModelGenerator.generateTree()), MasterRenderer.Pipeline.FLAT_SHADING));
            sculpture.addComponent(new TerrainCollider());
        }
        public void loadTerrain()
        {
            terrainManager.update(camera.getComponent<Transformation>().position);
        }
        public void update(float delta)
        {
            if (InputHandler.isKeyClicked(Keys.G))
            {
                Vector3 forward = camera.getComponent<Transformation>().createForwardVector();
                Vector3 center = camera.getComponent<Transformation>().position;
                Vector3 color = new Vector3(MyMath.rng(), MyMath.rng(), MyMath.rng());
                Entity sphere = new Entity();
                float power = (MyMath.rng() * 5f + 1) * 0.5f;
                sphere.addComponent(new Transformation(center+ forward*1.5f, new Vector3(0f, 0f, 0f), MathF.Sqrt(power)));
                RawModel rawModel = MeshGenerator.generateIcosahedron(new Vector3(1.0f), color * MathF.Sqrt(power), MasterRenderer.Pipeline.POST_GEOMETRY);
                rawModel.setBloom(3f);
                sphere.addComponent( new Model( glLoader.loadToVAO(rawModel), MasterRenderer.Pipeline.POST_GEOMETRY));
                sphere.addComponent(new PointLight(color * power, new Vector3(0.1f, 0f, 1.5f)));
                sphere.addComponent(new Momentum(forward * 50f));
                sphere.addComponent(new TerrainCollider());
                sphere.addComponent(new Gravity());
                sphere.addComponent(new HitBox(new Vector3(-MathF.Sqrt(power) / 2f), new Vector3(MathF.Sqrt(power) / 2f)));
            }
            if (InputHandler.isKeyDown(Keys.B))
            {
                for (int i = 0; i<300*delta; i++)
                {
                    Vector3 randOffset = MyMath.rng3DMinusPlus();
                    randOffset = camera.getComponent<Transformation>().createForwardVector(randOffset);
                    Vector3 forward = camera.getComponent<Transformation>().createForwardVector();

                    Vector3 center = camera.getComponent<Transformation>().position;
                    Vector3 color = new Vector3(MyMath.rng(), MyMath.rng(), MyMath.rng())*10;
                    Entity sphere = new Entity();
                    float power = (MyMath.rng() * 5f + 1) * 0.5f;
                    sphere.addComponent(new Transformation(center + forward*1.5f+randOffset*1f, new Vector3(0f, 0f, 0f), MathF.Sqrt(power)));
                    sphere.addComponent(new PointLight(color * power, new Vector3(0.1f, 0f, 1.5f)));
                    sphere.addComponent(new GlowEffect(color, MathF.Sqrt(power)));
                    sphere.addComponent(new Momentum(forward * 50f+randOffset*10f));
                    sphere.addComponent(new TerrainCollider());
                    sphere.addComponent(new Gravity());
                    sphere.addComponent(new HitBox(new Vector3(-MathF.Sqrt(power) / 2f), new Vector3(MathF.Sqrt(power) / 2f)));
                }

            }

            postGeometrySystem.getMembers().Sort((v1, v2) => (v2.owner.getComponent<Transformation>().position - camera.getComponent<Transformation>().position).LengthSquared.CompareTo((v1.owner.getComponent<Transformation>().position - camera.getComponent<Transformation>().position).LengthSquared));

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
            if (InputHandler.isKeyClicked(Keys.H))
            {
                this.sculpture.cleanUp();
                sculpture = new Entity();
                sculpture.addComponent(new Transformation(new Vector3(-15f, -15, 0), new Vector3(0)));
                sculpture.addComponent(new Model( glLoader.loadToVAO(ModelGenerator.generateTree()), MasterRenderer.Pipeline.FLAT_SHADING));
                sculpture.addComponent(new TerrainCollider());

                for (int i = 0; i<100; i++)
                {
                    Entity sculpture = new Entity();
                    Vector3 position = MyMath.rng3DMinusPlus() * 200;
                    position.Y = terrainManager.getNoiseHeightAt(position.Xz)-1.0f;
                    sculpture.addComponent(new Transformation(position, new Vector3(0, MyMath.rng()*MathF.PI*2f,0), 0.8f+MyMath.rng()*0.4f));
                    sculpture.addComponent(new Model(ModelGenerator.tree, MasterRenderer.Pipeline.FLAT_SHADING));
                }
            }
            //camera.updateComponents(delta);
            //lock (threadLock)
            {
                foreach (Entity entity in entities)
                {
                    entity.updateComponents(delta);
                }
            }

        }
    }
}
