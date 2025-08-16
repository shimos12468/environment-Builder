using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialsPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndecatorRendrer;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialsPrefab);
        cellIndicator.SetActive(false);
        cellIndecatorRendrer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab ,Vector2Int size)
    {
       
        previewObject = Instantiate(prefab);

        Vector3 pos = previewObject.transform.position;

        previewObject.TryGetComponent(out BoxCollider collider);
        if (collider != null)
        {
            pos.x += 0.5f * collider.size.x;
            pos.z += 0.5f * collider.size.z;

        }
        previewObject.transform.position = pos; 
        PreparePreview(previewObject);
        prepareCursor(size);
        cellIndicator.gameObject.SetActive(true);   
    }

    private void prepareCursor(Vector2Int size)
    {
        if (size.x > 0||size.y>0)
        cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
        cellIndecatorRendrer.material.mainTextureScale = size;

    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[]materials = renderer.materials;
            for(int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }

            renderer.materials = materials;
        }
    }


    public void StopShowinfPreview()
    {
       cellIndicator.SetActive(false);
       if(previewObject!=null)
       Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position,bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }
       
        MoveCursur(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity? Color.white : Color.red;
        c.a = 0.5f;
        cellIndecatorRendrer.material.color = c;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndecatorRendrer.material.color = c;
    }

    private void MoveCursur(Vector3 position)
    {

        position.x += 0.5f*cellIndicator.transform.localScale.x;
        position.z += 0.5f* cellIndicator.transform.localScale.z;
        cellIndicator.transform.position = position;

        ShowAllCells();

    }

    private void ShowAllCells()
    {
       for(int i = 0; i < cellIndicator.transform.localScale.x; i++)
       {
           for(int j = 0;j< cellIndicator.transform.localScale.z; j++)
            {
                printCell(i, j);
            }
       }
    }

    private void printCell(int i, int j)
    {
        //print($"({i},{j})");
    }

    private void MovePreview(Vector3 position)
    {

        previewObject.TryGetComponent(out BoxCollider collider);
        if (collider != null)
        {
            position.x += 0.5f * collider.size.x;
            position.z += 0.5f * collider.size.z;
        }
        previewObject.transform.position = new Vector3(
            position.x,
            position.y+previewYOffset,
            position.z);
    }

    public void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        prepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }


    public List<Vector2Int> rotations;
    public int rotationIndex = -1;

    internal void UpdateRotation(Vector2Int size)
    {

        //Vector3 rotation = cellIndicator.transform.rotation.eulerAngles;
        //rotation.y += 90;
        //cellIndicator.transform.DORotate(rotation, 0.1f);
        if (previewObject != null)
        {
            Vector3 objectRotation = previewObject.transform.rotation.eulerAngles;
            objectRotation.y += 90;
            previewObject.transform.DORotate(objectRotation, 0.1f);
        }
        Vector2Int direction = Vector2Int.one;
        rotationIndex += 1;
        if (rotationIndex >= rotations.Count)
        {
            rotationIndex = 0;
        }

        

        direction.x = rotations[rotationIndex].x;
        direction.y = rotations[rotationIndex].y;


        Vector2Int result = size;

        if (direction == new Vector2Int(1, 0))
        {
            result = new Vector2Int(size.x, size.y);
        }
        if (direction == new Vector2Int(0, -1))
        {
            result = new Vector2Int(-size.y, size.x);
        }
        if (direction == new Vector2Int(-1, 0))
        {
            result = new Vector2Int(-size.x, size.y);
        }
        if (direction == new Vector2Int(0, 1))
        {
            result = new Vector2Int(size.y, size.x);
        }
       

        if (size.x > 0 || size.y > 0)
            cellIndicator.transform.localScale = new Vector3(result.x, 1, result.y);
        cellIndecatorRendrer.material.mainTextureScale = new Vector2Int(result.x, result.y);
    }
}
