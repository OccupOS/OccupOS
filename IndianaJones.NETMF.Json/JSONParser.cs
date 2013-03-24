using System;
using System.Collections;
using System.Globalization;

using IndianaJones.NETMF.String;
using IndianaJones.NETMF.Integer;

namespace IndianaJones.NETMF.Json
{
	/// <summary>
	/// Parses JSON strings into a Hashtable.  The Hashtable contains one or more key/value pairs
	/// (DictionaryEntry objects).  Each key is the name of a property that (hopefully) exists
	/// in the class object that it represents.  Each value is one of the following:
	///   Hastable - Another list of one or more DictionaryEntry objects, essentially representing
	///              a property that is another class.
	///   ArrayList - An array of one or more objects, which themselves can be one of the items
	///               enumerated in this list.
	///   Value Type - an actual value, such as a string, int, bool, Guid, DateTime, etc
	/// </summary>
	public class JsonParser
	{
		public enum Token
		{
			None = 0,
			ObjectBegin,				// {
			ObjectEnd,					// }
			ArrayBegin,					// [
			ArrayEnd,					// ]
			PropertySeparator,			// :
			ItemsSeparator,				// ,
			StringType,					// "  <-- string of characters
			NumberType,					// 0-9  <-- number, fixed or floating point
			BooleanTrue,				// true
			BooleanFalse,				// false
			NullType					// null
		}

		private const int BUILDER_CAPACITY = 2000;

		/// <summary>
		/// Parses the string json into a value
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
		public static object JsonDecode(string json)
		{
			bool success = true;
			
			return JsonDecode(json, ref success);
		}

		/// <summary>
		/// Parses the string json into a value; and fills 'success' with the successfullness of the parse.
		/// </summary>
		/// <param name="json">A JSON string.</param>
		/// <param name="success">Successful parse?</param>
		/// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
		public static object JsonDecode(string json, ref bool success)
		{
			success = true;
			if (json != null)
			{
                char[] charArray = json.ToCharArray();
                int index = 0;
				object value = ParseValue(charArray, ref index, ref success);
				return value;
            }
			else
			{
                return null;
            }
		}

		protected static Hashtable ParseObject(char[] json, ref int index, ref bool success)
		{
			Hashtable table = new Hashtable();
			Token token;

			// {
			NextToken(json, ref index);

			bool done = false;
			while (!done)
			{
				token = LookAhead(json, index);
				if (token == JsonParser.Token.None)
				{
					success = false;
					return null;
				}
				else if (token == JsonParser.Token.ItemsSeparator)
				{
					NextToken(json, ref index);
				}
				else if (token == JsonParser.Token.ObjectEnd)
				{
					NextToken(json, ref index);
					return table;
				}
				else
				{

					// name
					string name = ParseString(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}

					// :
					token = NextToken(json, ref index);
					if (token != JsonParser.Token.PropertySeparator)
					{
						success = false;
						return null;
					}

					// value
					object value = ParseValue(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}

					table[name] = value;
				}
			}

			return table;
		}

		protected static ArrayList ParseArray(char[] json, ref int index, ref bool success)
		{
			ArrayList array = new ArrayList();

			// [
			NextToken(json, ref index);

			bool done = false;
			while (!done)
			{
				Token token = LookAhead(json, index);
				if (token == JsonParser.Token.None)
				{
					success = false;
					return null;
				}
				else if (token == JsonParser.Token.ItemsSeparator)
				{
					NextToken(json, ref index);
				}
				else if (token == JsonParser.Token.ArrayEnd)
				{
					NextToken(json, ref index);
					break;
				}
				else
				{
					object value = ParseValue(json, ref index, ref success);
					if (!success)
					{
						return null;
					}

					array.Add(value);
				}
			}

			return array;
		}

