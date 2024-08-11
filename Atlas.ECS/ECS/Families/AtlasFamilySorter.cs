using System;
using System.Collections.Generic;

namespace Atlas.ECS.Families;

/// <summary>
/// Sorting algorithms found at http://www.codecodex.com/wiki.
/// </summary>
public static class AtlasFamilySorter
{
	#region Bubble

	public static void Bubble<T>(IList<T> list, Func<T, T, int> compare)
	{
		for(var i = list.Count - 1; i > 0; i--)
		{
			for(var j = 0; j < i; j++)
			{
				if(compare(list[j], list[j + 1]) > 0)
					Swap(list, j, j + 1);
			}
		}
	}

	#endregion

	#region Insertion

	public static void Insertion<T>(IList<T> list, Func<T, T, int> compare)
	{
		var count = list.Count;
		for(var i = 1; i < count; ++i)
		{
			var j = i - 1;

			var value = list[i];

			while(j >= 0 && compare(list[j], value) > 0)
			{
				list[j + 1] = list[j];
				--j;
			}
			list[j + 1] = value;
		}
	}

	#endregion

	#region Quick

	public static void Quick<T>(IList<T> list, Func<T, T, int> compare)
	{
		Quick(list, 0, list.Count - 1, compare);
	}

	private static void Quick<T>(IList<T> list, int left, int right, Func<T, T, int> compare)
	{
		var leftHold = left;
		var rightHold = right;
		var pivot = new Random().Next(left, right);
		Swap(list, pivot, left);
		pivot = left;
		left++;

		while(right >= left)
		{
			if(compare(list[left], list[pivot]) >= 0
				&& compare(list[right], list[pivot]) < 0)
				Swap(list, left, right);
			else if(compare(list[left], list[pivot]) >= 0)
				right--;
			else if(compare(list[right], list[pivot]) < 0)
				left++;
			else
			{
				right--;
				left++;
			}
		}
		Swap(list, pivot, right);
		pivot = right;
		if(pivot > leftHold)
			Quick(list, leftHold, pivot, compare);
		if(rightHold > pivot + 1)
			Quick(list, pivot + 1, rightHold, compare);
	}

	#endregion

	#region Selection

	public static void Selection<T>(IList<T> list, Func<T, T, int> compare)
	{
		for(var i = 0; i < list.Count - 1; i++)
		{
			var min = i;
			for(var j = i + 1; j < list.Count; j++)
			{
				if(compare(list[j], list[min]) < 0)
					min = j;
			}
			Swap(list, i, min);
		}
	}

	#endregion

	#region Heap

	public static void Heap<T>(IList<T> list, Func<T, T, int> compare)
	{
		var count = list.Count;

		for(var index = count / 2 - 1; index >= 0; --index)
			Heap(list, count, index, compare);

		for(var index = count - 1; index >= 0; --index)
		{
			Swap(list, 0, index);
			Heap(list, index, 0, compare);
		}
	}

	private static void Heap<T>(IList<T> list, int count, int index, Func<T, T, int> compare)
	{
		var largest = index;
		var left = 2 * index + 1;
		var right = 2 * index + 2;

		if(left < count && compare(list[left], list[largest]) > 0)
			largest = left;

		if(right < count && compare(list[right], list[largest]) > 0)
			largest = right;

		if(largest != index)
		{
			Swap(list, index, largest);
			Heap(list, count, largest, compare);
		}
	}

	#endregion

	#region Merge

	public static void Merge<T>(IList<T> list, Func<T, T, int> compare)
	{
		var copy = MergeCopy(list, compare);
		for(var i = 0; i < list.Count; ++i)
			list[i] = copy[i];
	}

	private static IList<T> MergeCopy<T>(IList<T> list, Func<T, T, int> compare)
	{
		if(list.Count <= 1)
			return list;

		var mid = list.Count / 2;

		var left = new List<T>();
		var right = new List<T>();

		for(var i = 0; i < mid; i++)
			left.Add(list[i]);

		for(var i = mid; i < list.Count; i++)
			right.Add(list[i]);

		return Merge(MergeCopy(left, compare), MergeCopy(right, compare), compare);
	}

	private static IList<T> Merge<T>(IList<T> left, IList<T> right, Func<T, T, int> compare)
	{
		var list = new List<T>();

		while(left.Count > 0 && right.Count > 0)
			if(compare(left[0], right[0]) > 0)
			{
				list.Add(right[0]);
				right.RemoveAt(0);
			}
			else
			{
				list.Add(left[0]);
				left.RemoveAt(0);
			}

		for(var i = 0; i < left.Count; i++)
			list.Add(left[i]);

		for(var i = 0; i < right.Count; i++)
			list.Add(right[i]);

		return list;
	}

	#endregion

	//Used by all sorting algorithms that swap by index.
	private static void Swap<T>(IList<T> list, int index1, int index2)
	{
		var temp = list[index2];
		list[index2] = list[index1];
		list[index1] = temp;
	}
}