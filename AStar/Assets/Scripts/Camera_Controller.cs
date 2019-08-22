using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    private Vector3 position;
    private Camera cam_M;

    public float moveSpeedMax = 8f;
    public float zoomSpeed = 8f;

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

    public GameObject getRayHitObj(Vector3 pos)
    {
        Ray ray = cam_M.ScreenPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 400f))
            return hit.transform.gameObject;

        return null;
    }

    public void moveView(Vector3 mOrig, Vector3 dragPos)
    {
        Vector3 pos = cam_M.ScreenToViewportPoint(dragPos - mOrig);
        float moveSpeed = Vector3.Distance(mOrig, dragPos);
        if (moveSpeed > moveSpeedMax)
            moveSpeed = moveSpeedMax;
        Vector3 move = new Vector3(pos.x * moveSpeed, pos.y * moveSpeed, 0f);
        cam_M.transform.Translate(move, Space.World);
    }

    public void zoomView(Vector2 value)
    {
        Debug.Log(value.ToString());
        cam_M.orthographicSize -= (value.y * zoomSpeed);
    }

}
