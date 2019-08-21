using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    public Vector3 get_ItemPos() { return this.itemObj.transform.position; }
    public void set_ItemPos(Vector3 _pos) { this.itemObj.transform.position = _pos; }
    private int itemType;
    public int get_ItemType() { return this.itemType; }
    public void set_ItemType(int _itemType) { this.itemType = _itemType; }
    private GameObject itemObj;
    public GameObject get_ItemObj() { return this.itemObj; }
    public void set_ItemObj(GameObject _itemObj) { this.itemObj = _itemObj; }
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

}
