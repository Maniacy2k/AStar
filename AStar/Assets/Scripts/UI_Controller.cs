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

    public UnityEngine.UI.Text lbl_WallValue;
    public List<UnityEngine.UI.InputField> sizeInputs;

    private Vector3 m_Orig;
    private Vector3 zoom2;

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
        if (Input.GetMouseButtonDown(1))
        {

            Debug.Log("Right button down " + mouseCounter.ToString());
            mouseCounter--;
            if (mouseCounter < 0)
                mouseCounter = mouseCountMax;

            setObjectType();
        }
        if (Input.GetMouseButton(0))
        {
            setObjectType();
        }

        if (Input.GetMouseButtonDown(2))
        {
            m_Orig = Input.mousePosition;
            return;
        }
        if (Input.GetMouseButton(2))
        {
            if (cam_Script != null)
                cam_Script.moveView(m_Orig, Input.mousePosition);
        }
        if (!Input.mouseScrollDelta.Equals(Vector2.zero))
        {
            if (cam_Script != null)
                cam_Script.zoomView(Input.mouseScrollDelta);
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

    public void btn_GenWalls()
    {
        if (aStar_Script != null)
            aStar_Script.generate_Walls();
    }

    public void sl_Wall_Change(float value)
    {
        if (lbl_WallValue != null)
            lbl_WallValue.text = string.Format("Wall: {0}", value.ToString());

        if (aStar_Script != null)
            aStar_Script.wallChange = value;

        
    }

    public void sl_Wall_ButtonUp()
    {
        if (aStar_Script != null)
            aStar_Script.generate_Walls();
    }

    public void btn_SetSize()
    {
        if (sizeInputs == null || sizeInputs.Count < 1)
            return;

        if (sizeInputs[0].text.Length < 1 && sizeInputs[1].text.Length < 1)
            return;

        if (aStar_Script == null)
            return;

        int w = -1;
        int h = -1;
        if (sizeInputs[0].text.Length > 0) {
            w = Tools.stringToInt(sizeInputs[0].text);
        }

        if (sizeInputs[1].text.Length > 0)
        {
            h = Tools.stringToInt(sizeInputs[1].text);
        }
        aStar_Script.setBoardSize(w, h);

        for (int i = 0; i < sizeInputs.Count; i++)
            sizeInputs[i].text = "";
    }

    public void tog_RandPos(bool value)
    {
        if (aStar_Script != null)
            aStar_Script.set_RandPos(value);
    }
}
