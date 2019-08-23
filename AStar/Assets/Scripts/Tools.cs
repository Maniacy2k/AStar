using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools 
{
    public enum ItemType
    {
        inital,
        wall,
        open,
        closed,
        path,
        start,
        end
    };

    public static string hudObjName = "HUD";


    /// <summary>
    /// Get G Coast for moving from current position, Array Item values:
    /// <para>[0] [1] [2]</para>
    /// <para>[3]  x  [4]</para>
    /// <para>[5] [6] [7]</para>
    /// </summary>
    public static float[] Move_ValueCost = new float[] 
    {
        14f, 10f, 14f,
        10,       10f,
        14f, 10f, 14f
    };

    public static float calculate_HValue(Vector3 start, Vector3 end)
    {
        
        Vector3 dist = end - start;

        return Mathf.Sqrt(Mathf.Pow(dist.x,2f) + Mathf.Pow(dist.y, 2f) + Mathf.Pow(dist.z, 2f));
    }

    public static int[] getBoardItem(ref List<List<Item>> board, int iType)
    {
        if (board == null || board.Count < 1)
            return null;


        for(int i=0;i<board.Count;i++)
            for (int p = 0; p < board[i].Count; p++)
            {
                if (board[i][p].ItemType.Equals(iType))
                    return new int[] { i, p };
            }

        return null;
    }

    public static int[] getRandPosBoardItem(ref List<List<Item>> board, int[] notAt)
    {
        if (board == null || board.Count < 1)
            return null;

        int[] randPos = new int[2];
        int beSafe = 0;
        while (beSafe < 100)
        {
            randPos[0] = Random.Range(0, board.Count-1);
            randPos[1] = Random.Range(0, board[0].Count - 1);
            if (notAt == null)
                return randPos;

            if (!randPos.Equals(notAt))
                return randPos;

            beSafe++;
        }
        

        return null;
    }

    public static int stringToInt(string txt) {
        int value = -1;

        int.TryParse(txt, out value);

        return value;
    }
    
}
