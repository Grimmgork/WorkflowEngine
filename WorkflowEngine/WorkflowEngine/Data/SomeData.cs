using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows.Data
{
    public struct SomeData
    {
        public readonly WorkflowDataType DataType = WorkflowDataType.Undefined;
        private readonly object? scalarValue = null;
        private readonly ICollection<SomeData>? arrayItems;
        private readonly IDictionary<string, SomeData>? structProperties;

        public ICollection<SomeData> Items => arrayItems ?? throw new InvalidOperationException("Not an Array.");

        public IDictionary<string, SomeData> Properties => structProperties ?? throw new InvalidOperationException("Not a Struct.");

        public object? Value => IsScalar ? scalarValue : throw new InvalidOperationException("Not a Scalar.");


        public bool IsNull => DataType == WorkflowDataType.Undefined;
        public bool IsError => DataType == WorkflowDataType.Error;
        public bool IsStruct => DataType == WorkflowDataType.Struct;
        public bool IsArray => DataType == WorkflowDataType.Array;
        public bool IsScalar => DataType != WorkflowDataType.Struct && DataType != WorkflowDataType.Array;

        public SomeData()
        {

        }

        public SomeData(WorkflowDataType type, object? value = null)
        {
            if (value is not null)
            {
                this.scalarValue = value;
                DataType = type;
            }
        }

        public SomeData(IEnumerable<SomeData> items)
        {
            DataType = WorkflowDataType.Array;
            this.arrayItems = new List<SomeData>();
            foreach (var item in items)
            {
                this.arrayItems.Add(item);
            }
        }

        public SomeData(IEnumerable<KeyValuePair<string, SomeData>> properties)
        {
            DataType = WorkflowDataType.Struct;
            this.structProperties = new Dictionary<string, SomeData>();
            foreach (var entry in properties)
            {
                this.structProperties.Add(entry.Key, entry.Value);
            }
        }

        public SomeData this[string name]
        {
            get 
            {
                if (structProperties == null)
                {
                    return SomeData.Undef();
                }
                else
                {
                    return structProperties.GetValueOrDefault(name);
                }
            }
            set 
            {
                Properties[name] = value;
            }
        }

        public SomeData this[int index]
        {
            get
            {
                if (arrayItems == null)
                {
                    return SomeData.Undef();
                }
                else
                {
                    return arrayItems.ElementAt(index);
                }
            }
        }

        public static SomeData Undef()
        {
            return new SomeData();
        }

        public static SomeData FromInt32(Int32? value)
        {
            return new SomeData(WorkflowDataType.Int32, value);
        }

        public static SomeData FromInt64(Int64? value)
        {
            return new SomeData(WorkflowDataType.Int64, value);
        }

        public static SomeData FromBoolean(bool? value)
        {
            return new SomeData(WorkflowDataType.Boolean, value);
        }

        public static SomeData FromString(string? value)
        {
            return new SomeData(WorkflowDataType.String, value);
        }

        public static SomeData FromSingle(Single? value)
        {
            return new SomeData(WorkflowDataType.Single, value);
        }

        public static SomeData FromError(WorkflowError? value)
        {
            return new SomeData(WorkflowDataType.Error, value);
        }

        public static SomeData FromOutput(WorkflowOutputRef? value)
        {
            return new SomeData(WorkflowDataType.Output, value);
        }

        public static SomeData FromMethod(WorkflowMethodRef? value)
        {
            return new SomeData(WorkflowDataType.Method, value);
        }

        public static SomeData FromOutput(int methodId, string name)
        {
            return new SomeData(WorkflowDataType.Output, new WorkflowOutputRef(methodId, name));
        }

        public static SomeData FromVariable(string name)
        {
            return new SomeData(WorkflowDataType.Variable, new WorkflowVariableRef(name));
        }

        public static SomeData FromVariable(WorkflowVariableRef value)
        {
            return new SomeData(WorkflowDataType.Variable, value);
        }


        public static SomeData FromPolymorphicObject(object? value)
        {
            if (value == null)
            {
                return SomeData.Undef();
            }
            else if(value is SomeData someData)
            {
                return someData;
            }
            else if (value is IDictionary properties)
            {
                SomeData structure = SomeData.Struct();
                foreach (DictionaryEntry entry in properties)
                {
                    structure.Properties.Add((string)entry.Key, SomeData.FromPolymorphicObject(entry.Value));
                }
                return structure;
            }
            else if (value is IEnumerable items and not string)
            {
                SomeData array = SomeData.Array();
                foreach (object? item in items)
                {
                    array.Items.Add(FromPolymorphicObject(item));
                }
                return array;
            }
            else
            {
                return value switch
                {
                    Int32 v => FromInt32(v),
                    Int64 v => FromInt64(v),
                    String v => FromString(v),
                    Boolean v => FromBoolean(v),
                    Single v => FromSingle(v),
                    WorkflowMethodRef v => FromMethod(v),
                    WorkflowOutputRef v => FromOutput(v),
                    _ => throw new Exception($"Cannot convert {value.GetType()} into {nameof(SomeData)}")
                };
            }
        }

        public static SomeData Struct(IEnumerable<KeyValuePair<string, SomeData>> properties)
        {
            return new SomeData(properties);
        }

        public static SomeData Struct()
        {
            return new SomeData(Enumerable.Empty<KeyValuePair<string, SomeData>>());
        }

        public static SomeData Array(params SomeData[] items)
        {
            return new SomeData(WorkflowDataType.Array, items);
        }

        public static SomeData Array(IEnumerable<SomeData> items)
        {
            return new SomeData(WorkflowDataType.Array, items);
        }

        public static explicit operator Int32(SomeData value) => value.ToInt32();
        public static explicit operator String(SomeData value) => value.ToString();
        public static explicit operator Boolean(SomeData value) => value.ToBoolean();

        public int ToInt32()
        {
            return Convert.ToInt32(scalarValue);
        }

        public new string ToString()
        {
            if (IsNull)
                return "NULL";

            if (IsArray)
                return "ARRAY";

            if (IsStruct)
                return "STRUCT";

            return Convert.ToString(scalarValue) ?? "";
        }

        public bool ToBoolean()
        {
            return Convert.ToBoolean(scalarValue);
        }

        public WorkflowMethodRef ToMethodRef()
        {
            if (scalarValue == null)
            {
                return default;
            }
            else
            {
                return (WorkflowMethodRef)scalarValue;
            }
        }

        public WorkflowOutputRef ToOutputRef()
        {
            return (WorkflowOutputRef)scalarValue!;
        }

        public WorkflowVariableRef ToVariableRef()
        {
            return (WorkflowVariableRef)scalarValue!;
        }

        public object? ToPolymorphicObject()
        {
            if (IsNull)
            {
                return null;
            }
            else if (IsArray)
            {
                List<object?> items = new List<object?>();
                foreach (SomeData item in arrayItems!)
                {
                    items.Add(item.ToPolymorphicObject());
                }
                return items;
            }
            else if (IsStruct)
            {
                Dictionary<string, object?> properties = new Dictionary<string, object?>();
                foreach (KeyValuePair<string, SomeData> property in structProperties!)
                {
                    properties.Add(property.Key, property.Value.ToPolymorphicObject());
                }
                return properties;
            }
            else
            {
                return scalarValue;
            }
        }

        public SomeData Clone()
        {
            if (IsArray)
            {
                return SomeData.Array(arrayItems!.Select(i => i.Clone()));
            }
            else if (IsStruct)
            {
                SomeData result = SomeData.Struct();
                foreach (KeyValuePair<string, SomeData> pair in Properties!)
                    result.Properties.Add(pair.Key, pair.Value);

                return result;
            }
            else
            {
                return FromPolymorphicObject(scalarValue);
            }
        }
    }
}
