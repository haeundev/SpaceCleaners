using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(GadgetInfos))]
    public class GadgetInfosEditor : DataScriptEditor
    {
        public override string FileID => "15EwIIfOuPGi2uXNPlZmv2U4EUB5YY30if30UxfryNGY";
        public override string SheetName => "GadgetInfo";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(GadgetInfo);

        public override void SetAssetData(string json)
        {
            var obj = target as GadgetInfos;
            obj.Values = DataScript.MakeJsonListObjectString<GadgetInfo>(json).values;
        }
    }
}

