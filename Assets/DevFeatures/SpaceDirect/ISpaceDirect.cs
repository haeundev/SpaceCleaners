public enum SpaceDirectType
{
    Tutorial,
    Reward,
    AgentLevelUp,
}

public interface ISpaceDirect
{
    public SpaceDirectType DirectType { get; }
    public void OnDirect(SpaceDirectType type);
    public bool BlockUserInput { get; }
    public bool IsRunning { get; set; }
}