		protected static object ParseValue(char[] json, ref int index, ref bool success)
		{
			switch (LookAhead(json, index))
			{
				case JsonParser.Token.StringType:
					return ParseString(json, ref index, ref success);

				case JsonParser.Token.NumberType:
					return ParseNumber(json, ref index, ref success);

				case JsonParser.Token.ObjectBegin:
					return ParseObject(json, ref index, ref success);

				case JsonParser.Token.ArrayBegin:
					return ParseArray(json, ref index, ref success);

				case JsonParser.Token.BooleanTrue:
					NextToken(json, ref index);
					return true;

				case JsonParser.Token.BooleanFalse:
					NextToken(json, ref index);
					return false;

				case JsonParser.Token.NullType:
					NextToken(json, ref index);
					return null;

				case JsonParser.Token.None:
					break;
			}

			success = false;
			return null;
		}

		protected static string ParseString(char[] json, ref int index, ref bool success)
		{
			StringBuilder s = new StringBuilder(BUILDER_CAPACITY);
			char c;

			EatWhitespace(json, ref index);
			
			// "
			c = json[index++];

			bool complete = false;
			while (!complete)
			{
				if (index == json.Length)
				{
					break;
				}

				c = json[index++];
				if (c == '"')
				{
					complete = true;
					break;
				}
				else if (c == '\\')
				{
					if (index == json.Length)
					{
						break;
					}

					c = json[index++];
					if (c == '"')
					{
						s.Append('"');
					}
					else if (c == '\\')
					{
						s.Append('\\');
					}
					else if (c == '/')
					{
						s.Append('/');
					}
					else if (c == 'b')
					{
						s.Append('\b');
					}
					else if (c == 'f')
					{
						s.Append('\f');
					}
					else if (c == 'n')
					{
						s.Append('\n');
					}
					else if (c == 'r')
					{
						s.Append('\r');
					}
					else if (c == 't')
					{
						s.Append('\t');
					}
					else if (c == 'u')
					{
						int remainingLength = json.Length - index;
						if (remainingLength >= 4)
						{
							// parse the 32 bit hex into an integer codepoint
							uint codePoint;
							if (!(success = UInt32Extensions.TryParse(new string(json, index, 4), NumberStyle.Hexadecimal, out codePoint)))
							{
								return "";
							}

							// convert the integer codepoint to a unicode char and add to string
							s.Append(CharExtensions.ConvertFromUtf32((int)codePoint));

							// skip 4 chars
							index += 4;
						}
						else
						{
							break;
						}
					}

				}
				else
				{
					s.Append(c);
				}

			}

			if (!complete)
			{
				success = false;
				return null;
			}

			return s.ToString();
		}

		/// <summary>
		/// Determines the type of number (int, double, etc) and returns an object
		/// containing that value.
		/// </summary>
		/// <param name="json"></param>
		/// <param name="index"></param>
		/// <param name="success"></param>
		/// <returns></returns>
		protected static object ParseNumber(char[] json, ref int index, ref bool success)
		{
			EatWhitespace(json, ref index);

			int lastIndex = GetLastIndexOfNumber(json, index);
			int charLength = (lastIndex - index) + 1;

			// We now have the number as a string.  Parse it to determine the type of number.
			string value = new string(json, index, charLength);

