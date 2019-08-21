using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    private Vector3 position;
    private Camera cam_M;

    public void setCam_Pos(Vector3 pos)
    {
        if (cam_M == null)
            cam_M = Camera.main;

        cam_M.gameObject.transform.position = pos;
    }

    public void setCam_OrthoSize(float oSize)
    {
        if (cam_M == null)
            cam_M = Camera.main;

        cam_M.orthographicSize = oSize;
    }

}
