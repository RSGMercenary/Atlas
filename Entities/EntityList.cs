using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Entities
{
    class EntityList
    {
        internal EntityNode first;
        internal EntityNode last;
        private int count = 0;

        internal EntityList()
        {

        }

        public EntityNode First
        {
            get
            {
                return first;
            }
        }

        public EntityNode Last
        {
            get
            {
                return last;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public bool Has(Entity entity)
        {
            for(EntityNode current = first; current != null; current = current.next)
            {
                if(current.entity == entity)
                {
                    return true;
                }
            }
            return false;
        }

        public EntityNode Get(Entity entity)
        {
            for (EntityNode current = first; current != null; current = current.next)
            {
                if (current.entity == entity)
                {
                    return current;
                }
            }
            return null;
        }

        public void Add(Entity entity)
        {
            Add(entity, count);
        }

        public void Add(Entity entity, int index)
        {
            if(!Has(entity))
            {

            }
        }

        public void Remove(Entity entity)
        {
            EntityNode node = Get(entity);
            if(node != null)
            {
                if(node == first)
                {
                    first = first.next;
                }
                if(node == last)
                {
                    last = last.previous;
                }
                if(node.previous != null)
                {
                    node.previous.next = node.next;
                }
                if(node.next != null)
                {
                    node.next.previous = node.previous;
                }
            }
        }
    }
}
