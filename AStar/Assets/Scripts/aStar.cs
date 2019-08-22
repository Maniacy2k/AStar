using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aStar : MonoBehaviour
{

    /// <summary>
    /// path Material:
    /// <para>[0] = default</para>
    /// <para>[1] = wall</para> 
    /// <para>[2] = open</para> 
    /// <para>[3] = closed</para> 
    /// <para>[4] = path</para> 
    /// <para>[5] = start</para> 
    /// <para>[6] = end</para> 
    /// </summary>
    public List<Material> path_M;
    public GameObject cube_Obj;
    public GameObject boardHolder;

    private List<List<Item>> board;
    private bool boardUpdate_Flag = false;
    private bool aStar_Flag = false;
    private bool path_Flag = false;
    private bool noSol_Flag = false;

    public int width;
    public int height;
    public float cubeSpace;
    public float wallChange = 0.2f;

    private GameObject hud_Obj;
    private Camera_Controller camScript;

    private List<int[]> openList;
    private List<int[]> closedList;
    private int[] currBIndex;
    private int[] startIndex;
    private int[] endIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (hud_Obj == null)
            hud_Obj = this.gameObject;

        if (hud_Obj != null && camScript == null)
            camScript = hud_Obj.GetComponent<Camera_Controller>();

        
        createBoard();

        
    }

    private void Update()
    {
        if (boardUpdate_Flag)
            update_BoardItem_Materials();

        if (aStar_Flag)
            calculate_A_Star();

        if (path_Flag)
            draw_Path();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F)) {
            generate_Walls();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            createBoard();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            set_BoardStartEnd();
            Debug.Log("HValue: " + Tools.calculate_HValue(board[board.Count - 1][1].ItemPos, board[0][board[0].Count - 1].ItemPos).ToString());
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            startAStar();
            //List<int[]> tmpI = get_neightbourIndexs(new int[] { board.Count - 1, 0 });
            //set_Parents(tmpI, new int[] { board.Count - 1, 0 });
            //calculate_A_Star();
        }
    }

    public void startAStar()
    {
        
        if (set_BoardStartEnd())
        {
            Debug.Log("Start AStar");
            aStar_Flag = true;
        }
    }

    private void draw_Path()
    {
        path_Flag = false;
        if (closedList == null || closedList.Count < 1)
            return;

        int count = 0;
        List<int[]> pathIndex = new List<int[]>();
        if (noSol_Flag)
        {
            noSol_Flag = false;
            int[] lowIndex = getClosedLowestF();
            board[lowIndex[0]][lowIndex[1]].ItemType = (int)Tools.ItemType.path;
            pathIndex.Add(lowIndex);
        }
        else
        {
            //board[closedList[closedList.Count - 1][0]][closedList[closedList.Count - 1][1]].ItemType = (int)Tools.ItemType.path;
            pathIndex.Add(closedList[closedList.Count - 1]);
        }
        
        
        while (count < closedList.Count) {
            int[] parent = pathIndex[pathIndex.Count-1];
            if (board[parent[0]][parent[1]].Parent != null)
                pathIndex.Add(board[parent[0]][parent[1]].Parent);
            else
                break;

            count++;
        }

        if (pathIndex.Count > 0)
        {
            for (int i = 0; i < pathIndex.Count; i++)
            {
                int[] tmpI = pathIndex[i];
                if (board[tmpI[0]][tmpI[1]].ItemType.Equals((int)Tools.ItemType.closed))
                    board[tmpI[0]][tmpI[1]].ItemType = (int)Tools.ItemType.path;
            }
            boardUpdate_Flag = true;
        }
        
    }

    private void update_BoardItem_Materials()
    {
        boardUpdate_Flag = false;

        if (board == null || board.Count < 1)
            return;

        for (int i = 0; i < board.Count; i++)
            for (int p = 0; p < board[i].Count; p++)
                board[i][p].ItemObj.GetComponent<Renderer>().material = path_M[board[i][p].ItemType];
    }

    private void createBoard()
    {
        destoryBoard();

        board = new List<List<Item>>();
        float xPos = 0f;
        float yPos = 0f;
        for (int i = 0; i < width; i++)
        {
            board.Add(new List<Item>());
            for (int p = 0; p < height; p++)
            {
                Item tmpItem = new Item((int)Tools.ItemType.inital, new int[] { i, p });
                if (Random.Range(0f, 1f) <= wallChange)
                    tmpItem.ItemType = (int)Tools.ItemType.wall;
                GameObject tmpObj = Instantiate(cube_Obj);
                tmpObj.name = string.Format("cube_{0}_{1}", (i).ToString("0#"), (p).ToString("0#"));
                xPos = (xPos + cubeSpace)+width;
                tmpObj.transform.localScale = new Vector3(width, height, width);
                tmpObj.transform.position = new Vector3(xPos, yPos, 0f);
                tmpObj.transform.parent = boardHolder.transform;
                if (tmpItem.ItemType.Equals((int)Tools.ItemType.wall))
                    tmpObj.GetComponent<Renderer>().material = path_M[1];
                tmpItem.ItemObj = tmpObj;
                board[i].Add(tmpItem);
            }
            yPos = (yPos + cubeSpace) + height;
            xPos = 0f;
        }

        if (camScript != null && board.Count > 0)
        {
            //Vector3 tPos = board[width / 2][height / 2].get_ItemPos();
            camScript.setCam_Pos(new Vector3((((width + cubeSpace) * board.Count) / 2f) + cubeSpace, (((height + cubeSpace) * board[0].Count) / 2f) - cubeSpace, -200f));
            camScript.setCam_OrthoSize((((height + cubeSpace) * board[0].Count) / 2f) + (cubeSpace / 2f));
        }
    }

    private void destoryBoard()
    {
        if (board == null || board.Count < 1)
            return;

        for (int i = 0; i < board.Count; i++)
            for (int p = 0; p < board[i].Count; p++)
                GameObject.Destroy(board[i][p].ItemObj);
    }

    public void generate_Walls()
    {
        if (board == null || board.Count < 1)
            return;

        for (int i = 0; i < width; i++)
        {            
            for (int p = 0; p < height; p++)
            {
                board[i][p].ItemType = (int)Tools.ItemType.inital;
                board[i][p].ItemObj.GetComponent<Renderer>().material = path_M[0];
                if (Random.Range(0f, 1f) <= wallChange) {
                    board[i][p].ItemType = (int)Tools.ItemType.wall;
                    board[i][p].ItemObj.GetComponent<Renderer>().material = path_M[1];
                }
            }
        }

    }

    private bool set_BoardStartEnd()
    {
        if (board == null || board.Count < 1)
            return false;

        //startIndex = new int[] { board.Count - 1 , 0 };
        //endIndex = new int[] { 1, board[0].Count - 2 };
        startIndex = Tools.getBoardItem(ref board, (int)Tools.ItemType.start);
        endIndex = Tools.getBoardItem(ref board, (int)Tools.ItemType.end);

        if (startIndex == null || endIndex == null)
        {
            Debug.Log("No start or end!");
            return false;
        }

        board[startIndex[0]][startIndex[1]].ItemType = (int)Tools.ItemType.start;
        board[endIndex[0]][endIndex[1]].ItemType = (int)Tools.ItemType.end;
        boardUpdate_Flag = true;
        openList = new List<int[]>();
        closedList = new List<int[]>();
        set_BoardHValue(endIndex);
        currBIndex = startIndex;
        openList.Add(currBIndex);
        board[startIndex[0]][startIndex[1]].F_Value = 0f;
        return true;
    }

    private void set_Parents(List<int[]> surr_BItems, int[] parentIndex)
    {
        if (surr_BItems == null || surr_BItems.Count < 1)
            return;

        for (int i = 0; i < surr_BItems.Count; i++)
        {
            if (surr_BItems[i] != null)
                if (!board[surr_BItems[i][0]][surr_BItems[i][1]].ItemType.Equals((int)Tools.ItemType.wall) 
                    && !board[surr_BItems[i][0]][surr_BItems[i][1]].InOpen && !board[surr_BItems[i][0]][surr_BItems[i][1]].InClosed)
                {
                    board[surr_BItems[i][0]][surr_BItems[i][1]].Parent = parentIndex;
                    openList.Add(surr_BItems[i]);
                    boardUpdate_Flag = true;
                }
        }

    }

    private void calculate_A_Star()
    {
        // to do        
        openList.Remove(currBIndex);
        closedList.Add(currBIndex);
        board[currBIndex[0]][currBIndex[1]].InClosed = true;

        List<int[]> neighbours = get_neightbourIndexs(currBIndex);
        set_Parents(neighbours, currBIndex);
        if (atEnd(neighbours)) {
            // end aStar            
            return;
        }

        //float currF = board[currBIndex[0]][currBIndex[1]].F_Value;
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i] == null)
                continue;
           
            if (board[neighbours[i][0]][neighbours[i][1]].InClosed || board[neighbours[i][0]][neighbours[i][1]].ItemType.Equals((int)Tools.ItemType.wall))
                continue;

            //board[neighbours[i][0]][neighbours[i][1]].H_Value = currG + Tools.G_ValueCost[i];

            if (!board[neighbours[i][0]][neighbours[i][1]].InOpen)
            {
                openList.Add(neighbours[i]);
                board[neighbours[i][0]][neighbours[i][1]].InOpen = true;
                board[neighbours[i][0]][neighbours[i][1]].G_Value = board[currBIndex[0]][currBIndex[1]].G_Value + Tools.Move_ValueCost[i];
                board[neighbours[i][0]][neighbours[i][1]].F_Value = board[neighbours[i][0]][neighbours[i][1]].G_Value + board[neighbours[i][0]][neighbours[i][1]].H_Value;
            }

            if (board[neighbours[i][0]][neighbours[i][1]].F_Value < board[neighbours[i][0]][neighbours[i][1]].G_Value)
            {
                board[neighbours[i][0]][neighbours[i][1]].Parent = currBIndex;
                board[neighbours[i][0]][neighbours[i][1]].G_Value = board[neighbours[i][0]][neighbours[i][1]].H_Value;
                board[neighbours[i][0]][neighbours[i][1]].F_Value = board[neighbours[i][0]][neighbours[i][1]].G_Value + board[neighbours[i][0]][neighbours[i][1]].H_Value;
            }


        }
        if (openList.Count > 0)
            currBIndex = getLowestFValue();
        else {
            aStar_Flag = false;
            if (closedList.Count > 0)
            {
                noSol_Flag = true;
                path_Flag = true;
            }
            Debug.Log("No Solution!");
        }

    }

    private int[] getLowestFValue()
    {
        int best = 0;
        float bestF = 0f;
        for (int i = 0; i < openList.Count; i++)
        {
            if (i == 0)
                bestF = board[openList[i][0]][openList[i][1]].F_Value;
            else
            {
                if (board[openList[i][0]][openList[i][1]].F_Value < bestF)
                {
                    bestF = board[openList[i][0]][openList[i][1]].F_Value;
                    best = i;
                }
            }
        }

        return openList[best];
    }

    private int[] getClosedLowestF()
    {
        int best = 1;
        float bestF = 0f;
        for (int i = 1; i < closedList.Count; i++)
        {
            if (i == 1)
                bestF = board[closedList[i][0]][closedList[i][1]].F_Value;
            else
            {
                if (board[closedList[i][0]][closedList[i][1]].F_Value > bestF)
                {
                    bestF = board[closedList[i][0]][closedList[i][1]].F_Value;
                    best = i;
                }
            }
        }

        return closedList[best];
    }

    private bool atEnd(List<int[]> neighbours)
    {

        for (int i = 0; i < neighbours.Count; i++) {
            if (neighbours[i] != null)
                if (board[neighbours[i][0]][neighbours[i][1]].ItemType.Equals((int)Tools.ItemType.end))
                {
                    currBIndex = neighbours[i];
                    openList.Remove(neighbours[i]);
                    closedList.Add(neighbours[i]);
                    board[neighbours[i][0]][neighbours[i][1]].InClosed = true;
                    aStar_Flag = false;
                    path_Flag = true;
                    return true;
                }
        }

        return false;
    }

    private void set_BoardHValue(int[] endPosIndex)
    {
        if (board == null || board.Count < 1)
            return;

        for (int i = 0; i < board.Count; i++)
            for (int p = 0; p < board[i].Count; p++)
            {
                if (board[i][p].ItemType.Equals((int)Tools.ItemType.wall) ||
                    board[i][p].ItemType.Equals((int)Tools.ItemType.start) ||
                    board[i][p].ItemType.Equals((int)Tools.ItemType.end)
                    )
                    continue;
                board[i][p].ItemType = (int)Tools.ItemType.inital;
                board[i][p].set_InitialValues();
                board[i][p].H_Value = Tools.calculate_HValue(board[i][p].ItemPos, board[endPosIndex[0]][endPosIndex[1]].ItemPos);
                //GameObject tmp = new GameObject("HValue_" + board[i][p].H_Value.ToString());
                //tmp.transform.position = board[i][p].ItemPos;
                //tmp.transform.parent = board[i][p].ItemObj.transform;

            }
    }

    #region BoardCurrItem

    /// <summary>
    /// Get board items around current position, Array Item values:
    /// <para>[0] [1] [2]</para>
    /// <para>[3]  x  [4]</para>
    /// <para>[5] [6] [7]</para>
    /// </summary>
    /// <param name="currPosIndex"></param>
    /// <returns></returns>
    private List<int[]> get_neightbourIndexs(int[] currPosIndex)
    {
        List<int[]> tmpI = new List<int[]>();

        // Top Left
        int[] tl = new int[] { currPosIndex[0]+1, currPosIndex[1]-1};
        if (inBoardBounds(tl))
            tmpI.Add(tl);
        else
            tmpI.Add(null);

        // Top Middle
        int[] tm = new int[] { currPosIndex[0]+1, currPosIndex[1]  };
        if (inBoardBounds(tm))
            tmpI.Add(tm);
        else
            tmpI.Add(null);

        // Top Right
        int[] tr = new int[] { currPosIndex[0]+1, currPosIndex[1] + 1 };
        if (inBoardBounds(tr))
            tmpI.Add(tr);
        else
            tmpI.Add(null);

        // Middle Left
        int[] ml = new int[] { currPosIndex[0], currPosIndex[1]-1 };
        if (inBoardBounds(ml))
            tmpI.Add(ml);
        else
            tmpI.Add(null);

        // Centre (curr)

        // Middle Right
        int[] mr = new int[] { currPosIndex[0], currPosIndex[1] +1};
        if (inBoardBounds(mr))
            tmpI.Add(mr);
        else
            tmpI.Add(null);

        // Bottom Left
        int[] bl = new int[] { currPosIndex[0] - 1, currPosIndex[1]-1 };
        if (inBoardBounds(bl))
            tmpI.Add(bl);
        else
            tmpI.Add(null);

        // Bottom Middle
        int[] bm = new int[] { currPosIndex[0]-1 , currPosIndex[1] };
        if (inBoardBounds(bm))
            tmpI.Add(bm);
        else
            tmpI.Add(null);

        // Bottom Right
        int[] br = new int[] { currPosIndex[0]-1, currPosIndex[1] + 1 };
        if (inBoardBounds(br))
            tmpI.Add(br);
        else
            tmpI.Add(null);

        return tmpI;
    }

    private bool inBoardBounds(int[] value)
    {
        if (value[0] < 0 || value[0] >= board.Count)
            return false;

        if (value[1] < 0 || value[1] >= board[0].Count)
            return false;

        return true;
    }

    #endregion


    public void setBoardType(string name, int iType)
    {
        if (name.Length < 1)
            return;

        if (board == null || board.Count < 1)
            return;

        for (int i = 0; i < board.Count; i++)
            for (int p = 0; p < board[i].Count; p++) {
                if (board[i][p].ItemObj.name.Equals(name)) {
                    board[i][p].ItemType = iType;
                    boardUpdate_Flag = true;
                    return;
                }
            }

    }

}
