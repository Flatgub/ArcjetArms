
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions 
{
    public static T GetRandom<T>(this List<T> list)
    {
        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
    }

    public static T GetRandom<T>(this T[] array)
    {
        int index = UnityEngine.Random.Range(0, array.Length);
        return array[index];
    }

    public static T PopRandom<T>(this List<T> list)
    {
        int index = UnityEngine.Random.Range(0, list.Count);
        T element = list[index];
        list.RemoveAt(index);
        return element;
    }

    //Fisher-Yates Shuffle
    public static List<T> Shuffle<T>(this List<T> list)
    {
        List<T> output = new List<T>(list);

        int n = output.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T swap = output[k];
            output[k] = output[n];
            output[n] = swap;
        }

        return output;
    }

    public static string GetShort(this Guid guid)
    {
        String str = guid.ToString();
        return str.Substring(0, str.IndexOf("-"));
    }

    public static string Colored(this string text, Color color)
    {
        return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(color), text);
    }

    public static string Colored(this int num, Color color)
    {
        return num.ToString().Colored(color);
    }

    public static Vector2 IntoRect(this Vector2 v, RectTransform rect)
    {
        float x = (v.x * rect.sizeDelta.x) - (rect.sizeDelta.x * 0.5f);
        float y = (v.y * rect.sizeDelta.y) - (rect.sizeDelta.y * 0.5f);
        return new Vector2(x, y);
    }
}
