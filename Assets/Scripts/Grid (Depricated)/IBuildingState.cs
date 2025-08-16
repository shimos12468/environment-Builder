using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3Int gridPosition);
    void RotatePrefab();
    void UpdateState(Vector3Int gridPosition);
}