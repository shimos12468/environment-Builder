using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData 
{
    public static Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition,Vector2Int objectSize,int id,int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);

        ShowAllCells(positionToOccupy);
        PlacementData data = new PlacementData(positionToOccupy,id,placedObjectIndex);

        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"Dictonary already contains this cell position{pos}");
            }
            placedObjects[pos] = data;
        }

    }

    private void ShowAllCells(List<Vector3Int> positions)
    {

        Debug.Log("Placed an Object");
        for (int i = 0; i < positions.Count; i++)
        {
            printCell(positions[i]);
        }
    }
    private void printCell(params Vector3Int [] pos)
    {
        for(int i = 0;i<pos.Length; i++)
        {
            Debug.Log($"({pos[i].x},{pos[i].z})");
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        printCell(gridPosition);
        for(int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y < objectSize.y; y++)
            {
                printCell(gridPosition + new Vector3Int(x, 0, y));
                returnVal.Add(gridPosition+new Vector3Int(x,0,y));    
            }
        }
        return returnVal;
    }


    public bool CanPlaceObjectAt(Vector3Int gridPosition,Vector2Int objectSize)
    {
        List<Vector3Int>positionToOccupy = CalculatePositions(gridPosition, objectSize); ;
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;

    }

    public PlacementData GetObjectAt(Vector3Int gridPosition)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        positions.Add(gridPosition);
        Vector3Int pos = gridPosition;
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                positions.Add (gridPosition + new Vector3Int(i, 0, j));
            }
        }


        for(int i = 0; i < positions.Count; i++)
        {
            if (placedObjects.ContainsKey(positions[i]))
            {
                return placedObjects[positions[i]];
            }
        }

        if (placedObjects.ContainsKey(pos) == false)
        {
            return null;
        }

        return placedObjects[pos]; 
    }


    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;
        }
        return placedObjects[gridPosition].placedObjectIndex;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach(var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
}


public class PlacementData
{
    public List<Vector3Int> occupiedPositions;

    public int ID;
    
    public int placedObjectIndex;
    
    public PlacementData(List<Vector3Int>occupiedPositions ,int id ,int placedObjectIndex)
    {

        this.occupiedPositions = occupiedPositions;
        this.ID = id;   
        this.placedObjectIndex = placedObjectIndex;

    } 

}
