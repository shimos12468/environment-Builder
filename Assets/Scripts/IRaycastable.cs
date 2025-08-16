namespace RPG.Control
{
    internal interface IRaycastable
    {
        PlayerController.CursorType GetCursorType();
        bool HandleRaycast(PlayerController playerController);
        
    }
}