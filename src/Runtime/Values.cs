using System;
using System.Collections.Generic;
using static Sulfur.Main.Ast;

namespace Sulfur.Runtime
{
    public class Values
    {
        public enum ValueType
        {
            Null,
            Int,
            Long,
            String,
            Boolean,
            Object,
            NativeFunc,
            Func,
            If,
        }

        public interface IRuntimeVal
        {
            ValueType Type { get; }
        }


        public interface INullVal : IRuntimeVal
        {
            string Value { get; }
        }
        private static NullVal _nullVal;
        public class NullVal : INullVal
        {
            public ValueType Type => ValueType.Null;
            public string Value => "null";
            public static INullVal Create()
            {
                _nullVal ??= new NullVal();
                return _nullVal;
            }
        }


        public interface IIntVal : IRuntimeVal
        {
            int Value { get; set; }
        }
        public class IntVal : IIntVal
        {
            public ValueType Type => ValueType.Int;
            public int Value { get; set; }
            public static IIntVal Create(int val = 0)
            {
                var numVal = new IntVal
                {
                    Value = val,
                };
                return numVal;
            }
        }


        public interface ILongVal : IRuntimeVal
        {
            long Value { get; set; }
        }
        public class LongVal : ILongVal
        {
            public ValueType Type => ValueType.Long;
            public long Value { get; set; }
            public static ILongVal Create(long val = 0)
            {
                var numVal = new LongVal
                {
                    Value = val,
                };
                return numVal;
            }
        }


        public interface IBooleanVal : IRuntimeVal
        {
            bool Value { get; set; }
        }
        public class BooleanVal : IBooleanVal
        {
            public ValueType Type => ValueType.Boolean;
            public bool Value { get; set; }
            public static IBooleanVal Create(bool val = false)
            {
                var booleanVal = new BooleanVal
                {
                    Value = val,
                };
                return booleanVal;
            }
        }


        public interface IStringVal : IRuntimeVal
        {
            string Value { get; set; }
        }
        public class StringVal : IStringVal
        {
            public ValueType Type => ValueType.String;
            public string Value { get; set; }
            public static IStringVal Create(string val = "")
            {
                var stringVal = new StringVal
                {
                    Value = val,
                };
                return stringVal;
            }
        }


        public interface IObjectVal : IRuntimeVal
        {
            Dictionary<string, IRuntimeVal> Properties { get; set; }
        }
        public class ObjectVal : IObjectVal
        {
            public ValueType Type => ValueType.Object;
            public Dictionary<string, IRuntimeVal> Properties { get; set; }
        }


        public interface INativeFuncVal : IRuntimeVal
        {
            Func<List<IRuntimeVal>, Environment, IRuntimeVal> Call { get; set; }
        }
        public class NativeFuncVal : INativeFuncVal
        {
            public ValueType Type => ValueType.NativeFunc;
            public Func<List<IRuntimeVal>, Environment, IRuntimeVal> Call { get; set; }
            public static NativeFuncVal Create(Func<List<IRuntimeVal>, Environment, IRuntimeVal> call)
            {
                var nativeFunc = new NativeFuncVal
                {
                    Call = call,
                };
                return nativeFunc;
            }
        }


        public interface IFuncVal : IRuntimeVal
        {
            string Name { get; set; }
            List<string> Parameters { get; set; }
            Environment DeclarationEnv { get; set; }
            List<IStmt> Body { get; set; }
        }
        public class FuncVal : IFuncVal
        {
            public ValueType Type => ValueType.Func;
            public string Name { get; set; }
            public List<string> Parameters { get; set; }
            public Environment DeclarationEnv { get; set; }
            public List<IStmt> Body { get; set; }
        }


        public interface IIfVal : IRuntimeVal
        {
            Environment DeclarationEnv { get; set; }
            List<IStmt> Body { get; set; }
        }
        public class IfVal : IIfVal
        {
            public ValueType Type => ValueType.If;
            public Environment DeclarationEnv { get; set; }
            public List<IStmt> Body { get; set; }
        }
    }
}
