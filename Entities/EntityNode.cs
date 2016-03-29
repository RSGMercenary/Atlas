using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Entities
{
    class EntityNode
    {
        internal EntityNode previous;
        internal EntityNode next;
        internal Entity entity;

        internal EntityNode()
        {

        }

        public EntityNode Previous
        {
            get
            {
                return previous;
            }
        }

        public EntityNode Next
        {
            get
            {
                return next;
            }
        }

        public Entity Entity
        {
            get
            {
                return entity;
            }
        }
    }
}
