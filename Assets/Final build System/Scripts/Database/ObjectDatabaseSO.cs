using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu( fileName ="object database", menuName ="ObjectsSO")]
public class ObjectDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectData;
}
[Serializable]
public class ObjectData
{

    [field : SerializeField]
    public string Name;
    [field: SerializeField]
    public int id;
    [field: SerializeField]
    public Vector2Int size;
    [field: SerializeField]
    public  GameObject prefab;

}