

using OpenTK.Mathematics;
using SpaceEngine.RenderEngine;
using static System.Formats.Asn1.AsnWriter;

namespace SpaceEngine.GameWorld;

internal class ModelEntity : Entity
{
    private Model model;
    public ModelEntity(Vector3 postion, Vector3 rotation, Model model, float scale = 1)
        : base(postion, rotation, scale)
    {
        this.model = model;
    }

    public void update(float delta)
    {
        base.update(delta);
    }
    public Model GetModel() 
    {
        return model; 
    }
}
