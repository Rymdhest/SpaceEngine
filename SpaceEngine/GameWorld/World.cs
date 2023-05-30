
namespace SpaceEngine.GameWorld;

internal class World
{
    private List<ModelEntity> modelEntities;

    public World() {
        modelEntities = new List<ModelEntity>();
    }

    public void update(float delta)
    {
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
}
