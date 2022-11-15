using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Extension
{
public static class DictionaryExt
{
	public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair,
		out TKey key, out TValue value)
	{
		key = pair.Key;
		value = pair.Value;
	}
}

public static class ListExt
{
	public static void PopulateList<T>(this List<T> list, int count)
		where T : class, new()
	{
		for (var i = 0; i < count; i++)
		{
			list.Add(new T());
		}
	}
}

public static class IntExt
{
	public static int ToLayerMaskInt(this int layerValue)
	{
		return 1 << layerValue;
	}
}

public static class VectorsExt
{
	public static bool AlmostTheSameAs(this Vector3 fst, Vector3 snd, float tolerance)
	{
		return Vector3.Distance(fst, snd) < tolerance;
	}

	private const float TOLERANCE = 0.01f;

	public static bool AlmostTheSameAsFast(this Vector2 fst, Vector2 snd)
	{
		var diff = fst - snd;
		return Mathf.Abs(diff.x) < TOLERANCE && Mathf.Abs(diff.y) < TOLERANCE;
	}

	public static Vector3 GetRounded(this Vector3 vector, int precision = 2)
	{
		return new Vector3(
			vector.x = (float) Math.Round(vector.x, precision),
			vector.y = (float) Math.Round(vector.y, precision),
			vector.z = (float) Math.Round(vector.z, precision)
		);
	}

	public static Vector2 ToFlatPosition(this Vector3 position)
	{
		return new Vector2(position.x, position.z);
	}
}

public static class StringExt
{
	public static bool IsNullOrEmpty(this string str)
	{
		return string.IsNullOrEmpty(str);
	}
}

public static class GameObjectExt
{
	public static void SetSafeActive(this GameObject gameObject, bool active)
	{
		if (gameObject.activeSelf != active)
		{
			gameObject.SetActive(active);
		}
	}
}

public static class FloatExt
{
	public static bool AlmostTheSame(this float first, float second, float tolerance)
	{
		return Math.Abs(first - second) < tolerance;
	}
}

public static class EnumerableExt
{
	public static string Print<T>(this IEnumerable<T> values, string separator = ", ")
	{
		return string.Join(separator, values);
	}
}

public static class UIEventsExt
{
	public static bool TryGetUnderMouse<T>(this PointerEventData eventData, out T obj) 
		where T : UnityEngine.Object
	{
		obj = null;
		foreach (var gObject in eventData.hovered)
		{
			if (gObject.TryGetComponent<T>(out obj))
			{
				return true;
			}
		}

		return false;
	}
}

public static class PhysicsHelper
{
	public const float RAYCAST_DISTANCE = 100000; 
	
	public static bool TryRaycastFromCameraToLayer(Camera camera, Vector3 mousePos, int targetLayer,
		out RaycastHit raycastHit)
	{
		return Physics.Raycast(
			camera.ScreenPointToRay(mousePos),
			out raycastHit, 
			RAYCAST_DISTANCE,
			targetLayer.ToLayerMaskInt()
		);
	}
}
}