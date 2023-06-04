using SpaceEngine.Modelling;
using SpaceEngine.RenderEngine;
using OpenTK.Mathematics;
using SpaceEngine.Core;
using OpenTK.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpaceEngine.Entity_Component_System.Components
{
    internal class EntityManager
    {

        public List<Entity> entities;
        public List<Entity> renderEntities;
        public Entity camera { get; set; }
        public Entity? terrainChunk;
        public EntityManager() {
            entities= new List<Entity>();
            renderEntities = new List<Entity>();



            camera = new Entity();
            camera.addComponent(new Transformation(posY: 5f));
            camera.addComponent(new InputMove());

            Entity box = new Entity();
            box.addComponent(new Transformation());
            box.addComponent(glLoader.loadToVAO(MeshGenerator.generateBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))));
            renderEntities.Add(box);

            Entity sphere = new Entity();
            sphere.addComponent(new Transformation(new Vector3(0f, 10, 0f), new Vector3(0f,0f,0f)));
            sphere.addComponent(glLoader.loadToVAO(MeshGenerator.generateIcosahedron(1f)));
            renderEntities.Add(sphere);

            loadTerrain();

        }
        public void loadTerrain()
        {
            Console.WriteLine("Terrain");
            if (terrainChunk != null)
            {
                terrainChunk.cleanUp();
                renderEntities.Remove(terrainChunk);
            }
            float size = 200f;
            int detail = 200;
            terrainChunk = new Entity();
            terrainChunk.addComponent(TerrainChunk.generateModel(detail, size));
            terrainChunk.addComponent(new Transformation());
            renderEntities.Add(terrainChunk);
        }
        public void update(float delta)
        {   
            if (InputHandler.isKeyClicked(Keys.T))
            {
                loadTerrain();
            }
            camera.updateComponents(delta);
            foreach(Entity entity in entities)
            {
                entity.updateComponents(delta);
            }
        }
    }
}
