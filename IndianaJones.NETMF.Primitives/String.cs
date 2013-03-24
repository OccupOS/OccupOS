using System;
using Microsoft.SPOT;
using System.Collections;

namespace IndianaJones.NETMF.String
{
	public static class StringExtensions
	{

		public static bool IsNullOrEmpty(string str)
		{
			if (str == null || str == string.Empty)
				return true;

			return false;
		}

		public static bool Contains(this string source, string check)
		{
			// check string must be smaller
			try
			{
				if (check.Length > source.Length)
				{
					return false;
				}

				// Now do the easy check
				if (source == check)
				{
					return true;
				}

				for (int i = 0; i < source.Length; i++)
				{
					if (source.Substring(i, check.Length) == check)
					{
						return true;
					}
				}
			}
			catch
			{
				return false;
			}

			return false;
		}

		public static bool StartsWith(this string source, string check)
		{
			try
			{
				return (source.Substring(0, check.Length) == check);
			}
			catch
			{
			}

			return false;
		}

		public static bool EndsWith(this string source, string check)
		{
			try
			{
				return (source.Substring(source.Length - check.Length, check.Length) == check);
			}
			catch
			{
			}

			return false;
		}

		public static string PadLeft(this string source, int count, char pad)
		{
			for(int i=0; i<count; i++)
			{
				source = pad.ToString() + source;
			}

			return source;
		}

		public static string PadRight(this string source, int count, char pad)
		{
			for (int i = 0; i < count; i++)
			{
				source = source + pad.ToString();
			}

			return source;
		}
	}



	public class StringBuilder 
    { 
        ArrayList m_charArray = new ArrayList(); 
 
        public StringBuilder() 
        { 
        } 

		public StringBuilder(int capacity)
		{
			// we do nothing with capacity, this is to make the compiler happy
		}
 
        public StringBuilder(string value) : base() 
        { 
            Append(value); 
        } 
 
        public void Append(string value) 
        { 
            Char[] charArray = value.ToCharArray(); 
            Append(charArray, 0, charArray.Length); 
        } 
 
        public void Append(char[] value, int startIndex, int charCount) 
        { 
            for (int index = startIndex; index < startIndex + charCount; index++) 
                m_charArray.Add(value[index]); 
        } 

		public void Append(char value)
		{
			Append(value.ToString());
		}
 
        public int Length 
        { 
            get 
            { 
                return m_charArray.Count; 
            } 
        } 
 
        public override string ToString() 
        { 
            return new string((char[])m_charArray.ToArray(typeof(char))); 
        } 
    } 

}
