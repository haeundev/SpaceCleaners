public class TutorialDirect : ISpaceDirect
{
    private int _reservedID;
    public SpaceDirectType DirectType => SpaceDirectType.Tutorial;
    public void OnDirect(SpaceDirectType type)
    {
        IsRunning = true;
        TutorialManager.Run(_reservedID);
    }

    public bool BlockUserInput { get; }
    public bool IsRunning { get; set; }

    public void Reserve(int id)
    {
        _reservedID = id;
        SpaceDirector.Reserve(this);
    }

    public void OnDone(int id)
    {
        if (id != _reservedID)
            return;
        IsRunning = false;
    }
}