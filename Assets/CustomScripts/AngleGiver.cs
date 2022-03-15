using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AngleGiver
{
    public float[] Angles();
    public Rotation4D getRotMat();
    public Vector4 getPosition();
}
