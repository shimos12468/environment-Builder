using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{

    private int gameobjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    public RemovingState(Grid grid, PreviewSystem previewSystem, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowinfPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if (furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = furnitureData;
        }
        else if (floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = floorData;
        }
        if (selectedData==null)
        {
            //sound
        }
        else
        {
            gameobjectIndex = selectedData.GetRepresentationIndex(gridPosition);

            if (gameobjectIndex == -1) return;

            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameobjectIndex);

        }

        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition,CheckIsSelectionIsValid(gridPosition));
    }

    private bool CheckIsSelectionIsValid(Vector3Int gridPosition)
    {
        return !(furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one)&& floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIsSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }

    public void RotatePrefab()
    {
        throw new NotImplementedException();
    }
}