			// Since the Json doesn't contain the Type of the property, and since multiple number
			// values can fit in the various Types (e.g. 33 decimal fits in an Int16, UInt16,
			// Int32, UInt32, Int64, and UInt64), we need to be a bit smarter in how we deal with
			// the size of the number, and also the case (negative or positive).
			object result = null;
			string dot = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
			string comma = CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator;
			string minus = CultureInfo.CurrentUICulture.NumberFormat.NegativeSign;
			string plus = CultureInfo.CurrentUICulture.NumberFormat.PositiveSign;
			if (value.Contains(dot) || value.Contains(comma) || value.Contains("e") || value.Contains("E"))
			{
				// We have either a double or a float.  Force it to be a double
				// and let the deserializer unbox it into the proper size.
				result = Double.Parse(new string(json, index, charLength));
			}
			else
			{
				NumberStyle style = NumberStyle.Decimal;
				if(value.StartsWith("0x") || (value.IndexOfAny(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F' }) >= 0))
				{
					style = NumberStyle.Hexadecimal;
				}

				// If it's an integer, force it to either signed or unsigned 64-bit.
				// The deserializer will then do unboxing to fit it into the proper size.
				if(value.StartsWith(minus) || value.StartsWith(plus))
				{
				    result = Int64Extensions.Parse(value, style);
				}
				else
				{
				    result = UInt64Extensions.Parse(value, style);
				}
			}

			index = lastIndex + 1;

			return result;
		}

		protected static int GetLastIndexOfNumber(char[] json, int index)
		{
			int lastIndex;

			for (lastIndex = index; lastIndex < json.Length; lastIndex++) {
				if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1) {
					break;
				}
			}
			return lastIndex - 1;
		}

		protected static void EatWhitespace(char[] json, ref int index)
		{
			for (; index < json.Length; index++) {
				if (" \t\n\r".IndexOf(json[index]) == -1) {
					break;
				}
			}
		}

		protected static Token LookAhead(char[] json, int index)
		{
			int saveIndex = index;
			return NextToken(json, ref saveIndex);
		}

		protected static Token NextToken(char[] json, ref int index)
		{
			EatWhitespace(json, ref index);

			if (index == json.Length) {
				return JsonParser.Token.None;
			}
			
			char c = json[index];
			index++;
			switch (c) {
				case '{':
					return JsonParser.Token.ObjectBegin;
				case '}':
					return JsonParser.Token.ObjectEnd;
				case '[':
					return JsonParser.Token.ArrayBegin;
				case ']':
					return JsonParser.Token.ArrayEnd;
				case ',':
					return JsonParser.Token.ItemsSeparator;
				case '"':
					return JsonParser.Token.StringType;
				case '0': case '1': case '2': case '3': case '4': 
				case '5': case '6': case '7': case '8': case '9':
				case '-':
					return JsonParser.Token.NumberType;
				case ':':
					return JsonParser.Token.PropertySeparator;
			}
			index--;

			int remainingLength = json.Length - index;

			// false
			if (remainingLength >= 5) {
				if (json[index] == 'f' &&
					json[index + 1] == 'a' &&
					json[index + 2] == 'l' &&
					json[index + 3] == 's' &&
					json[index + 4] == 'e') {
					index += 5;
					return JsonParser.Token.BooleanFalse;
				}
			}

			// true
			if (remainingLength >= 4) {
				if (json[index] == 't' &&
					json[index + 1] == 'r' &&
					json[index + 2] == 'u' &&
					json[index + 3] == 'e') {
					index += 4;
					return JsonParser.Token.BooleanTrue;
				}
			}

			// null
			if (remainingLength >= 4) {
				if (json[index] == 'n' &&
					json[index + 1] == 'u' &&
					json[index + 2] == 'l' &&
					json[index + 3] == 'l') {
					index += 4;
					return JsonParser.Token.NullType;
				}
			}

			return JsonParser.Token.None;
		}

		protected static bool SerializeValue(object value, StringBuilder builder)
		{
			bool success = true;

			if (value is string) {
				success = SerializeString((string)value, builder);
			} else if (value is Hashtable) {
				success = SerializeObject((Hashtable)value, builder);
			} else if (value is ArrayList) {
				success = SerializeArray((ArrayList)value, builder);
			} else if (IsNumeric(value)) {
				success = SerializeNumber(Convert.ToDouble(value.ToString()), builder);
			} else if ((value is Boolean) && ((Boolean)value == true)) {
				builder.Append("true");
			} else if ((value is Boolean) && ((Boolean)value == false)) {
				builder.Append("false");
			} else if (value == null) {
				builder.Append("null");
			} else {
				success = false;
			}
			return success;
		}
		
		protected static bool SerializeObject(Hashtable anObject, StringBuilder builder)
		{
			builder.Append("{");

			IEnumerator e = anObject.GetEnumerator();
			//Hashtable e = anObject;
			bool first = true;
			while (e.MoveNext()) {
				DictionaryEntry d = e.Current as DictionaryEntry;
				string key = d.Key.ToString();
				object value = d.Value;

				if (!first) {
					builder.Append(", ");
				}

				SerializeString(key, builder);
				builder.Append(":");
				if (!SerializeValue(value, builder)) {
					return false;
				}

				first = false;
			}

			builder.Append("}");
			return true;
		}

		protected static bool SerializeArray(ArrayList anArray, StringBuilder builder)
		{
			builder.Append("[");

			bool first = true;
			for (int i = 0; i < anArray.Count; i++) {
				object value = anArray[i];

				if (!first) {
					builder.Append(", ");
				}

				if (!SerializeValue(value, builder)) {
					return false;
				}

				first = false;
			}

			builder.Append("]");
			return true;
		}

		protected static bool SerializeString(string aString, StringBuilder builder)
		{
			builder.Append("\"");

			char[] charArray = aString.ToCharArray();
			for (int i = 0; i < charArray.Length; i++) {
				char c = charArray[i];
				if (c == '"') {
					builder.Append("\\\"");
				} else if (c == '\\') {
					builder.Append("\\\\");
				} else if (c == '\b') {
					builder.Append("\\b");
				} else if (c == '\f') {
					builder.Append("\\f");
				} else if (c == '\n') {
					builder.Append("\\n");
				} else if (c == '\r') {
					builder.Append("\\r");
				} else if (c == '\t') {
					builder.Append("\\t");
				} else {
					int codepoint = Convert.ToInt32(c.ToString());
					if ((codepoint >= 32) && (codepoint <= 126)) {
						builder.Append(c);
					} else {
						string value = Int32Extensions.ToHexString(codepoint);
						builder.Append("\\u" + StringExtensions.PadLeft(value, 4, '0'));
					}
				}
			}

			builder.Append("\"");
			return true;
		}

		protected static bool SerializeNumber(double number, StringBuilder builder)
		{
			builder.Append(DoubleExtensions.ToHexString(number/*, CultureInfo.CurrentUICulture*/));
			return true;
		}

		/// <summary>
		/// Determines if a given object is numeric in any way
		/// (can be integer, double, null, etc). 
		/// 
		/// Thanks to mtighe for pointing out Double.TryParse to me.
		/// </summary>
		protected static bool IsNumeric(object o)
		{
			double result;

			return (o == null) ? false : DoubleExtensions.TryParse(o.ToString(), out result);
		}
	}

