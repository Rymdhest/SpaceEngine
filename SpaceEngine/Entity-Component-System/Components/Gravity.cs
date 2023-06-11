
namespace SpaceEngine.Entity_Component_System.Components
{
    internal class Gravity : Component
    {
        public static float gravityConstant = 9f;


        public override void update(float delta)
        {
            float ground = 1.0f;
            if (owner.hasComponent<TerrainCollider>())
            {
                if (owner.getComponent<TerrainCollider>().IsOnGround())
                {
                    ground = 0.0f;
                }
            }
            owner.getComponent<Momentum>().velocity.Y -= gravityConstant * delta*ground;
        }
    }
}
