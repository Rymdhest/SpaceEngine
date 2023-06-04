using SpaceEngine.Util;
using OpenTK.Mathematics;

namespace SpaceEngine.Entity_Component_System.Components
{
    internal class RandomMover : Component
    {
        public override void update(float delta)
        {
            Transformation transformation = owner.getComponent<Transformation>();
            Random rand = new Random();
            transformation.rotation.Y += (rand.NextSingle()-0.5f)*delta*10f;
            Vector4 forward = new Vector4(0f, 0f, -1f, 1f);
            forward = forward * MyMath.createRotationMatrix(transformation.rotation);
            transformation.position += forward.Xyz*delta;
        }
    }
}
