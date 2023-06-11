using OpenTK.Mathematics;
using SpaceEngine.Util;

namespace SpaceEngine.Entity_Component_System.Components
{
    internal class TerrainCollider : Component
    {
        private bool isOnGround = false;
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
                isOnGround = true;
                owner.getComponent<Transformation>().position.Y = groundHeight;
                
                if (owner.hasComponent<Momentum>())
                {
                    if (owner.getComponent<Momentum>().HasMoved())
                    {
                        Vector3 terrainNormal = EntityManager.terrainManager.getNormalFlatAt(position.Xz);
                        Vector3 velocity = owner.getComponent<Momentum>().velocity;
                        float velocityLength = velocity.Length;
                        velocity.Normalize();
                        Vector3 newVelocity = MyMath.reflect(velocity, terrainNormal);
                        owner.getComponent<Momentum>().velocity = newVelocity * velocityLength*0.45f;
                        //owner.getComponent<Momentum>().velocity = new Vector3(0f);
                    }
                }
            } else
            {
                isOnGround = false;
            }
        }
        public bool IsOnGround()
        {
            return isOnGround;
        }
    }
}
