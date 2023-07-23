using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(ItemInfos))]
    public class ItemInfosEditor : DataScriptEditor
    {
        public override string FileID => "15EwIIfOuPGi2uXNPlZmv2U4EUB5YY30if30UxfryNGY";
        public override string SheetName => "ItemInfo";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(ItemInfo);

        public override void SetAssetData(string json)
        {
            var obj = target as ItemInfos;
            obj.Values = DataScript.MakeJsonListObjectString<ItemInfo>(json).values;
        }
    }
}

