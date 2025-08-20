using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movment;
using System;
using System.Data;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [Serializable]
        public struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }
        public enum CursorType
        {
            None,
            enemy,
            Movment,
            UI
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;
        [SerializeField] GameObject playerCamera;
        private void Awake()
        {
           
        }


        void Update()
        {

            if (BuildingSystem.instance.GetSelectedMode() == Modes.Player)
            {
                if (InteractWithUI()) return;
                if (InteractWithComponant()) return;
                if (InteractWithMovment()) return;
            }
            

        }



        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponant()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);

            float[] distance = new float[hits.Length];

            for (int i = 0; i < distance.Length; i++)
            {
                distance[i] = hits[i].distance;
            }
            Array.Sort(distance, hits);


            return hits;
        }

        public bool InteractWithMovment()
        {
            Vector3 target = new Vector3();
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;
                if (Input.GetMouseButton(1))
                {
                   
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movment);
                return true;
            }
            SetCursor(CursorType.None);
            return false;
        }


        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hit);
            if (!hasHit) return false;
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) { return false; }
            target = navMeshHit.position;

            return true;
        }



        private void SetCursor(CursorType type)
        {

            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
        private CursorMapping GetCursorMapping(CursorType type)
        {
            CursorMapping mappin = cursorMappings[0];
            foreach (var mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    mappin = mapping;
                    break;
                }
            }
            return mappin;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        internal void EnableCamera(bool v)
        {
            playerCamera.SetActive(v);
        }
    }

}