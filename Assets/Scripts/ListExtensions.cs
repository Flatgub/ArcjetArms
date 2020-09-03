﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions 
{
    public static T GetRandom<T>(this List<T> list)
    {
        int index = Random.Range(0, list.Count-1);
        return list[index];
    }
}
