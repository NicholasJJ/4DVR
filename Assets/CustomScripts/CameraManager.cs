using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraManager : MonoBehaviour
{
    public ComputeShader shader;
    public AngleGiver angleGiver;
    public float FOV;
    private RenderTexture _target;
    private Camera _camera;

    public GameObject lightSource;

    public ObjectList objectList;

    private int totalSize;
    private void Awake() {
        int posSize = sizeof(float) * 4;
        int typeSize = sizeof(int);
        int ColorSize = sizeof(float) * 4;
        int DimSize = sizeof(float) * 4;

        totalSize = posSize + typeSize + ColorSize + DimSize + typeSize + (4*ColorSize);
    }

    private void Start() {
        angleGiver = GameObject.FindGameObjectWithTag("AG").GetComponent<AngleGiver>();
        _camera = gameObject.GetComponent<Camera>();
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        SetShaderParameters();
        Render(dest);
    }

    private void SetShaderParameters(){
        Rotation4D cRot = angleGiver.getRotMat();
        shader.SetVector("_lightDir", -lightSource.transform.forward);
        shader.SetMatrix("cRot",cRot.GetMatrix().transpose);
        shader.SetMatrix("_CameraInverseProjection", matFromList(new float[]{FOV,0,0,0,
                                                                             0,FOV,0,0,
                                                                             0,0,0,1,
                                                                             0,0,0,0}));
        shader.SetVector("cPos", angleGiver.getPosition()); // change this once we can move around
        
        
        
    }

    private Matrix4x4 matFromList(float[] input){
        Vector4 c0 = new Vector4(input[0],input[4],input[8],input[12]);
        Vector4 c1 = new Vector4(input[1],input[5],input[9],input[13]);
        Vector4 c2 = new Vector4(input[2],input[6],input[10],input[14]);
        Vector4 c3 = new Vector4(input[3],input[7],input[11],input[15]);
        return new Matrix4x4(c0,c1,c2,c3);
    }

    private void Render(RenderTexture dest) {
        InitRenderTexture();
        shader.SetTexture(0, "Result", _target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);

        shader.SetInt("width",Screen.width);
        shader.SetInt("height",Screen.height);
        

        List<RayMarchObject> objs = objectList.objs;
        List<computeObject> cObjs = new List<computeObject>();
        foreach (RayMarchObject obj in objs) {
            cObjs.Add(obj.GetCObj());
        }

        ComputeBuffer objectBuffer = new ComputeBuffer(cObjs.Count, totalSize);
        objectBuffer.SetData(cObjs.ToArray());
        shader.SetBuffer(0, "objs", objectBuffer);
        shader.SetInt("objSize", totalSize);
        shader.SetInt("numObjs", cObjs.Count);

        shader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        Graphics.Blit(_target, dest);

        objectBuffer.Dispose();
    }

    private void InitRenderTexture()
    {
        if (_target == null || _target.width != Screen.width || _target.height != Screen.height)
        {
            if (_target != null)
            {
                _target.Release();
            }

            _target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Linear);
            _target.enableRandomWrite = true;
            _target.Create();
        }
    }
}
