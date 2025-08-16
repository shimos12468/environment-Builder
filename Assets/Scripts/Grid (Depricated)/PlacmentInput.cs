using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacmentInput : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    private Vector3 lastPosition;
    [SerializeField]
    private LayerMask placmentLayerMask, SelectionLayerMask;

    public float clickDelay =0.5f;
    public bool placeing = false;
    public int clicked =0;
    public event Action OnClicked, OnExit;

    public State state;

    public void SetState(State state)
    {
        this.state = state; 
    }
   

    public void SetPlacing(bool place)
    {
        placeing = place;
        print(place);
    }
    public enum States
    {
        Placement 
    }

    private void Update()
    {
        switch (state)
        {
            case State.Placeing:
                HandlePlacmentInput();
            break;
            case State.Editing:
                HandlePlacmentInput();
                break;
            case State.Removing:
                HandlePlacmentInput();
                break;
            default: 
                HandleStartEditInput();
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
            

        } 
    }

    private void HandlePlacmentInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnClicked?.Invoke();
        }
    }

    
    private void HandleStartEditInput()
    {

        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = GetSelectedObject();
            if (obj != null)
            {
                PlacmentSystem.instance.StartEditing(obj);
            }
        }


        //if (Time.time - clickTime > clickDelay) clicked = 0;
        //if (Input.GetMouseButtonDown(0) && !placeing)
        //{
        //    clicked += 1;
        //    if (clicked == 1)
        //        clickTime = Time.time;

        //    if (IsSelectingObject())
        //    {
        //        if (clicked > 1 && (Time.time - clickTime) < clickDelay)
        //        {
        //            print("object selected");
        //            clicked = 0;
        //            GameObject obj = GetSelectedObject();
        //            if (obj != null)
        //            {
        //                PlacmentSystem.instance.StartEditing(obj);
        //            }

        //        }
        //    }

       // }
    }

    

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, 100, placmentLayerMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }

    public GameObject GetSelectedObject()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, SelectionLayerMask))
        {
           return hit.collider.gameObject;
        }

        return null;
    }

    public bool IsSelectingObject()
    {
        if (clicked > 1)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mainCamera.nearClipPlane;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, SelectionLayerMask))
            {
                print(hit.collider.name);
                return true;
            }

            return true;
        }
        return false;
    }



}


public enum State
{
    Placeing,
    Editing,
    Removing,
    Nothing
}
