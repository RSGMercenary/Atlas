using Atlas.Components;
using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Systems
{
    sealed class SystemTypeManager : Component
    {
        private HashSet<Type> systemTypes = new HashSet<Type>();
	    private Dictionary<Type, AtlasSystem> systems = new Dictionary<Type, AtlasSystem>();
	    private Signal<SystemTypeManager, Type> systemTypeAdded = new Signal<SystemTypeManager, Type>();
        private Signal<SystemTypeManager, Type> systemTypeRemoved = new Signal<SystemTypeManager, Type>();
        
	    public SystemTypeManager()
	    {
		    
	    }

        protected override void Disposing()
        {
            RemoveSystemTypes();
            systemTypeAdded.Dispose();
            systemTypeRemoved.Dispose();

            base.Disposing();
        }

	    public Signal<SystemTypeManager, Type> SystemTypeAdded
	    {
            get
            {
                return systemTypeAdded;
            }
	    }

        public Signal<SystemTypeManager, Type> SystemTypeRemoved
        {
            get
            {
                return systemTypeRemoved;
            }
        }

        public List<Type> SystemTypes
        {
            get
            {
                return new List<Type>(systemTypes);
            }
        }
        
	    public bool HasSystemType(Type systemType)
	    {
		    return systemTypes.Contains(systemType);
	    }

        /*public Type GetSystemType(Type systemType)
        {
            return systemTypes.Contains(systemType) ? systemTypes[systemType] : null;
        }*/

        public bool AddSystemType(Type systemType)
	    {
		    if(!systemTypes.Contains(systemType))
		    {
			    systemTypes.Add(systemType);
			    systemTypeAdded.Dispatch(this, systemType);
			    return true;
		    }
		    return false;
	    }
	
	    public bool RemoveSystemType(Type systemType)
	    {
		    if(systemTypes.Contains(systemType))
		    {
			    systemTypeRemoved.Dispatch(this, systemType);
			    systemTypes.Remove(systemType);
			    return true;
		    }
		    return false;
	    }
	
	    public void RemoveSystemTypes()
	    {
            /*
		    for(systemClass in this._systemClasses.keys())
		    {
			    this.removeSystemClass(systemClass);
		    }
            */
	    }
	
	    public List<AtlasSystem> Systems
	    {
            get
            {
                return new List<AtlasSystem>(systems.Values);
            }
	    }
	
	    public AtlasSystem GetSystem(Type systemType)
	    {
		    return systems[systemType];
	    }
	
	    internal void AddSystem(AtlasSystem system)
	    {
            /*
		    if(system != null)
		    {
			    if(system.hasSystemClassManager(this))
			    {
				    if(this._systemClasses.exists(system.systemClass))
				    {
					    this._systems.set(system.systemClass, system);
					    //TO-DO :: Add signal?
				    }
			    }
		    }
            */
	    }
	
	    public void RemoveSystem(AtlasSystem system)
	    {
            /*
		    if(system != null)
		    {
			    if(!system.hasSystemClassManager(this))
			    {
				    if(this._systemClasses.exists(system.systemClass))
				    {
					    this._systems.remove(system.systemClass);
					    //TO-DO :: Add signal?
				    }
			    }
		    }
            */
	    }
    }
}