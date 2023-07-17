using Proto.Data;

namespace Proto.Booting
{
    public class DataTableBootTarget : DontDestroyBootTarget
    {
        protected override void Configure()
        {
            base.Configure();
            DataTableManager.Instance = TargetInstance.GetComponent<DataTableManager>();
            DataTableManager.IsLoaded.SetValueAndForceNotify(true);
        }
    }
}