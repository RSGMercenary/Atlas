using Newtonsoft.Json.Linq;
using System.IO;

namespace Atlas.ECS.Objects
{
	public static class ConfigCreator
	{
		public static void Create(string path)
		{
			if(File.Exists(path))
				return;

			var config = new JObject();
			config["Description"] =
				"These Systems are a prioritized list of {Key, Value} pairs of " +
				"{Interface/Class, Instance} Types. " +
				"When instantiating a System, the Engine will search for " +
				"the {Interface/Class} Type, and then instantiate the {Instance} Type. " +
				"The System.Priority is then set by its priority in the list.";

			config["DeltaFixedTime"] = 0.01666666666;
			config["MaxVariableTime"] = 0.25;

			var systems = new JArray();
			config["Systems"] = systems;

			var system = new JObject();
			system["Key"] = "System.Key.Full.Name";
			system["Value"] = "System.Value.Full.Name";
			systems.Add(system);

			system = new JObject();
			system["Key"] = "System.Key.Full.Name";
			system["Value"] = "System.Value.Full.Name";
			systems.Add(system);

			File.WriteAllText(path, config.ToString());
		}
	}
}
