using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCLook : MonoBehaviour, AngleGiver
{
    [SerializeField] float mouseSensitivity;
    [SerializeField] float speed;
    public GameObject head;
    public float[] lookAngles;
    private Rotation4D rotMat;
    public Vector4 position;
    Vector3 mouseLook;
    // Start is called before the first frame update
    void Start()
    {
        mouseLook = Vector2.zero;
        lookAngles = new float[]{0,0,0,0,0,0};
        rotMat = new Rotation4D(lookAngles[0],lookAngles[1],lookAngles[2],lookAngles[3],lookAngles[4],lookAngles[5]);
    }

    // Update is called once per frame
    void Update()
    {
        //look
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if(Input.GetKey(KeyCode.E))
                mouseLook += new Vector3(0, Input.GetAxis("Mouse Y"),Input.GetAxis("Mouse X"));
            else 
                mouseLook += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"),0);
            lookAngles = new float[]{mouseLook.y,mouseLook.x,0,mouseLook.z,0,0};
            for (int i = 0; i < lookAngles.Length; i++){
                lookAngles[i] *= Mathf.Deg2Rad * mouseSensitivity;
            }
            rotMat = new Rotation4D(lookAngles[0],lookAngles[1],lookAngles[2],lookAngles[3],lookAngles[4],lookAngles[5]);
            transform.localRotation = Quaternion.AngleAxis(mouseLook.x * mouseSensitivity, Vector3.up);
            head.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y * mouseSensitivity, Vector3.right);
        }

        //move
        Vector4 forwards = rotMat.MultiplyPoint(new Vector4(0,0,1,0));
        Vector4 right = rotMat.MultiplyPoint(new Vector4(1,0,0,0));
        Vector4 change = (forwards * Input.GetAxis("Vertical"))
                   +(right * Input.GetAxis("Horizontal"));
        change.y = 0;
        position += (change.normalized * speed * Time.deltaTime);
        
        //Mouse
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (Cursor.visible)
                LockMouse();
            else
                UnlockMouse();
        }
    }

    void LockMouse() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void UnlockMouse() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public float[] Angles() {
        return lookAngles;
    }

    public Rotation4D getRotMat() {
        return rotMat;
    }

    public Vector4 getPosition() {
        return position;
    }
}
