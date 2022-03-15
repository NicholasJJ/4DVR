using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation4D {
    public float YZ{get; set;}
    public float XZ{get; set;}
    public float XY{get; set;}
    public float XW{get; set;}
    public float YW{get; set;}
    public float ZW{get; set;}
    private Matrix4x4 matrix;
    private bool isMatrixGenerated;
    private float[] lastRotationsGenerated;
    public Rotation4D(float iYZ, float iXZ, float iXY, float iXW, float iYW, float iZW) {
        YZ = iYZ;
        XZ = iXZ;
        XY = iXY;
        XW = iXW;
        YW = iYW;
        ZW = iZW;
        isMatrixGenerated = false;
    }
    public Matrix4x4 GetMatrix() {
        if(isMatrixGenerated && YZ == lastRotationsGenerated[0]
                             && XZ == lastRotationsGenerated[1]
                             && XY == lastRotationsGenerated[2]
                             && XW == lastRotationsGenerated[3]
                             && YW == lastRotationsGenerated[4]
                             && ZW == lastRotationsGenerated[5]) {
            return matrix;
        } else {
            Matrix4x4 rYZ = matFromList(new float[]{1,0,0,0,
                                                    0, cos(YZ),sin(YZ),0,
                                                    0,-sin(YZ),cos(YZ),0,
                                                    0,0,0,1});
            Matrix4x4 rXZ = matFromList(new float[]{cos(XZ),0,-sin(XZ),0,
                                                    0,1,0,0,
                                                    sin(XZ),0,cos(XZ),0,
                                                    0,0,0,1});
            Matrix4x4 rXY = matFromList(new float[]{cos(XY),sin(XY),0,0,
                                                    -sin(XY),cos(XY),0,0,
                                                    0,0,1,0,
                                                    0,0,0,1});
            Matrix4x4 rXW = matFromList(new float[]{cos(XW),0,0,sin(XW),
                                                    0,1,0,0,
                                                    0,0,1,0,
                                                    -sin(XW),0,0,cos(XW)});
            Matrix4x4 rYW = matFromList(new float[]{1,0,0,0,
                                                    0,cos(YW),0,-sin(YW),
                                                    0,0,1,0,
                                                    0,sin(YW),0,cos(YW)});
            Matrix4x4 rZW = matFromList(new float[]{1,0,0,0,
                                                    0,1,0,0,
                                                    0,0,cos(ZW),-sin(ZW),
                                                    0,0,sin(ZW),cos(ZW)});
            // matrix = rXW * (rYW * (rXY * (rYZ * (rXZ * rZW))));
            matrix = rXY * (rYZ * (rXZ * (rXW * (rYW * rZW))));
            isMatrixGenerated = true;
            lastRotationsGenerated = new float[]{YZ,XZ,XY,XW,YW,ZW};
            return matrix;
        }
    }

    public Vector4 MultiplyPoint(Vector4 p) {
        Matrix4x4 m = this.GetMatrix().transpose;
        return new Vector4(dot(p,m.GetRow(0)),dot(p,m.GetRow(1)),dot(p,m.GetRow(2)),dot(p,m.GetRow(3)));
    }

    public float dot(Vector4 a, Vector4 b) {
        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z) + (a.w * b.w);
    }

    private Matrix4x4 matFromList(float[] input){
        Vector4 c0 = new Vector4(input[0],input[4],input[8],input[12]);
        Vector4 c1 = new Vector4(input[1],input[5],input[9],input[13]);
        Vector4 c2 = new Vector4(input[2],input[6],input[10],input[14]);
        Vector4 c3 = new Vector4(input[3],input[7],input[11],input[15]);
        return new Matrix4x4(c0,c1,c2,c3);
    }

    private float cos(float inp) => Mathf.Cos(inp);
    private float sin(float inp) => Mathf.Sin(inp);
}

public class Transform4D : MonoBehaviour
{
    public Vector4 position;

    public Vector4 dimensions;
    public Rotation4D rotation;

    [SerializeField] private float YZ;
    [SerializeField] private float XZ;
    [SerializeField] private float XY;
    [SerializeField] private float XW;
    [SerializeField] private float YW;
    [SerializeField] private float ZW;

    // Update is called once per frame
    void Update()
    {
        if (rotation == null) rotation = new Rotation4D(0,0,0,0,0,0);
        position.x = transform.position.x;
        position.y = transform.position.y;
        position.z = transform.position.z;
        rotation.YZ = YZ = transform.rotation.eulerAngles.x;
        rotation.XZ = XZ = transform.rotation.eulerAngles.y;
        rotation.XY = XY = transform.rotation.eulerAngles.z;
        rotation.XW = XW;
        rotation.YW = YW;
        rotation.ZW = ZW;
        rotation.YZ *= Mathf.Deg2Rad;
        rotation.XZ *= Mathf.Deg2Rad;
        rotation.XY *= Mathf.Deg2Rad;
        rotation.XW *= Mathf.Deg2Rad;
        rotation.YW *= Mathf.Deg2Rad;
        rotation.ZW *= Mathf.Deg2Rad;

        dimensions.x = transform.lossyScale.x;
        dimensions.y = transform.lossyScale.y;
        dimensions.z = transform.lossyScale.z;
    }
}
