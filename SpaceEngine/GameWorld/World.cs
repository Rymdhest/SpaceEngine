
using OpenTK.Mathematics;
namespace SpaceEngine.GameWorld;

internal class World
{
    private List<ModelEntity> modelEntities;
    private Camera camera;

    public World() {
        modelEntities = new List<ModelEntity>();
        camera = new Camera(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
    }

    public void update(float delta)
    {
        camera.update(delta);
        foreach (var entity in modelEntities) {
            entity.update(delta);
        }
    }

    public void addModelEntity(ModelEntity entity)
    {
        modelEntities.Add(entity);
    }
    public void removeModelEntity(ModelEntity entity)
    {
        modelEntities.Remove(entity);
    }
    public List<ModelEntity> getModelEntities()
    {
        return modelEntities;
    }
    public Camera getCamera()
    {
        return camera;
    }
}
