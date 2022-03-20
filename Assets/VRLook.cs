using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class VRLook : MonoBehaviour, AngleGiver
{
    private static XRController leftController;
    private static XRController rightController;

    public Transform head;
    private Vector3 prevPos;
    private Vector3 prevRot;
    public float[] lookAngles;
    private Rotation4D rotMat;
    public Vector4 position;

    [SerializeField] float RotSpeed;
    // Start is called before the first frame update
    void Start()
    {
        leftController = GameObject.FindGameObjectWithTag("lController").GetComponent<XRController>();
        rightController = GameObject.FindGameObjectWithTag("rController").GetComponent<XRController>();
        lookAngles = new float[] { 0, 0, 0, 0, 0, 0 };
        rotMat = new Rotation4D(lookAngles[0], lookAngles[1], lookAngles[2], lookAngles[3], lookAngles[4], lookAngles[5]);
    }

    // Update is called once per frame
    void Update()
    {
        //look
        Vector3 dr = head.rotation.eulerAngles - prevRot;
        dr *= Mathf.Deg2Rad;
        if (leftController.activateInteractionState.active || rightController.activateInteractionState.active)
        {
            //4D
            lookAngles[0] += dr.x;
            lookAngles[5] += -dr.y;
            lookAngles[2] += dr.z;
            lookAngles[3] += dr.z;

            //lookAngles[3] += RotSpeed * Time.deltaTime;
        } else
        {
            //Regular
            lookAngles[0] += dr.x;
            lookAngles[1] += dr.y;
            lookAngles[2] += dr.z;

            //lookAngles[3] -= RotSpeed * Time.deltaTime;
        }
        lookAngles[3] = Mathf.Clamp(lookAngles[3], 0, Mathf.PI / 2);
        rotMat = new Rotation4D(lookAngles[0], lookAngles[1], lookAngles[2], lookAngles[3], lookAngles[4], lookAngles[5]);


        //move
        Vector3 dp = head.position - prevPos + head.position; // problem
        dp = head.worldToLocalMatrix.MultiplyPoint3x4(dp);
        Vector4 forwards = rotMat.MultiplyPoint(new Vector4(0, 0, 1, 0));
        Vector4 right = rotMat.MultiplyPoint(new Vector4(1, 0, 0, 0));
        Vector4 up = rotMat.MultiplyPoint(new Vector4(0, 1, 0, 0));
        Vector4 change = (right * dp.x) + (up * dp.y) + (forwards * dp.z);
        position += (change);


        prevPos = head.position;
        prevRot = head.rotation.eulerAngles;
    }

    public float[] Angles()
    {
        return lookAngles;
    }

    public Rotation4D getRotMat()
    {
        return rotMat;
    }

    public Vector4 getPosition()
    {
        //return new Vector4(0, 1, 0, 0);
        return position;
    }
}
