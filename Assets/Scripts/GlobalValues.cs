public static class GlobalValues
{
    public static int InitialTaskID = 1;

    public static void SetInitialTaskID(int id)
    {
        InitialTaskID = id;
        TaskManager.Instance.OnGlobalCurrentTaskSet();
    }
}