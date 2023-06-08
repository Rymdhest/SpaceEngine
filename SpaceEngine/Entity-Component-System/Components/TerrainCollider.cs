using OpenTK.Mathematics;

namespace SpaceEngine.Entity_Component_System.Components
{
    internal class TerrainCollider : Component
    {
        public override void update(float delta)
        {
            Vector3 position = owner.getComponent<Transformation>().position;
            float groundHeight = EntityManager.terrainManager.getNoiseHeightAt(position.Xz);

            if (owner.hasComponent<HitBox>())
            {
                groundHeight -= owner.getComponent<HitBox>().hitBox.min.Y;
            }
            if (position.Y <= groundHeight)
            {
                owner.getComponent<Transformation>().position.Y = groundHeight;
                
                if (owner.hasComponent<Momentum>())
                {
                    owner.getComponent<Momentum>().velocity.Y = 0f;
                }
            }
        }
    }
}
