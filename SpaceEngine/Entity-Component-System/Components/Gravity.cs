
namespace SpaceEngine.Entity_Component_System.Components
{
    internal class Gravity : Component
    {
        public static float gravityConstant = 9f;


        public override void update(float delta)
        {
            owner.getComponent<Momentum>().velocity.Y -= gravityConstant*delta;
        }
    }
}
