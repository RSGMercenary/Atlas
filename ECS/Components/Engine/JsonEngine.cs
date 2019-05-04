using Atlas.ECS.Systems;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Atlas.ECS.Components
{
	public class JsonEngine : AtlasEngine
	{
		//Configuration
		private string configPath = "";
		private JToken config;

		public JsonEngine(string configPath = "EngineConfig.json")
		{
			ConfigPath = string.IsNullOrWhiteSpace(configPath) ? "EngineConfig.json" : configPath;
		}

		#region Configuration

		public string ConfigPath
		{
			get { return configPath; }
			set
			{
				if(configPath == value)
					return;
				configPath = value;
				ParseConfig();
			}
		}

		private void ParseConfig()
		{
			Create(configPath);
			config = JToken.Parse(File.ReadAllText(configPath));
			DeltaFixedTime = ParseValue(nameof(DeltaFixedTime), DeltaFixedTime);
			MaxVariableTime = ParseValue(nameof(MaxVariableTime), MaxVariableTime);
		}

		private static void Create(string path)
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

		private T ParseValue<T>(string path, T defaultValue)
			where T : struct
		{
			return config.SelectToken(path)?.Value<T>() ?? defaultValue;
		}

		#endregion

		protected override ISystem CreateSystem(Type type)
		{
			var systems = config.SelectToken("Systems") as JArray;
			for(var priority = 0; priority < systems.Count; ++priority)
			{
				var system = systems[priority];
				if(system.SelectToken("Key").Value<string>() != type.FullName)
					continue;

				var fullName = system.SelectToken("Value").Value<string>();
				var assembly = fullName.Split('.')[0];

				type = Type.GetType($"{fullName}, {assembly}", true, true);
				var instance = (ISystem)Activator.CreateInstance(type);
				instance.Priority = priority;

				return instance;
			}
			throw new NullReferenceException($"Couldn't find a System to instantiate for {type.FullName}.");
		}
	}
}