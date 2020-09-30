using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions 
{
    public static T GetRandom<T>(this List<T> list)
    {
        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
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
