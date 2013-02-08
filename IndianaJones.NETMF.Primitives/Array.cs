using System;
using Microsoft.SPOT;
using System.Collections;

namespace IndianaJones.NETMF.Arrays
{
	/// <summary>
	/// Extension methods for ArrayList
	/// </summary>
	public static class ArrayListExtensions
	{
		/// <summary>
		/// Add Range to ArrayList
		/// </summary>
		/// <param name="list">Aray List to add to</param>
		/// <param name="arr">Items to be added to the collection</param>
		public static void AddRange(this ArrayList list, Array arr)
		{
			foreach (object b in arr)
			{
				list.Add(b);
			}
		}

		// Add a range of the source collection to the destination collection
		public static void AddRange(this ArrayList list, ArrayList arr, int sourceIndex, int length)
		{
			for (int i = sourceIndex; i < length; i++)
			{
				list.Add(arr[i]);
			}
		}


		/// <summary>
		/// Removes a range of elements from the ArrayList
		/// </summary>
		/// <param name="list">List to operate on</param>
		/// <param name="index">starting index</param>
		/// <param name="count"></param>
		public static void RemoveRange(this ArrayList list, int index, int count)
		{
			for (int i = 0; i < count; i++)
			{
				list.RemoveAt(index);
			}
		}

	}
}
