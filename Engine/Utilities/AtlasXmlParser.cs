using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Atlas.Engine.Utilities
{
	static class AtlasXmlParser
	{
		public static object create(XmlNode xml, Type objectClass = null)
		{
			object instance = construct(xml, objectClass);
			SetMembers(instance, xml);
			return instance;
		}

		public static object construct(XmlNode xml, Type objectClass = null)
		{
			if(objectClass == null)
				objectClass = Type.GetType(xml.Attributes["type"].Value, false);
			if(objectClass == null)
				return null;
			List<object> properties = null;
			XmlNode constructor = xml.SelectSingleNode("constructor");
			if(constructor != null)
			{
				properties = GetMembers(constructor);
			}
			//Account for null object params.
			return Activator.CreateInstance(objectClass, properties.ToArray());
		}

		public static List<object> GetMembers(XmlNode xml)
		{
			List<object> members = new List<object>();
			if(xml == null)
				return members;
			for(var current = xml.FirstChild; current != null; current = current.NextSibling)
			{
				members.Add(GetMember(current));
			}
			return members;
		}

		public static object GetMember(XmlNode xml, Type type = null)
		{
			if(xml == null)
				return null;
			if(type == null)
				type = Type.GetType(xml.Attributes["class"].Value, false);
			if(type == null)
				return null;
			if(type.IsPrimitive)
			{
				return xml.Value;
			}
			else if(type == typeof(Delegate))
			{
				XmlNode parameters = xml.SelectSingleNode("parameters");
				if(parameters != null)
				{
					return GetMembers(parameters);
				}
				else
				{
					return null;
				}
			}
			else if(type == typeof(Type))
			{
				return Type.GetType(xml.Value, false);
			}
			else
			{
				return create(xml, type);
			}
		}

		public static void SetMembers(object instance, XmlNode xml)
		{
			if(instance == null)
				return;
			if(xml == null)
				return;
			XmlNode properties = xml.SelectSingleNode("properties");
			if(properties == null)
				return;
			for(var current = properties.FirstChild; current != null; current = current.NextSibling)
			{
				SetMember(instance, xml);
			}
		}

		public static void SetMember(object instance, XmlNode xml)
		{
			if(instance == null)
				return;
			if(xml == null)
				return;
			string property = xml.LocalName;

			Type type = Type.GetType(xml.Attributes["type"].Value, false);

			if(type == null)
				return;

			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
			Type instanceType = instance.GetType();

			if(type == typeof(Delegate))
			{
				MethodInfo methodInfo = instanceType.GetMethod(property, flags);
				if(methodInfo != null)
					methodInfo.Invoke(instance, null);
				//instance[property].apply(null, AtlasXmlParser.getProperty(xml, objectClass));
			}
			else
			{
				if(xml.Attributes["constructed"].Value.ToLower() != bool.TrueString.ToLower())
				{
					FieldInfo fieldInfo = instanceType.GetField(property, flags);
					if(fieldInfo != null)
					{
						fieldInfo.SetValue(instance, GetMember(xml, type));
					}
					else
					{
						PropertyInfo propertyInfo = instanceType.GetProperty(property, flags);
						propertyInfo.SetValue(instance, GetMember(xml, type));
					}
				}
				else
				{
					FieldInfo fieldInfo = instanceType.GetField(property, flags);
					if(fieldInfo != null)
					{
						SetMembers(fieldInfo.GetValue(instance), xml);
					}
					else
					{
						PropertyInfo propertyInfo = instanceType.GetProperty(property, flags);
						SetMembers(propertyInfo.GetValue(instance), xml);
					}
				}
			}
		}
	}
}
