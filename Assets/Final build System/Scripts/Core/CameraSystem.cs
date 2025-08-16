using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraSystem : MonoBehaviour
{


    [SerializeField] bool edgeScroll ,dragPan;
    [SerializeField] float fieldOfViewMin = 10, fieldOfViewMax = 50, followOffsetMin = 5f, followOffsetMax =50f;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    
    float targetFieldOfView =50;
    bool dragPanMoveActive;
    Vector2 lastMousePosition;
    private void Awake()
    {
     //   followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }
    // Update is called once per frame
    void Update()
    {
        HandleCameraMovement();
        if (dragPan)
        {
            HandleCameraMovementDragPan();
        }

        if (edgeScroll)
        {
            HandleCameraMovementEdgeScrolling();
        }
        HandleCameraRotation();
        HandleCameraZoom();
        HandleCameraVerticalMovement();
    }

    private void HandleCameraVerticalMovement()
    {
        float rotateDirY = 0f;
        if (Input.GetKey(KeyCode.Z)) rotateDirY = 1f;
        if (Input.GetKey(KeyCode.X)) rotateDirY = -1f;

        float offsetSpeed = 10;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += rotateDirY * Time.deltaTime * offsetSpeed;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = Mathf.Clamp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y, 5, 40);
    }

    private void HandleCameraMovement()
    {
        Vector3 inputDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) inputDir.z = 1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = 1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 50f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

    }

    private void HandleCameraMovementDragPan()
    {
        Vector3 inputDir = Vector3.zero;
        if (Input.GetMouseButtonDown(0))
        {
            dragPanMoveActive = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragPanMoveActive = false;


        }
        if (dragPanMoveActive)
        {
            Vector2 mouseMovementDelta = lastMousePosition - (Vector2)Input.mousePosition;

            float dragPanSpeed = 0.2f;
            inputDir.x = mouseMovementDelta.x * dragPanSpeed;
            inputDir.z = mouseMovementDelta.y * dragPanSpeed;
            lastMousePosition = Input.mousePosition;
        }

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 50f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
    private void HandleCameraMovementEdgeScrolling()
    {
        Vector3 inputDir = Vector3.zero;
       
            int edgeScrollSize = 20;
            if (Input.mousePosition.x < edgeScrollSize)
            {
                inputDir.x = -1f;
            }
            if (Input.mousePosition.y < edgeScrollSize)
            {
                inputDir.z = -1f;
            }

            if (Input.mousePosition.x > Screen.width - edgeScrollSize)
            {
                inputDir.x = 1f;
            }
            if (Input.mousePosition.y > Screen.height - edgeScrollSize)
            {
                inputDir.z = 1f;
            }
        
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 50f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

    }
    private void HandleCameraRotation() {



        float rotateDir = 0f;
       
        if (Input.GetKey(KeyCode.Q)) rotateDir = 1f;
        if (Input.GetKey(KeyCode.E)) rotateDir = -1f;

       


        float rotateSpeed = 100f;
        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime,0f);

    }
    private void HandleCameraZoom()
    {
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFieldOfView += 5;
        }
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFieldOfView -= 5;
        }

        targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);
        float zoomSpeed = 10f;
        cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
    }

    Vector3 followOffset = Vector3.zero;
    private void HandleCameraZoomMoveForward()
    {
        Vector3 zoomDir = followOffset.normalized;

        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset -= zoomDir;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset += zoomDir;
        }


        if (followOffset.magnitude < followOffsetMin)
        {
            followOffset = zoomDir * followOffsetMin;
        }
        if (followOffset.magnitude > followOffsetMax)
        {
            followOffset = zoomDir * followOffsetMax;
        }
        float zoomSpeed = 10f;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime* zoomSpeed);
    }
}
