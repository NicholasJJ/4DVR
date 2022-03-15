using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct computeObject{
    public Vector4 position;
    public int type;
    public Vector4 color;
    public Vector4 dimensions;
    public int combType;

    public Vector4 r0;
    public Vector4 r1;
    public Vector4 r2;
    public Vector4 r3;
}

[RequireComponent(typeof(Transform4D))]
public class RayMarchObject : MonoBehaviour
{
    [SerializeField] ObjectList objectList;
    public int type;
    public Color color;
    public Transform4D transform4D;

    private void Start() {
        transform4D = gameObject.GetComponent<Transform4D>();
    }

    private void Awake() {
        objectList.AddToList(this);
    }

    private void OnDestroy() {
        objectList.RemoveFromList(this);
    }

    public computeObject GetCObj(){
        computeObject c = new computeObject();
        c.position = transform4D.position;
        c.type = type;
        c.color = color;
        c.dimensions = transform4D.dimensions;
        Matrix4x4 m = transform4D.rotation.GetMatrix();
        c.r0 = m.GetRow(0);
        c.r1 = m.GetRow(1);
        c.r2 = m.GetRow(2);
        c.r3 = m.GetRow(3);
        return c;
    }
}