	/// <summary>
	/// A Json Object.
	/// Programmed by Huysentruit Wouter
	/// See the Json.ToJson method for more information.
	/// </summary>
	public class JsonObject : Hashtable
	{
		/// <summary>
		/// Convert the object to its JSON representation.
		/// </summary>
		/// <returns>A string containing the JSON representation of the object.</returns>
		public override string ToString()
		{
			string result = "";

			string[] keys = new string[Count];
			object[] values = new object[Count];

			Keys.CopyTo(keys, 0);
			Values.CopyTo(values, 0);

			for (int i = 0; i < Count; i++)
			{
				if (result.Length > 0)
				{
					result += ", ";
				}

				// If this string is already JSON'd, as denoted by start of object {
				// or start of array [, then use it as-is.  Otherwise, encode it to JSON
				string value = string.Empty;
				char v = values[i].ToString()[0];
				if ((v == '{') || (v == '['))
				{
					value = values[i].ToString();
				}
				else
				{
					value = JsonPrimitives.Serialize(values[i]);
				}
				if (value == null)
				{
					continue;
				}

				result += "\"" + keys[i] + "\"";
				result += ": ";
				result += value;
			}

			return "{" + result + "}";
		}

	}

	/// <summary>
	/// A Json Array.
	/// Programmed by Huysentruit Wouter
	/// See the Json.ToJson method for more information.
	/// </summary>
	public class JsonArray : ArrayList
	{
		/// <summary>
		/// Convert the array to its JSON representation.
		/// </summary>
		/// <returns>A string containing the JSON representation of the array.</returns>
		public override string ToString()
		{
			string[] parts = new string[Count];

			for (int i = 0; i < Count; i++)
			{
				// Encapsulate in quotes if not a JSON object or not already in quotes
				char c = this[i].ToString()[0];
				if (c == '{' || c == '[' || c == '"')
				{
					parts[i] = this[i].ToString();
				}
				else
				{
					parts[i] = "\"" + this[i].ToString() + "\"";
				}
			}

			string result = "";

			foreach (string part in parts)
			{
				if (result.Length > 0)
				{
					result += ", ";
				}

				result += part;
			}

			return "[" + result + "]";
		}
	}

}
