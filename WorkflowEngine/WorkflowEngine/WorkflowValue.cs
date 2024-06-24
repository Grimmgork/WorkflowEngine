using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public struct WorkflowValue
    {
        public WorkflowDataType Type;
        public object? Value;

        public bool IsDefined => Type != WorkflowDataType.Undef;
        public bool IsNull => Type == WorkflowDataType.Null;
        public bool IsError => Type == WorkflowDataType.Error;
        public bool IsInt => Type == WorkflowDataType.Int;
        public bool IsString => Type == WorkflowDataType.String;
        public bool IsBool => Type == WorkflowDataType.Bool;

        public WorkflowValue()
        {
            this.Value = null;
            this.Type = WorkflowDataType.Undef;
        }

        public WorkflowValue(WorkflowDataType type, object? value = null)
        {
            this.Value = value;
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

        public static WorkflowValue TryParse(object? value)
        {
            if (value == null)
                return WorkflowValue.Null();
            return value switch
            {
                int parsed => WorkflowValue.Int(parsed),
                bool parsed => WorkflowValue.Bool(parsed),
                string parsed => WorkflowValue.String(parsed),
                _ => WorkflowValue.Error()
            };
        }

        public static WorkflowValue operator +(WorkflowValue a, WorkflowValue b)
        {
            if (a.Type != b.Type)
                return WorkflowValue.Error(2);

            try
            {
                return a.Type switch
                {
                    WorkflowDataType.Int => WorkflowValue.Int((int)a.Value! + (int)b.Value!),
                    WorkflowDataType.String => WorkflowValue.String((string)b.Value! + (string)b.Value!),
                    _ => WorkflowValue.Error(1)
                };
            }
            catch
            {
                return WorkflowValue.Error(3);
            }
        }

        public int AsInt()
        {
            return As<int>();
        }

        public string AsString()
        {
            return As<string>();
        }

        public int AsFunctionRef()
        {
            return As<int>();
        }

        public bool AsBool()
        {
            return As<bool>();
        }

        public int AsOutputRef()
        {
            return As<int>();
        }

        private T As<T>()
        {
            if (Value!.GetType() == typeof(T))
                return (T)Value!;
            throw new Exception($"Value is not of type {typeof(T)}!");
        }

        public WorkflowValueObject AsObject()
        {
            return (WorkflowValueObject)Value!;
        }

        public override string ToString()
        {
            return $"{Type}#{Value}";
        }
    }
}
