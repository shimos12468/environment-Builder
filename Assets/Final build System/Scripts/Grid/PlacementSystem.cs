using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementSystem : MonoBehaviour
{
    public PlaceableObject objectToPlace;
    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private TileBase whiteTile;
    //[SerializeField] private LayerMask SelectionLayerMask;





    // get the placed furniture peice which we only interested in objects with furniture layer and ignore the rest
    // and if there is a object we return it other wise we return null
    public PlaceableObject GetPlacedObject(Vector3 position)
    {
        //Vector3 mousePos = Input.mousePosition;
        position.z = Camera.main.nearClipPlane;
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            hit.collider.TryGetComponent(out PlaceableObject obj);
            if (obj == null) return null;
            return obj;
        }

        return null;
    }

    // check if our selected object can be placed on the grid on the selected positions
    // we return weather if we can place it or not

    public bool CanBePlaced(GridLayout gridLayout)
    {
        Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        Vector3Int end = objectToPlace.Size;

        for (int i = start.x; i <= start.x + end.x; i++)
        {
            for (int j = start.y; j <= start.y + end.y; j++)
            {
                if (GridTable.placedObjectsPositions.ContainsKey(new Vector3Int(i, j, start.z)))
                {
                    return false;
                }
            }

        }
        return true;

    }
     
    //
    public void MoveObjectVertical(int dir)
    {
        if (objectToPlace != null)
        {
            if (objectToPlace.IsVertical)
            {
                float magnitude = BuildingSystem.instance.gridLayout.cellSize.y * dir;
                Vector3 result = objectToPlace.transform.position + transform.up * magnitude;
                objectToPlace.gameObject.transform.DOMove(result, 0.1f);
            }
           
        }
    }

    public void MoveObjectHorizontal(int dir)
    {
        if (objectToPlace != null)
        {
            float magnitude = BuildingSystem.instance.gridLayout.cellSize.x * dir;
            Vector3 result = objectToPlace.transform.position + transform.right * magnitude;
            objectToPlace.gameObject.transform.DOMove(result, 0.1f);
        }
    }


    //we  just rotate the object from the PlaceableObject script by calling Rotate function thius is just a warpper to make sure there is no null value
    public void RotateObject()
    {
        if (objectToPlace != null)
        {
            objectToPlace.Rotate();
        }
    }

    //setter that sets the objectToPlace variable
    public void SetSelectedObject(PlaceableObject placeableObject)
    {
        objectToPlace = placeableObject;
    }
    //getter t that sends the current value of objectToPlace
    public PlaceableObject GetSelectedObject()
    {
        return objectToPlace;
    }


    //returns true if objectToPlace is not null otherwise false
    public bool HasSelectedObject()
    {
        
            return objectToPlace != null;
        
    }
  

    // a warpper to make sure we are not trying to access null value
    // and if we arent we unsubscribe from input listner and destroy the selected object
    public void DeleteSelectedObject()
    {

        if (objectToPlace != null)
        {
            objectToPlace.DeleteObject();
            objectToPlace = null;
        }
       
    }
    // free current accuired position by selected object and disable input listening
    public void RemoveObject(GridLayout gridLayout)
    {
        if (objectToPlace != null)
        {
            Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
            FreeGridPositions(start, objectToPlace.Size);
            DeleteSelectedObject();
        }
    }

    // free grid position by itirating on the celles between the begining and the end and mark them as free
    public void FreeGridPositions(Vector3Int start, Vector3Int end)
    {
        for (int i = start.x; i <= start.x + end.x; i++)
        {
            for (int j = start.y; j <= start.y + end.y; j++)
            {
                mainTilemap.SetTile(new Vector3Int(i, j, start.z), null);
                if (GridTable.placedObjectsPositions.ContainsKey(new Vector3Int(i, j, start.z)))
                {
                    GridTable.placedObjectsPositions.Remove(new Vector3Int(i, j, start.z));
                }
            }
        }
    }

    //// accuire positions for selected object by itirating and mark those positions as accuired
    //public void TakeArea(Vector3Int start, Vector3Int end)
    //{
    //    for (int i = start.x; i <= start.x + end.x; i++)
    //    {
    //        for (int j = start.y; j <= start.y + end.y; j++)
    //        {
    //            for (int k = start.z; k <= start.z + end.z; k++)
    //            {
    //                mainTilemap.SetTile(new Vector3Int(i, j, k), whiteTile);
    //                GridTable.placedObjectsPositions[new Vector3Int(i, j, k)] = objectToPlace;
    //            }
    //        }

    //    }
    //}

    public void TakeArea2d(Vector3Int start, Vector3Int end)
    {
        for (int i = start.x; i <= start.x + end.x; i++)
        {
            for (int j = start.y; j <= start.y + end.y; j++)
            {
                mainTilemap.SetTile(new Vector3Int(i, j, start.z), whiteTile);
                GridTable.placedObjectsPositions[new Vector3Int(i, j, start.z)] = objectToPlace;

            }

        }
    }

    // place current object in the grid and accuire wanted positions
    public void PlaceToGrid(GridLayout gridLayout)
    {
        if (CanBePlaced(gridLayout))
        {
            objectToPlace.Place(true);
            Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
            TakeArea2d(start, objectToPlace.Size);   
            SetSelectedObject(null);
        }
        else
        {
            DeleteSelectedObject();
        }
    }
}
