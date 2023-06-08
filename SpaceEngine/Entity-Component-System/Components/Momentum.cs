using OpenTK.Mathematics;
namespace SpaceEngine.Entity_Component_System.Components
{
    internal class Momentum : Component
    {
        public Vector3 velocity;


        public override void update(float delta)
        {
            owner.getComponent<Transformation>().position += velocity * delta;
        }
    }
}
