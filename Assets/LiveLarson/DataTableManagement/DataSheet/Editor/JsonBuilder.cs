﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiveLarson.Util;

namespace LiveLarson.DataTableManagement.DataSheet.Editor
{
    public class JsonBuilder
    {
        private IList<IList<object>> _values;

        public string Build(DataScript.DataType type, DataClass baseClass, IList<IList<object>> values,
            string outJsonPath)
        {
            _values = values;
            using (var sw = new StreamWriter(outJsonPath))
            {
                if (type == DataScript.DataType.Table)
                {
                    sw.WriteLine("[");
                    WriteClasses(sw, baseClass, 1, 2);
                    sw.WriteLine("]");
                }
                else if (type == DataScript.DataType.Const)
                {
                    WriteClasses(sw, baseClass, 0, 2);
                }
            }

            string json;
            using (var sr = new StreamReader(outJsonPath))
            {
                json = sr.ReadToEnd();
            }

            return json;
        }

        public string BuildRaw(string fileID, IList<IList<object>> values, string outJsonPath)
        {
            _values = values;
            using (var sw = new StreamWriter(outJsonPath))
            {
                sw.Write($"{{\"id\":\"{fileID}\",");
                sw.Write("\"data\":[");
                var hasLine = false;
                var firstLine = true;
                var exceptColumns = new List<int>();

                var lineLength = 0;
                foreach (var line in values)
                {
                    sw.WriteLine(hasLine ? "," : "");
                    hasLine = true;
                    sw.Write("[");
                    var first = true;
                    if (firstLine) lineLength = line.Count;
                    for (var i = 0; i < lineLength; i++)
                    {
                        if (!firstLine && exceptColumns.Contains(i))
                            continue;
                        var text = line.Count > i ? line[i].ToString().Trim() : "";
                        if (firstLine && string.IsNullOrEmpty(text))
                        {
                            exceptColumns.Add(i);
                            continue;
                        }

                        sw.Write(first ? $"\"{text}\"" : $", \"{text}\"");
                        first = false;
                    }

                    firstLine = false;
                    sw.Write("]");
                }

                sw.WriteLine("");
                sw.WriteLine("]}");
            }

            string json;
            using (var sr = new StreamReader(outJsonPath))
            {
                json = sr.ReadToEnd();
            }

            return json;
        }

        public string BuildTable(DataClass baseClass, IList<IList<object>> values, string outJsonPath)
        {
            return Build(DataScript.DataType.Table, baseClass, values, outJsonPath);
        }

        public string BuildConst(DataClass baseClass, IList<IList<object>> values, string outJsonPath)
        {
            return Build(DataScript.DataType.Const, baseClass, values, outJsonPath);
        }

        private int WriteClass(StreamWriter sw, DataClass dataClass, int indent, int row)
        {
            var indentString = GetIndentString(indent);
            var indentString2 = GetIndentString(indent + 1);
            sw.WriteLine(indentString + "{");
            var hasValue = WriteMembers(sw, dataClass, indent + 1, row);
            if (dataClass.DataClasses.Count > 0)
            {
                var resultRow = row;
                foreach (var subClass in dataClass.DataClasses)
                {
                    if (hasValue)
                        sw.WriteLine(",");
                    sw.WriteLine($"{indentString2}\"{subClass.Name.ToPlural()}\" : [");
                    var tempRow = WriteClasses(sw, subClass, indent + 2, row);
                    if (resultRow < tempRow)
                        resultRow = tempRow;
                    sw.WriteLine(indentString2 + "]");
                    hasValue = true;
                }

                row = resultRow;
            }

            if (hasValue) sw.WriteLine();
            sw.Write(indentString + "}");
            return row;
        }

        private bool WriteMembers(StreamWriter sw, DataClass dataClass, int indent, int row)
        {
            var hasValue = false;
            foreach (var member in dataClass.DataMembers) WriteMember(sw, member, indent, row, ref hasValue);
            return hasValue;
        }

        private void WriteMember(StreamWriter sw, DataMember member, int indent, int row, ref bool hasValue)
        {
            var rowValues = _values[row];
            if (rowValues.Count <= member.Column)
                return;
            var data = rowValues[member.Column];
            if (data == null || string.IsNullOrEmpty(data.ToString()))
                return;

            if (hasValue)
                sw.WriteLine(",");
            hasValue = true;

            var indentString = GetIndentString(indent);
            if (member.IsArray)
            {
                var valueString = data.ToString();
                if (member.Type.Equals("bool")) valueString = valueString.ToLower();
                if (IsStringType(member))
                {
                    var strs = valueString.Split(member.Separator.ToCharArray());
                    valueString = string.Concat(strs.Select(p => $"\"{p.Trim()}\","));
                    valueString = valueString.TrimEnd(',');
                }

                sw.Write($"{indentString}\"{member.Name.ToPlural()}\" : [{valueString}]");
            }
            else
            {
                var value = data.ToString().Trim();
                var valueString = IsStringType(member) ? $"\"{value}\"" : value;
                if (member.Type.Equals("bool")) valueString = valueString.ToLower();
                sw.Write($"{indentString}\"{member.Name}\" : {valueString}");
            }
        }

        private bool IsStringType(DataMember member)
        {
            return member.Type.Equals("string") || member.IsEnum;
        }

        private int WriteClasses(StreamWriter sw, DataClass dataClass, int indent, int startRow)
        {
            var endRow = GetEndRow(dataClass, startRow);
            for (var row = startRow; row < endRow; row++)
            {
                if (row != startRow)
                    sw.WriteLine(",");
                row = WriteClass(sw, dataClass, indent, row);
            }

            sw.WriteLine("");
            return endRow - 1;
        }

        private int GetEndRow(DataClass dataClass, int startRow)
        {
            var length = _values.Count;
            if (dataClass == null || dataClass.Parent == null)
                return length;

            if (dataClass.Parent.DataMembers.Count == 0)
                return length;

            var keyMember = dataClass.Parent.DataMembers[0];
            var column = keyMember.Column;
            for (var i = startRow + 1; i < length; i++)
            {
                if (_values[i].Count <= column)
                    continue;
                var data = _values[i][column];
                if (data == null || string.IsNullOrEmpty(data.ToString()))
                    continue;
                return i;
            }

            return length;
        }

        private string GetIndentString(int indentLevel)
        {
            return new string(' ', indentLevel * 4);
        }
    }
}