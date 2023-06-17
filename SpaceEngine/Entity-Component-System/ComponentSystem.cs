
namespace SpaceEngine.Entity_Component_System
{
    internal class ComponentSystem
    {
        private List<Component> memberComponents = new List<Component>();

        public ComponentSystem()
        {

        }
        public void addMember(Component member)
        {
            memberComponents.Add(member);
            member.addSubscribedSystem(this);
        }
        public void removeMember(Component member)
        {
            memberComponents.Remove(member);
            member.removeSubscribedSystem(this);

        }

        public List<Component> getMembers()
        {
            return memberComponents;
        }
    }

}
