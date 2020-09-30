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
}
