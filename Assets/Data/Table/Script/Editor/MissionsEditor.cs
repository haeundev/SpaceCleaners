using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(Missions))]
    public class MissionsEditor : DataScriptEditor
    {
        public override string FileID => "15EwIIfOuPGi2uXNPlZmv2U4EUB5YY30if30UxfryNGY";
        public override string SheetName => "Mission";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(Mission);

        public override void SetAssetData(string json)
        {
            var obj = target as Missions;
            obj.Values = DataScript.MakeJsonListObjectString<Mission>(json).values;
        }
    }
}

