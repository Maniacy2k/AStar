using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{    
    private int itemType;    
    private GameObject itemObj;
    private int[] parent;
    private bool inOpen = false;
    private bool inClosed = false;
    private int[] currIndex;

    public Item() {
        itemObj = new GameObject();
        itemType = (int)Tools.ItemType.inital;
    }

    public Item(int _itemType, int[] c_Index)
    {
        this.itemType = _itemType;
        this.currIndex = c_Index;
    }

    
    public int ItemType
    {
        get { return this.itemType; }
        set { this.itemType = value; }
    }

    public int[] Parent
    {
        get { return this.parent; }
        set {
            this.parent = value;
            this.itemType = (int)Tools.ItemType.open;
            this.InOpen = true;
        }
    }

    public bool InOpen
    {
        get { return this.inOpen; }
        set { this.inOpen = value; }
    }

    public bool InClosed
    {
        get { return this.inClosed; }
        set { this.inClosed = value; }
    }

    public int[] CurrIndex
    {
        get { return this.currIndex; }
    }

    public GameObject ItemObj
    {
        get { return this.itemObj; }
        set { this.itemObj = value; }
    }

    public Vector3 ItemPos
    {
        get {
            if (this.ItemObj != null)
                return this.ItemObj.transform.position;
            return Vector3.zero;
        }
        set {
            if (this.ItemObj != null)
                this.itemObj.transform.position = value;
        }
    }
}
