using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    private GameObject hud;

    private Camera_Controller cam_Script;
    private aStar aStar_Script;

    private int mouseCounter = 0;
    private int mouseCountMax = 3;
    private int[] itemTypes = new int[] { 0,1,5,6 };

    // Start is called before the first frame update
    void Start()
    {
        if (hud == null)
            hud = GameObject.Find(Tools.hudObjName);

        if (hud != null && cam_Script == null)
            cam_Script = hud.GetComponent<Camera_Controller>();

        if (hud != null && aStar_Script == null)
            aStar_Script = hud.GetComponent<aStar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            Debug.Log("Left button down "+mouseCounter.ToString());
            mouseCounter++;
            if (mouseCounter > mouseCountMax)
                mouseCounter = 0;

            setObjectType();
        }

    }

    private void setObjectType()
    {
        if (aStar_Script == null)
            return;

        aStar_Script.setBoardType(getObjName(), itemTypes[mouseCounter]);
        

    }

    private string getObjName()
    {
        if (cam_Script == null)
            return "";

        GameObject hitObj = cam_Script.getRayHitObj(Input.mousePosition);
        if (hitObj != null)
            return hitObj.name;

        return "";
    }

    public void btn_StartAStar()
    {
        if (aStar_Script != null)
            aStar_Script.startAStar();
    }

}
