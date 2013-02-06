using System;
using Microsoft.SPOT;
using System.Reflection;
using System.Collections;

using IndianaJones.NETMF.String;
using IndianaJones.NETMF.Integer;
using IndianaJones.NETMF.Time;

namespace IndianaJones.NETMF.Json
{
	/// <summary>
	/// .NET Micro Framework JSON Serializer and Deserializer.
	/// Mimics, as closely as possible, the excellent JSON (de)serializer at http://www.json.org.
	/// You can (de)serialize just about any object that contains real property values:
	/// Value Types (int, bool, string, etc), Classes, Arrays, Dictionaries, Hashtables, etc.
	/// Caveats:
	///   1) Each property to be (de)serialized must be public, and contain BOTH a property getter and setter.
	///   2) You can't (de)serialize interfaces, virtual or abstract properties, private properties.
	///      Your class can contain these objects, but their values are not (de)serialized.
	///   3) DateTime objects can be (de)serialized, and their format in JSON will be ISO 8601 format.
	///   4) Guids can be (de)serialized.
	///   3) You can't use Array or IList because they are abstract (or an interface).  Use ArrayList instead.
	///   4) You can't use IDictionaryEntry, use DictionaryEntry instead.
	///   5) .NET MF floating point seems to have very little precision, at least on my GHI USBizi hardware.
	///      I get only about 3 or 4 decimal places of accuracy.
	/// 
	/// How does this class work, given the extremely limited Reflection capabilities?
	/// Serialization is easy: you take the specified object, enumerate its Methods (yes, Methods, there
	/// is no Property enumeration), and find the getters and setters, filtering out the unusable items.
	/// Then you just JSON-format the Property names and their values.
	/// 
	/// Deserialization is much more difficult, as JSON contains no type definitions, and .NET MF contains
	/// next-to-zero assistance.  We instantiate a PropertyTable class to enumerate every public class
	/// that's loaded by this process, and create a non-heirarchical Hashtable list of all public classes,
	/// their property names, and each Property's Type.  When the JSON string is presented for deserialization,
	/// the JSON string is parsed (using this class) into a heirarchical Hashtable, containing all of the properties
	/// and their values.  At that point, you have (a) a Hastable containing Property Names with their Types,
	/// and (b) a Hastable containing the Property Names and their actual Values.  Deserialization at that point
	/// is a matter of matching the Properties in the two lists, instantiating them by their Type, and setting
	/// their Values.  Hoping of course that you contain no classes with identical property names.
	/// 
	/// This is more difficult than it sounds.  In normal .NET, you have boxing/unboxing to convert any object
	/// to any defined Type.  In .NET MF, you do not.  The only Type that an object will unbox into is the
	/// type exposed by the ReturnType property of the Parse method on the object.  So a LOT of wrangling is
	/// needed to properly unbox types from JSON into C#.  But it (mostly) works.
	/// </summary>

	
	/// <summary>
	/// Enumeration of the popular formats of time and date
	/// within Json.  It's not a standard, so you have to
	/// know which on you're using.
	/// </summary>
	public enum DateTimeFormat
	{
		Unknown = 0,
		ISO8601 = 1,
		Ajax = 2
	}
	
	/// <summary>
	/// .NET Micro Framework JSON Serializer and Deserializer.
	/// </summary>
	public class Serializer
	{
		private PropertyTable _propertyTable;
		private DateTimeFormat _dateFormat;

		public Serializer()
		{
			_dateFormat = DateTimeFormat.ISO8601;
			_propertyTable = new PropertyTable();
		}

		/// <summary>
		/// Gets/Sets the format that will be used to display
		/// and parse dates in the Json data.
		/// </summary>
		public DateTimeFormat DateFormat
		{
			get
			{
				return _dateFormat;
			}
			set
			{
				_dateFormat = value;
			}
		}

		/// <summary>
		/// Serializes an object into a Json string.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public string Serialize(object o)
		{
			return JsonPrimitives.Serialize(o);
		}

		/// <summary>
		/// Desrializes a Json string into an object.
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public object Deserialize(string json)
		{
			Hashtable table = JsonParser.JsonDecode(json) as Hashtable;
			JsonPrimitives.DumpObjects(table, 0);

			return _propertyTable.FindObject(table);
		}

