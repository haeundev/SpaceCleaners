using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(Tasks))]
    public class TasksEditor : DataScriptEditor
    {
        public override string FileID => "15EwIIfOuPGi2uXNPlZmv2U4EUB5YY30if30UxfryNGY";
        public override string SheetName => "Task";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(Task);

        public override void SetAssetData(string json)
        {
            var obj = target as Tasks;
            obj.Values = DataScript.MakeJsonListObjectString<Task>(json).values;
        }
    }
}

