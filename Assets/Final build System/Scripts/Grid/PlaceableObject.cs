using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
   public bool Placed { get; private set; }
   public Vector3Int Size { get; private set; }
   public Vector3[] Vertices;
   public bool IsVertical;


    //calculate the vertecies based on the collider also we can make it based on the mesh but we will
    //need a whole mesh (these meshed are cutted to smaller meshes) and with this we calculatew the 4 vertecies for any object
    public void GetColliderVertexPositionsLocal()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        Vertices = new Vector3[4];
        Vertices[0] = collider.center  +new Vector3(-collider.size.x , -collider.size.y, -collider.size.z)*0.5f;
        Vertices[1] = collider.center + new Vector3(collider.size.x, -collider.size.y, -collider.size.z) * 0.5f;
        Vertices[2] = collider.center + new Vector3(collider.size.x, -collider.size.y, collider.size.z) * 0.5f;
        Vertices[3] = collider.center + new Vector3(-collider.size.x, -collider.size.y, collider.size.z) * 0.5f;
    }

    // we get the world size by transforming the local points to global one and we store the size of the object after that
    public void CalculateSizeInCells()
    {
        Vector3Int[] vertices = new Vector3Int[Vertices.Length];

        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(Vertices[i]);
            vertices[i] =BuildingSystem.instance.gridLayout.WorldToCell(worldPos);
        }


        Size = new Vector3Int(Math.Abs((vertices[0] - vertices[1]).x), Math.Abs((vertices[0] - vertices[3]).y), 1);
        print("Placable Object Size" + Size);
    }

    //get the start position  of the object in world space 
    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }


    //initialize the object by calculating the vertces and size
    private void Start()
    {
        GetColliderVertexPositionsLocal();
        CalculateSizeInCells();
    }

    // flag object and unsubscribe from player input
    public virtual void Place(bool placed)
    {
        ObjectDrag drag = gameObject.GetComponent<ObjectDrag>();
        drag.Unsubscribe();
        Placed = placed;
    }


    // rotate the object and flip the verteces of the object which will make our size right
    public void Rotate()
    {

        Vector3 rotation= transform.rotation.eulerAngles;
        rotation.y += 90;
        transform.DORotate(rotation, 0.1f);

        Size = new Vector3Int(Size.y, Size.x, 1);
        Vector3 [] vertices = new Vector3[Vertices.Length];
        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Vertices[(i+1)%Vertices.Length];
        }

        Vertices = vertices;
    }

    // Delete this game object we make sure to unsubscribe from the listner first
    internal void DeleteObject()
    {
        ObjectDrag drag = gameObject.GetComponent<ObjectDrag>();
        drag.Unsubscribe();
        Placed = false;
        Destroy(gameObject);
    }

    [SerializeField]private bool canPlace =true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            canPlace = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Wall")
        {
            canPlace = true;
        }
    }

    public bool CanPlace()
    {

        return canPlace;
    }

}
