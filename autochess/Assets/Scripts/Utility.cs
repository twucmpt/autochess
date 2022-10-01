using System;
using System.Collections.Generic;

public static class Utility
{
	public static List<T> ChooseMultiple<T>(List<T> list, int number)
	{
		List<T> values = new();

		// only grab as many items as are there to grab
		if (number >= list.Count)
			return list;

		// generate a list of all the indices (0, 1, 2, ..., list.Count-1)
		List<int> indices = new();
		for (int i = 0; i < list.Count; i++)
			indices.Add(i);

		// pick a random sampling of indices and return out their corresponding values
		var selected = TakeMultiple(indices, number);
		foreach (var index in selected)
		{
			values.Add(list[index]);
		}

		return values;
	}
	public static List<T> TakeMultiple<T>(List<T> list, int number)
	{
		List<T> returnValues = new List<T>();
		for (int i = 0; i < number; i++)
		{
			if (list.Count == 0)
				break;

			returnValues.Add(Take(list));
		}

		return returnValues;
	}
	public static T Take<T>(List<T> list)
	{
		if (list.Count == 0)
			return default(T); 

		int index = UnityEngine.Random.Range(0,list.Count - 1);
		T value = list[index];
		list.RemoveAt(index);

		return value;
	}
}
