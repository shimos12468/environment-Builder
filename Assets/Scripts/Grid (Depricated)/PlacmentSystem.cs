using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacmentSystem : MonoBehaviour
{

    public static PlacmentSystem instance;

    [SerializeField]
    PlacmentInput input;
    
    [SerializeField]
    Grid grid;
    
    [SerializeField]
    public ObjectDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField] 
    public AudioClip correctPlacementClip, wrongPlacementClip;
    
    [SerializeField]
    public AudioSource source;

    private GridData floorData,furnitureData;

    [SerializeField]
    private PreviewSystem previewSystem;
   
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    
    [SerializeField]
    private ObjectPlacer objectPlacer;

    [SerializeField]
    IBuildingState buildingState;


    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        StopPlacment();
        floorData = new();
        furnitureData = new();
    }
    public void StartPlacement(int id)
    {

        StopPlacment();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(id, grid, previewSystem, database, floorData, furnitureData, objectPlacer);
        input.OnClicked += PlaceStructure;
        input.OnExit += StopPlacment;
        input.SetState(State.Placeing);
        Initialise();
    }

    public void StartEditing(GameObject obj)
    {
        StopPlacment();
        Vector3Int objectPos= grid.WorldToCell(obj.transform.position);
        PlacementData objectData= GridData.placedObjects[objectPos];
        input.SetState(State.Editing);
        gridVisualization.SetActive(true);
        buildingState = new EditingState(objectData.ID, grid, previewSystem, database, floorData, furnitureData, objectPlacer,objectData.occupiedPositions,objectData.placedObjectIndex);

        input.OnClicked += RepositionStructure;
        input.OnExit += StopPlacment;
    }


    public void StartRemoving()
    {
        StopPlacment();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid,previewSystem,floorData,furnitureData,objectPlacer);

        input.OnClicked += PlaceStructure;
        input.OnExit += StopPlacment;
    }

    private void StopPlacment()
    {
        if (buildingState == null) return;
        input.SetState(State.Nothing);
        gridVisualization.SetActive(false);
        buildingState.EndState();
        input.OnClicked-= PlaceStructure;
        input.OnClicked -= RepositionStructure;
        input.OnExit -= StopPlacment;
        lastDetectedPosition = Vector3Int.zero;
    }

    private void RepositionStructure()
    {
        if (input.IsPointerOverUI())
        {

            return;
        }

        input.SetPlacing(true);
        Vector3 mousePosition = input.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        buildingState.OnAction(gridPosition);
        StopPlacment();
    }


    private void PlaceStructure()
    {
        if (input.IsPointerOverUI())
        {

            return;
        }

        input.SetPlacing(true);
        Vector3 mousePosition = input.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        buildingState.OnAction(gridPosition);
    }



    private void Update()
    {
        if (IsPointerOverUI()) return;
        if (Input.GetMouseButton(0))
        {
            if (buildingState == null) { return; }
            print("clicked");
            Vector3 mousePosition = input.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);

            if (lastDetectedPosition != gridPosition)
            {
                buildingState.UpdateState(gridPosition);
                lastDetectedPosition = gridPosition;
            }
        }
    }
    

    public void RotateObject()
    {
        buildingState.RotatePrefab();
    }


    public void Initialise()
    {
        Vector3 mousePosition = input.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
}
