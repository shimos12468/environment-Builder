using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditingState : IBuildingState
{

    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectDatabaseSO database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    List<Vector3Int> occupiedPositions;
    int index;
    public EditingState(int iD,
        Grid grid, PreviewSystem previewSystem,
        ObjectDatabaseSO database, GridData floorData,
        GridData furnitureData, ObjectPlacer objectPlacer, List<Vector3Int> occupiedPositions ,int index)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.occupiedPositions = occupiedPositions;
        this.index = index;

        selectedObjectIndex = database.objectData.FindIndex(data => data.id == ID);

        GridData selectedData = null;
        if (furnitureData.CanPlaceObjectAt(occupiedPositions[0], Vector2Int.one) == false)
        {
            selectedData = furnitureData;
        }
        else if (floorData.CanPlaceObjectAt(occupiedPositions[0], Vector2Int.one) == false)
        {
            selectedData = floorData;
        }
        if (selectedData == null)
        {
            //sound
        }

        else
        {
            selectedData.RemoveObjectAt(occupiedPositions[0]);
            objectPlacer.RemoveObjectAt(index);
        }


        if (selectedObjectIndex > -1)
        {

            previewSystem.StartShowingPlacementPreview(database.objectData[selectedObjectIndex].prefab, database.objectData[selectedObjectIndex].size);

        }
        else
        {
            throw new System.Exception($"no object with id{ID}");
        }
    }


    public void EndState()
    {
        previewSystem.StopShowinfPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            return;
        }

        int index = objectPlacer.PlaceObject(database.objectData[selectedObjectIndex].prefab, grid.CellToWorld(gridPosition));
        GridData selectedData = database.objectData[selectedObjectIndex].id == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(gridPosition,
            database.objectData[selectedObjectIndex].size,
            database.objectData[selectedObjectIndex].id,
           index);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectData[selectedObjectIndex].id == 0 ? floorData : furnitureData;

        return selectedData.CanPlaceObjectAt(gridPosition, database.objectData[selectedObjectIndex].size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }

    public void RotatePrefab()
    {
        throw new System.NotImplementedException();
    }
}
