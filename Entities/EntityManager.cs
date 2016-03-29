using Atlas.Components;
using Atlas.Signals;
using System.Collections.Generic;

namespace Atlas.Entities
{
    sealed class EntityManager : Component
    {
        private List<Entity> entities = new List<Entity>();
	    private Dictionary<string, Entity> uniqueNames = new Dictionary<string, Entity>();
	    private Signal<EntityManager, Entity> entityAdded = new Signal<EntityManager, Entity>();
        private Signal<EntityManager, Entity> entityRemoved = new Signal<EntityManager, Entity>();

        public EntityManager()
        {

        }

        override protected void AddingComponentManager(Entity entity)
	    {
		    base.AddingComponentManager(entity);
		    AddEntity(entity);
        }

        override protected void RemovingComponentManager(Entity entity)
	    {
            RemoveEntity(entity);
            base.RemovingComponentManager(entity);
        }

        public bool IsUniqueName(string uniqueName)
	    {
		    return !string.IsNullOrWhiteSpace(uniqueName) && !uniqueNames.ContainsKey(uniqueName);
	    }

        public bool HasEntity(Entity entity)
	    {
            return entity != null && entities.Contains(entity);
	    }

        public string GetUniqueName()
	    {
		    for(int index = 0; index < int.MaxValue; ++index)
		    {
			    if(!uniqueNames.ContainsKey("instance" + index))
			    {
				    return "instance" + index;
			    }
            }
		    return "";
	    }

        public Signal<EntityManager, Entity> EntityAdded
        {
            get
            {
                return entityAdded;
            }
        }

        public Signal<EntityManager, Entity> EntityRemoved
        {
            get
            {
                return entityRemoved;
            }
        }

        public Entity GetEntityAt(int index)
	    {
		    if(index < 0) 					return null;
		    if(index > entities.Count - 1) 	return null;
		    return entities[index];
	    }

        public int GetEntityIndex(Entity entity)
	    {
		    return entities.IndexOf(entity);
	    }

        public bool SetEntityIndex(Entity entity, int index)
	    {
		    if(index < 0)				    return false;
		    if(index > entities.Count - 1) 	return false;

            int previousIndex = entities.IndexOf(entity);
		
		    if(previousIndex == index) 	return true;
		    if(previousIndex < 0) 		return false;
		
		    //-1 is needed since technically you lost a child on the previous splice.
		    if(index > previousIndex)
            {
                --index;
            }
		
		    entities.RemoveAt(previousIndex);
		    entities.Insert(index, entity);
		    return true;
        }

        public int NumEntities
        {
            get
            {
                return entities.Count;
            }
        }

        public Entity GetEntityByUniqueName(string uniqueName)
	    {
		    return uniqueNames[uniqueName];
	    }

        public List<Entity> Entities
        {
            get
            {
                return new List<Entity>(entities);
            }
        }

        private void AddEntity(Entity entity)
	    {
		    if(string.IsNullOrWhiteSpace(entity.UniqueName) || (uniqueNames.ContainsKey(entity.UniqueName) && uniqueNames[entity.UniqueName] != entity))
		    {
			    entity.UniqueName = GetUniqueName();
		    }
		    if(!uniqueNames.ContainsKey(entity.UniqueName))
            {
                uniqueNames.Add(entity.UniqueName, entity);
                entities.Add(entity);

                entity.ChildAdded.Add(ChildAdded, int.MinValue);
                entity.ParentChanged.Add(ParentChanged, int.MinValue);
                entity.UniqueNameChanged.Add(UniqueNameChanged, int.MinValue);

                entity.EntityManager = this;

                entityAdded.Dispatch(this, entity);

                if(entity.NumChildren > 0)
                {
                    foreach(Entity child in entity.Children)
                    {
                        AddEntity(child);
                    }
                }
            }
        }

        private void RemoveEntity(Entity entity)
	    {
            if(entity.NumChildren > 0)
            {
                foreach(Entity child in entity.Children)
                {
                    RemoveEntity(child);
                }
            }

            entityRemoved.Dispatch(this, entity);
		
		    uniqueNames.Remove(entity.UniqueName);
            entities.Remove(entity);

            entity.ChildAdded.Remove(ChildAdded);
            entity.ParentChanged.Remove(ParentChanged);
            entity.UniqueNameChanged.Remove(UniqueNameChanged);

            entity.EntityManager = null;
        }

        private void ChildAdded(Entity parent, Entity child)
	    {
            AddEntity(child);
        }

        private void ParentChanged(Entity child, Entity previousParent)
	    {
		    if(child.Parent == null)
		    {
                RemoveEntity(child);
			
			    if(child.IsDisposedWhenUnmanaged)
                {
                    child.Dispose();
                }
            }
        }

        private void UniqueNameChanged(Entity entity, string previousUniqueName)
	    {
            uniqueNames.Remove(previousUniqueName);
		    uniqueNames.Add(entity.UniqueName, entity);
        }
    }
}
