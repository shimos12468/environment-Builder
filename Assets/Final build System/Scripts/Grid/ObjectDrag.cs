using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ObjectDrag : MonoBehaviour
{


    public InputAction press, screenPosition;
    public bool Active;
    Vector3 currentScreenPos;
    public bool isDragging;

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    // check if the touch is on the current object
    private bool IsClickedOn
    {
        get
        {
            if (IsPointerOverUI()) return false;

            Ray ray = Camera.main.ScreenPointToRay(currentScreenPos);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                print(hit.transform.name);
                return transform == hit.transform;
            }
            return false;
        }
    }

    //getting the world position of the object and dragging
    private Vector3 WorldPos
    {
        get
        {
            float z = Camera.main.WorldToScreenPoint(transform.position).z;
            return Camera.main.ScreenToWorldPoint(currentScreenPos+new Vector3(0,0,z));
        }
    }

   
    //initialize input listners to listen to the player input
    public void Initialize()
    {
        press.Enable();
        screenPosition.Enable();
        screenPosition.performed += GetPosition;
        press.performed += StartDragging;
        press.canceled += StopDragging;
        Active = true;
    }
    // flag dragging  to false
    private void StopDragging(InputAction.CallbackContext context)
    {
         isDragging = false;
    }

    //chicking if the object is clicked on and if yes we start dragging coroutine which lasts untill we deselect the object
    //and starts only if we started to drag which makes this object a very cost effeciant and also we can compine this script with any other one.
    private void StartDragging(InputAction.CallbackContext context)
    {
        print("drag");
        if (IsClickedOn)
        {
            print("drag");
            StartCoroutine(Drag());
            BuildingSystem.instance.placementSystem.SetSelectedObject(gameObject.GetComponent<PlaceableObject>());
        }
    }


    //read the touch position from unity input system
    private void GetPosition(InputAction.CallbackContext context)
    {
        currentScreenPos = context.ReadValue<Vector2>();
    }
    //unsubscribe from getting any input from the player in this object
    internal void Unsubscribe()
    {
        press.Disable();
        screenPosition.Disable();
        screenPosition.performed -= GetPosition;
        press.performed -= StartDragging;
        press.canceled -= StopDragging;
        Active = false;
    }


    // dragging mechanism
    private IEnumerator Drag()
    {

        if (BuildingSystem.instance.GetSelectedMode()==Modes.Build)
        {
            isDragging = true;
            Vector3 offset = transform.position - WorldPos;
            while (isDragging)
            {
                transform.position = WorldPos + offset;
                Vector3 position = transform.position;
                if (GetComponent<PlaceableObject>().IsVertical)
                {
                    position.y = Mathf.Clamp(position.y, 0.75f, 5.25f);
                }
                else
                {
                    position.y = 0.25f;
                }

                if (BuildingSystem.instance.FreeMovement)
                {
                    transform.position = position;
                }
                else
                {

                    transform.position = BuildingSystem.instance.SnapCoordinateToGrid(position);
                }
                yield return null;
            }
        }

        
    }
}