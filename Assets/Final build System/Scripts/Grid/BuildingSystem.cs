
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;




/// <summary>
///
/// </summary>
public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem instance;

    [Header("Object spawn Offset From Camera")]
    [SerializeField]
    [Range(20f, 100f)]
    private float spawnOffset;
    public Transform spawn;
    [Header("Serialized Feilds")]
    [SerializeField] public GridLayout gridLayout;

    [SerializeField] public PlacementSystem placementSystem;
    
    [SerializeField] ObjectDatabaseSO databaseSO;

    [Header("Furneture Layer")]
    [SerializeField] private LayerMask SelectionLayerMask;

    [Header("Movement Mode")]
    public bool FreeMovement;

    [Header("Input Bindings")]
    [SerializeField] InputAction press, screenPosition;

    [SerializeField] private CinemachineCamera camera;

    private Modes SelectedMode;
    private Grid grid;




    #region Unity Methods
    //makeing singleton and initializing the grid 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        //getting the existing grid
        grid = gridLayout.gameObject.GetComponent<Grid>();
        // initializing the game in the player mode
        SelectedMode = Modes.Player;
    }

    private void Update()
    {
        //only check for player input when we are in the building mode
        if (SelectedMode == Modes.Build)
        {
            //when player press the screen with only one touch
            if (Input.GetMouseButtonDown(0) || Input.touchCount == 1)
            {
                // getting the pressed object if there are any 
                // and enable the control of the object
                GetPressedObject();
            }
        }

    }

    #endregion

    #region Utils

    // returns true if the player pressing on ui or hovering on it
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();


    public void SetFreeMovement()
    {
        FreeMovement = !FreeMovement;
    }

    //getting the clicked on object Position on the map
    public static Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = GetPosition();
        mousePos.z = Camera.main.nearClipPlane;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
    

    // change the continues value of the object position on the map to fixed position which translates to a object snaping on the grad when dragging
    public  Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;

    }


    // just check if its mouse press or touch and return each thier respected input position
    private static Vector3 GetPosition()
    {
        Vector3 position;
        if (Input.touchCount > 0)
        {
            position = Input.GetTouch(0).position;
        }
        else
        {
            position = Input.mousePosition;
        }

        return position;
    }
    
    #endregion

    #region Building system

   


    //enable dragging to selected object if there are any
    private void GetPressedObject()
    {
        //check if touch on uiif true  dont continue
        if (IsPointerOverUI()) return;
        //getting touch position
        Vector3 position = GetPosition();

        //tell the placement object to get the placed object
        PlaceableObject obj = placementSystem.GetPlacedObject(position);
        //only continue if there is a furniture selected and only a furniture
        if (obj != null)
        {
            // if the object is placed unplace it and update the grid
            if (obj.Placed)
            {
                UnplaceObject(obj);
            }

            //update the selected object variable (we can use it for hover effect)
            placementSystem.SetSelectedObject(obj);
            
            // if there is a drag componant we  invoke the drag listners to start listning for player input 
            if (obj.TryGetComponent(out ObjectDrag drag))
            {
                if (drag == null)
                {
                    drag = obj.AddComponent<ObjectDrag>();
                }
                drag.Initialize();
            }
        }
    }

    // calling placementSystem to remove this object accuired positions from the grid
    private void UnplaceObject(PlaceableObject obj)
    {
        obj.Place(false);
        Vector3Int start = gridLayout.WorldToCell(obj.GetStartPosition());
        placementSystem.FreeGridPositions(start, obj.Size);
    }

    //getting to send the current selected mode
    public Modes GetSelectedMode()
    {
        return SelectedMode;
    }

    #endregion

    #region triggired by UI


    // function invoked by ui button and if there is a selected object we place it on the grid and update the grid
    public void PlaceObject()
    {
        if (placementSystem.HasSelectedObject())
        {
            placementSystem.PlaceToGrid(gridLayout);
        }
    }
    // function invoked by ui button and if there is a selected object we remove it from the grid and update the grid
    public void RemoveObject()
    {
        placementSystem.RemoveObject(gridLayout);
    }
    // function invoked by ui button and depends on the button we switch state from build to player and vise versa
    public void SetSelectedMode(bool state)
    {
        SelectedMode = state == true ? Modes.Player : Modes.Build;
    }

    // unique ui button for every peice with unique Id and we get the selected object from
    // our global database and spawn it in the place of the camer in the x,z axis and 0 on 
    // the y axis with offset forward with default value of 20
    public void Initialize(int id)
    {
        
        GameObject prefab = databaseSO.objectData[id].prefab;
        Vector3 CameraCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane + spawnOffset));
        CameraCenter.y = 0.25f;
        Vector3 position = SnapCoordinateToGrid(CameraCenter);

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            position = hit.point;
        }


        GameObject obj = Instantiate(prefab, Vector3.up, Quaternion.identity);
        if (obj.GetComponent<PlaceableObject>().IsVertical)
        {
            position.y = 1.25f;
        }
        else
        {
            position.y = 0.25f;
        }
        obj.transform.position = spawn.position;
        placementSystem.SetSelectedObject(obj.GetComponent<PlaceableObject>());
        ObjectDrag drag = obj.AddComponent<ObjectDrag>();
        drag.press = press;
        drag.screenPosition = screenPosition;
        drag.Initialize();
    }


    private void OnDrawGizmos()
    {
       Gizmos.DrawLine(camera.transform.position, camera.transform.forward * 100);
    }
    #endregion
}