		/// <summary>
		/// Resets the contents of the Property Table with current classes and property definitions.
		/// This does NOT need to be called when values change, only when new classes are
		/// loaded dynamically at runtime.
		/// </summary>
		public void Snapshot()
		{
			_propertyTable.Snapshot();
		}
	}

	public static class JsonPrimitives
	{
		/// <summary>
		/// Lookup table for hex values.
		/// </summary>
		public const string ContentType = "application/json";

		/// <summary>
		/// Deserializes the specified Json string into an object whose type matches
		/// what was discovered in the PropertyTable.
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static object Deserialize(string json)
		{
			object result = null;

			Hashtable table = JsonParser.JsonDecode(json) as Hashtable;
			DumpObjects(table, 0);

			return result;
		}

		#region Serialize Methods
		
		/// <summary>
		/// Converts a character to its javascript unicode equivalent.
		/// </summary>
		/// <param name="c">The character to convert.</param>
		/// <returns>The javascript unicode equivalent.</returns>
		private static string JsUnicode(char c)
		{
			string HEX_CHARS = "0123456789ABCDEF";
			string result = "\\u";
			ushort value = (ushort)c;

			for (int i = 0; i < 4; i++, value <<= 4)
			{
				result += HEX_CHARS[value >> 12];
			}

			return result;
		}

