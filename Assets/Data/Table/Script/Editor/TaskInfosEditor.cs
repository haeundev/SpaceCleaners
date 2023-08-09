using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(TaskInfos))]
    public class TaskInfosEditor : DataScriptEditor
    {
        public override string FileID => "15EwIIfOuPGi2uXNPlZmv2U4EUB5YY30if30UxfryNGY";
        public override string SheetName => "TaskInfo";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(TaskInfo);

        public override void SetAssetData(string json)
        {
            var obj = target as TaskInfos;
            obj.Values = DataScript.MakeJsonListObjectString<TaskInfo>(json).values;
        }
    }
}

