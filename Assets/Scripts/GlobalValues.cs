using System;

public static class GlobalValues
{
    public static int InitialTaskID = 1;
    public static event Action<int> OnGlobalInitTaskSet;

    public static void SetInitialTaskID(int id)
    {
        InitialTaskID = id;
        // TaskManager.Instance.OnGlobalCurrentTaskSet();
        OnGlobalInitTaskSet?.Invoke(id);
    }
}