		/// <summary>
		/// Convert a value to JSON.
		/// </summary>
		/// <param name="o">The value to convert. Supported types are: Boolean, String, Byte, (U)Int16, (U)Int32, Float, Double, Decimal, JsonObject, JsonArray, Array, Object and null.</param>
		/// <returns>The JSON object as a string or null when the value type is not supported.</returns>
		/// <remarks>For objects, only public fields are converted.</remarks>
		public static string Serialize(object o)
		{
			if (o == null)
				return "null";

			Type type = o.GetType();

			// All ordinary value types and all objects that are classes that can
			// and shouold be ToString()'d are handled here.  Special objects like
			// arrays and classes that have properties to be enumerated are handled below.
			switch (type.Name)
			{
				case "Boolean":
				{
					return (bool)o ? "true" : "false";
				}
				case "String":
				{
					// Encapsulate object in double-quotes if it's not already
					char v = o.ToString()[0];
					if (v == '"')
					{
						return o.ToString();
					}
					else
					{
						return "\"" + o.ToString() + "\"";
					}
				}
				case "Single":
				case "Double":
				case "Decimal":
				case "Float":
				{
					return DoubleExtensions.ToString((double)o);
				}
				case "Byte":
				case "SByte":
				case "Int16":
				case "UInt16":
				case "Int32":
				case "UInt32":
				case "Int64":
				case "UInt64":
				case "JsonObject":
				case "JsonArray":
				{
					return o.ToString();
				}
				case "Char":
				case "Guid":
				{
					return "\"" + o.ToString() + "\"";
				}
				case "DateTime":
				{
					// This MSDN page describes the problem with JSON dates:
					// http://msdn.microsoft.com/en-us/library/bb299886.aspx
					return "\"" + DateTimeExtensions.ToIso8601((DateTime)o) + "\"";
				}

			}

			if (type.IsArray)
			{
				JsonArray jsonArray = new JsonArray();
				foreach (object i in (Array)o)
				{
					// If the array object needs to be serialized first, do it
					object valueToAdd = string.Empty;
					SerializeStatus serialize = GetSerializeState(i);
					if (serialize == SerializeStatus.Serialize)
					{
						valueToAdd = Serialize(i);
					}
					else
					{
						valueToAdd = i;
					}
					jsonArray.Add(valueToAdd);
				}
				return jsonArray.ToString();
			}

			if (type == typeof(System.Collections.ArrayList))
			{
				JsonArray jsonArray = new JsonArray();
				foreach (object i in (o as ArrayList))
				{
					// If the array object needs to be serialized first, do it
					object valueToAdd = string.Empty;
					SerializeStatus serialize = GetSerializeState(i);
					if (serialize == SerializeStatus.Serialize)
					{
						valueToAdd = Serialize(i);
					}
					else
					{
						valueToAdd = i;
					}
					jsonArray.Add(valueToAdd);
				}
				return jsonArray.ToString();
			}

			if (type == typeof(System.Collections.Hashtable))
			{
				JsonObject main = new JsonObject();
				Hashtable table = o as Hashtable;
				JsonObject to = new JsonObject();
				foreach (var key in table.Keys)
				{
					// If the array object needs to be serialized first, do it
					object valueToAdd = string.Empty;
					SerializeStatus serialize = GetSerializeState(table[key]);
					if (serialize == SerializeStatus.Serialize)
					{
						valueToAdd = Serialize(table[key]);
					}
					else
					{
						valueToAdd = table[key];
					}

					to.Add(key, valueToAdd);
					//to.Add(key, table[key]);
				}
				//main.Add(type.Name, to.ToString());
				//return main.ToString();
				return to.ToString();
			}

			if (type == typeof(System.Collections.DictionaryEntry))
			{
				DictionaryEntry dict = o as DictionaryEntry;
				JsonObject to = new JsonObject();

				// If the Value property of the DictionaryEntry is an object rather
				// than a string, then serialize it first.
				object valueToAdd = string.Empty;
				SerializeStatus serialize = GetSerializeState(dict.Value);
				if (serialize == SerializeStatus.Serialize)
				{
					valueToAdd = Serialize(dict.Value);
				}
				else
				{
					valueToAdd = dict.Value;
				}

				to.Add(dict.Key, valueToAdd);

				return to.ToString();
			}

			if (type.IsClass)
			{
				JsonObject jsonObject = new JsonObject();

				// Iterate through all of the methods, looking for GET properties
				MethodInfo[] methods = type.GetMethods();
				foreach (MethodInfo method in methods)
				{
					// We care only about property getters when serializing
					if (method.Name.StartsWith("get_"))
					{
						// Ignore abstract and virtual objects
						if ((method.IsAbstract ||
							(method.IsVirtual) ||
							(method.ReturnType.IsAbstract)))
						{
							continue;
						}

						// Ignore delegates and MethodInfos
						if ((method.ReturnType == typeof(System.Delegate)) ||
							(method.ReturnType == typeof(System.MulticastDelegate)) ||
							(method.ReturnType == typeof(System.Reflection.MethodInfo)))
						{
							continue;
						}
						// Ditto for DeclaringType
						if ((method.DeclaringType == typeof(System.Delegate)) ||
							(method.DeclaringType == typeof(System.MulticastDelegate)))
						{
							continue;
						}

						// If the property returns a Hashtable
						if (method.ReturnType == typeof(System.Collections.Hashtable))
						{
							Hashtable table = method.Invoke(o, null) as Hashtable;
							JsonObject to = new JsonObject();
							foreach (var key in table.Keys)
							{
								// If the array object needs to be serialized first, do it
								object valueToAdd = string.Empty;
								SerializeStatus serialize = GetSerializeState(table[key]);
								if (serialize == SerializeStatus.Serialize)
								{
									valueToAdd = Serialize(table[key]);
								}
								else
								{
									valueToAdd = table[key];
								}
								to.Add(key, valueToAdd);
								//to.Add(key, table[key]);
							}
							jsonObject.Add(method.Name.Substring(4), to.ToString());
							continue;
						}

						// If the property returns an array of objects
						if (method.ReturnType == typeof(System.Collections.ArrayList))
						{
							ArrayList no = method.Invoke(o, null) as ArrayList;
							JsonArray jsonArray = new JsonArray();
							foreach (object i in no)
							{
								// If the array object needs to be serialized first, do it
								object valueToAdd = string.Empty;
								SerializeStatus serialize = GetSerializeState(i);
								if (serialize == SerializeStatus.Serialize)
								{
									valueToAdd = Serialize(i);
								}
								else
								{
									valueToAdd = i;
								}
								jsonArray.Add(valueToAdd);
							}
							jsonObject.Add(method.Name.Substring(4), jsonArray.ToString());
							continue;
						}

						// If the property returns a DictionaryEntry
						if (method.ReturnType == typeof(System.Collections.DictionaryEntry))
						{
							DictionaryEntry dict = method.Invoke(o, null) as DictionaryEntry;

							// If the Value property of the DictionaryEntry needs to be serialized first, do it
							object valueToAdd = string.Empty;
							SerializeStatus serialize = GetSerializeState(dict.Value);
							if (serialize == SerializeStatus.Serialize)
							{
								valueToAdd = Serialize(dict.Value);
							}
							else
							{
								valueToAdd = dict.Value;
							}

							// Wrap the DictionaryEntry in a JsonObject
							JsonObject to = new JsonObject();
							to.Add(dict.Key, valueToAdd);
							jsonObject.Add(method.Name.Substring(4), to.ToString());
							continue;
						}

						// If the property is a Class that should NOT be ToString()'d, because
						// it has properties that must themselves be enumerated and serialized,
						// then recursively call myself to serialize them.
						if ((method.ReturnType.IsClass) &&
							(method.ReturnType.IsArray == false) &&
							(method.ReturnType.ToString().StartsWith("System.Collections") == false) &&
							(method.ReturnType.ToString().StartsWith("System.String") == false))
						{
							object no = method.Invoke(o, null);
							string value = Serialize(no);
							jsonObject.Add(method.Name.Substring(4), value);
							continue;
						}

						// All other properties are types that will be handled according to 
						// their type.  That handler code is the switch statement at the top
						// of this function.
						object newo = method.Invoke(o, null);
						jsonObject.Add(method.Name.Substring(4), newo);


					}
				}
				return jsonObject.ToString();
			}

			return null;
		}

