using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public struct WorkflowValue
    {
        public readonly WorkflowDataType Type;
        private object? value;

        public bool IsDefined => Type != WorkflowDataType.Undef;
        public bool IsNull => Type == WorkflowDataType.Null;
        public bool IsError => Type == WorkflowDataType.Error;
        public bool IsInt => Type == WorkflowDataType.Int;
        public bool IsString => Type == WorkflowDataType.String;
        public bool IsBool => Type == WorkflowDataType.Bool;
        public bool IsObject => Type == WorkflowDataType.Object;
        public bool IsArray => Type == WorkflowDataType.Array;

        public WorkflowValue()
        {
            this.value = null;
            this.Type = WorkflowDataType.Undef;
        }

        public WorkflowValue(WorkflowDataType type, object? value = null)
        {
            this.value = value;
            this.Type = type;
        }

        public static WorkflowValue Null()
        {
            return new WorkflowValue(WorkflowDataType.Null);
        }

        public static WorkflowValue Int(int value)
        {
            return new WorkflowValue(WorkflowDataType.Int, value);
        }

        public static WorkflowValue Bool(bool value)
        {
            return new WorkflowValue(WorkflowDataType.Bool, value);
        }

        public static WorkflowValue String(string value)
        {
            return new WorkflowValue(WorkflowDataType.String, value);
        }

        public static WorkflowValue Error(int value = 1)
        {
            return new WorkflowValue(WorkflowDataType.Error, value);
        }

        public static WorkflowValue OutputNode(int value)
        {
            return new WorkflowValue(WorkflowDataType.OutputRef, value);
        }

        public static WorkflowValue Function(int value)
        {
            return new WorkflowValue(WorkflowDataType.FunctionRef, value);
        }

        public static WorkflowValue Object(WorkflowValueObject obj)
        {
            return new WorkflowValue(WorkflowDataType.Object, obj);
        }

        public static WorkflowValue Array(WorkflowValueArray arr)
        {
            return new WorkflowValue(WorkflowDataType.Array, arr);
        }

        public static WorkflowValue operator +(WorkflowValue a, WorkflowValue b)
        {
            if (a.Type != b.Type)
                return WorkflowValue.Error(2);

            try
            {
                return a.Type switch
                {
                    WorkflowDataType.Int => WorkflowValue.Int(b.Int() + b.Int()),
                    WorkflowDataType.String => WorkflowValue.String(b.String() + b.String()),
                    _ => WorkflowValue.Error(1)
                };
            }
            catch
            {
                return WorkflowValue.Error(3);
            }
        }

        public int Int()
        {
            return As<int>();
        }

        public string String()
        {
            return As<string>();
        }

        public int FunctionRef()
        {
            return As<int>();
        }

        public bool Bool()
        {
            return As<bool>();
        }

        public int OutputRef()
        {
            return As<int>();
        }

        public WorkflowValueArray Array()
        {
            return As<WorkflowValueArray>();
        }

        private T As<T>()
        {
            if (value!.GetType() == typeof(T))
                return (T)value!;
            throw new Exception($"Value is not of type {typeof(T)}!");
        }

        public WorkflowValueObject Object()
        {
            return (WorkflowValueObject)value!;
        }

        public override string ToString()
        {
            return $"{Type}#{value}";
        }
    }
}
