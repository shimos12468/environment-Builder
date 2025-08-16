using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Searcher;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> placedGameObject = new();


    public int PlaceObject(GameObject prefab, Vector3 gridPosition)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.name = prefab.name+ gridPosition;
        newObject.transform.position =gridPosition;


        Vector3 pos = newObject.transform.position;
        newObject.TryGetComponent(out BoxCollider collider);
        if (collider != null)
        {
            pos.x += 0.5f * collider.size.x;
            pos.z += 0.5f * collider.size.z;

        }
        newObject.transform.position = pos;
        placedGameObject.Add(newObject);
        return placedGameObject.Count-1;
    }

    public void RemoveObjectAt(int gameobjectIndex)
    {
        if (placedGameObject.Count <= gameobjectIndex) return;
        Destroy(placedGameObject[gameobjectIndex]);
        placedGameObject[gameobjectIndex] = null;
    }

    public GameObject GetObjectAt(int index) {

        return placedGameObject[index];
    }

    public int GetObjectIndex(GameObject obj)
    {

        return placedGameObject.IndexOf(obj);
    }
}
