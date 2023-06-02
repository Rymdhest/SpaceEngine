using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceEngine.Entity_Component_System
{
    internal abstract class Component
    {
        public Entity owner { get; set; }


        public virtual void initialize()   { }
        public virtual void cleanUp() { }
        public virtual void update(float delta) { }
    }
}
