using SpaceEngine.Modelling;
using SpaceEngine.RenderEngine;
using OpenTK.Mathematics;

namespace SpaceEngine.Entity_Component_System.Components
{
    internal class EntityManager
    {

        public List<Entity> entities;
        public List<Entity> renderEntities;
        public Entity camera { get; set; }

        public EntityManager() {
            entities= new List<Entity>();
            renderEntities = new List<Entity>();

            float size = 400f;

            camera = new Entity();
            camera.addComponent(new Transformation(posX: size/2f, posY: 5f, posZ: size/2f));
            camera.addComponent(new InputMove());

            Entity box = new Entity();
            box.addComponent(new Transformation());
            box.addComponent(glLoader.loadToVAO(MeshGenerator.generateBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f))));
            renderEntities.Add(box);

            Entity terrainChunk = new Entity();
            terrainChunk.addComponent(TerrainChunk.generateModel(400, size));
            terrainChunk.addComponent(new Transformation());
            renderEntities.Add(terrainChunk);

        }

        public void update(float delta)
        {
            camera.updateComponents(delta);
            foreach(Entity entity in entities)
            {
                entity.updateComponents(delta);
            }
        }
    }
}