		public enum SerializeStatus
		{
			None = 0,
			Serialize = 1
		}

		/// <summary>
		/// Determines if the specified object needs to be serialized.  It needs to be serialized if it's a 
		/// class that contains properties that need enumeration.  All other objects that can be directly
		/// returned, such as ints, strings, etc, do not need to be serialized.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		private static SerializeStatus GetSerializeState(object o)
		{
			Type type = o.GetType();

			// Ignore delegates and MethodInfos
			if ((type == typeof(System.Delegate)) ||
				(type == typeof(System.MulticastDelegate)) ||
				(type == typeof(System.Reflection.MethodInfo)))
			{
				return SerializeStatus.None;
			}

			// If the property returns a Hashtable
			if (type == typeof(System.Collections.Hashtable))
			{
				return SerializeStatus.None;
			}

			// If the property returns an array of objects
			if (type == typeof(System.Collections.ArrayList))
			{
				return SerializeStatus.Serialize;
			}

			// If the property returns a DictionaryEntry
			if (type == typeof(System.Collections.DictionaryEntry))
			{
				return SerializeStatus.Serialize;
			}

			// If the property is a Class that should NOT be ToString()'d, because
			// it has properties that must themselves be enumerated and serialized,
			// then recursively call myself to serialize them.
			if ((type.IsClass) &&
				(type.IsArray == false) &&
				(type.ToString().StartsWith("System.Collections") == false) &&
				(type.ToString().StartsWith("System.String") == false))
			{
				return SerializeStatus.Serialize;
			}

			// All other properties are types that will be handled according to 
			// their type.  That handler code is the switch statement at the top
			// of this function.
			return SerializeStatus.None;
		}

		#endregion

		#region Deserialize Methods

		public static void DumpObjects(Hashtable hash, int level)
		{
			foreach (DictionaryEntry d in hash)
			{
				string name = d.Key.ToString();
				string value = string.Empty;
				string tabs = string.Empty;
				for (int i = 0; i < level; i++)
				{
					tabs = tabs + " ";
				}
				Debug.Print(tabs + name + " : ");

				if (d.Value is Hashtable)
				{
					DumpObjects(d.Value as Hashtable, level + 4);
				}
				if (d.Value is ArrayList)
				{
					DumpObjectArray(d.Value as ArrayList, level + 4);
				}
				else
				{
					Debug.Print(d.Value.ToString());
				}
			}

		}

		private static void DumpObjectArray(ArrayList array, int level)
		{
			foreach (object o in array)
			{
				if (o is Hashtable)
				{
					DumpObjects(o as Hashtable, level + 4);
				}
				else if (o is ArrayList)
				{
					DumpObjectArray(o as ArrayList, level + 4);
				}
				else
				{
					Debug.Print(o.ToString());
				}
			}
		}

		

		#endregion
	}
}
