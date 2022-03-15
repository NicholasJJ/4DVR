using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectList : ScriptableObject
{
    public List<RayMarchObject> objs;


    public void AddToList(RayMarchObject obj) {
        if (objs == null) objs = new List<RayMarchObject>();
        if (!objs.Contains(obj)) objs.Add(obj);
    }

    public void RemoveFromList(RayMarchObject obj) {
        if (objs.Contains(obj)) objs.Remove(obj);
    }
}
