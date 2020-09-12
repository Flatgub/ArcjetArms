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

}
