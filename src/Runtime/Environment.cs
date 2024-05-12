using Sulfur.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Sulfur.Runtime.Values;

namespace Sulfur.Runtime
{
    public class Environment
    {
        public Environment(Environment parentEnv) 
        {
            Parent = parentEnv;
            Variables = new Dictionary<string, IRuntimeVal>();
            Constants = new HashSet<string>();
        }
        private Environment Parent { get; set; }
        private Dictionary<string, IRuntimeVal> Variables { get; set; }
        private HashSet<string> Constants { get; set; }

        public IRuntimeVal DeclareVar(string name, IRuntimeVal value, bool isContant) 
        { 
            if (Variables.ContainsKey(name)) 
            {
                throw new InvalidOperationException($"A variable name {name} already exists in the current context");
            }

            Variables.Add(name, value);

            if (isContant)
            {
                Constants.Add(name);
            }

            return value;
        }

        public IRuntimeVal GetValue(string varName)
        {
            var env = Resolve(varName);

            var value = env.Variables[varName];

            return value;
        }

        public IRuntimeVal AssignVar(string varName, IRuntimeVal value)
        {
            var env = Resolve(varName);

            if (env.Constants.Contains(varName))
            {
                throw new Exception($"Cannot reasign to variable '{varName}' as it was declared constant.");
            }

            env.Variables[varName] = value;
            
            return value;
        }

        public Environment Resolve(string varName)
        {
            if (Variables.ContainsKey(varName))
            {
                return this;
            }

            if (Parent is null)
            {
                throw new Exception($"Cannot resolve '{varName}' as it does not exists.");
            }

            return Parent.Resolve(varName);
        }

        public static Environment CreateGlobalEnv()
        {
            var env = new Environment(null!);
            env.DeclareVar("null", NullVal.Create(), true);
            env.DeclareVar("false", BooleanVal.Create(false), true);
            env.DeclareVar("true", BooleanVal.Create(true), true);

            env.DeclareVar("time", NativeFuncVal.Create((args, scope) =>
            {
                var timeTicks = DateTime.Now.Ticks;
                return LongVal.Create(timeTicks);
            }), true);

            env.DeclareVar("print", NativeFuncVal.Create((args, scoupe) =>
            {
                string str;
                if(args.Count < 3)
                {
                    str = ProcessSmallPrint(args);
                }
                else
                {
                    str = ProcessPrint(args).ToString();
                }
                
                Console.Write(str);
                return NullVal.Create();
            }), true);

            env.DeclareVar("println", NativeFuncVal.Create((args, scope) =>
            {
                string str;
                if (args.Count < 3)
                {
                    str = ProcessSmallPrint(args);
                }
                else
                {
                    str = ProcessPrint(args).ToString();
                }

                Console.WriteLine(str);
                return NullVal.Create();
            }), true);

            env.DeclareVar("consoleRead", NativeFuncVal.Create((args, scope) =>
            {
                var str = Console.ReadLine();
                return StringVal.Create(str ?? "");
            }), true);

            env.DeclareVar("consoleClear", NativeFuncVal.Create((args, scope) =>
            {
                Console.Clear();
                return NullVal.Create();
            }), true);

            env.DeclareVar("strLen", NativeFuncVal.Create((args, scope) =>
            {
                if (args.Any(x => x.Type is not Values.ValueType.String))
                {
                    return NullVal.Create();
                }

                var strArgs = args.Select(x => (IStringVal)x);

                var totalLength = strArgs.Aggregate(0, (acc, val) => acc + val.Value.Length);
                var intVal = IntVal.Create(totalLength);

                return intVal;
            }), true);

            env.DeclareVar("strSetAtIndex", NativeFuncVal.Create((args, scope) =>
            {
                if (args.Count <= 2)
                {
                    return NullVal.Create();
                }

                var sourceStr = args[0] as IStringVal;
                var index = args[1] as IIntVal;
                var val = args[2] as IStringVal;

                if(sourceStr is null || index is null || val is null)
                {
                    return NullVal.Create();
                }

                var sourceString = sourceStr.Value;

                var newString = sourceString.ReplaceAt(index.Value, val.Value);

                var strVal = StringVal.Create(newString);

                return strVal;
            }), true);

            env.DeclareVar("strContains", NativeFuncVal.Create((args, scope) =>
            {
                if (args.Count <= 1)
                {
                    return NullVal.Create();
                }

                var sourceStr = args[0] as IStringVal;
                var containStr = args[1] as IStringVal;

                if (sourceStr is null || containStr is null)
                {
                    return NullVal.Create();
                }

                var strContains = sourceStr.Value.Contains(containStr.Value);

                var boolVal = BooleanVal.Create(strContains);

                return boolVal;
            }), true);

            env.DeclareVar("strRepeat", NativeFuncVal.Create((args, scope) =>
            {
                if (args.Count <= 1)
                {
                    return NullVal.Create();
                }

                var sourceStr = args[0] as IStringVal;
                var times = args[1] as IIntVal;

                if (sourceStr is null || times is null)
                {
                    return NullVal.Create();
                }

                if (times.Value <= 0)
                {
                    return sourceStr;
                }

                var stringBuilder = new StringBuilder(sourceStr.Value.Length * times.Value);

                for (int i = 0; i < times.Value; i++)
                {
                    stringBuilder.Append(sourceStr.Value);
                }

                var strVal = StringVal.Create(stringBuilder.ToString());

                return strVal;
            }), true);

            return env;
        }

        private static StringBuilder ProcessPrint(List<IRuntimeVal> args)
        {
            var sw = new StringBuilder();

            for (int i = 0; i < args.Count; i++)
            {
                var item = args[i];

                switch (item.Type)
                {
                    case Values.ValueType.Int:
                        sw.Append(((IIntVal)item).Value).Append(' ');
                        break;

                    case Values.ValueType.Long:
                        sw.Append(((ILongVal)item).Value).Append(' ');
                        break;

                    case Values.ValueType.Boolean:
                        sw.Append(((IBooleanVal)item).Value).Append(' ');
                        break;

                    case Values.ValueType.String:
                        sw.Append(((IStringVal)item).Value).Append(' ');
                        break;

                    case Values.ValueType.Null:
                        sw.Append(((INullVal)item).Value).Append(' ');
                        break;
                }
            }

            return sw;
        }

        private static string ProcessSmallPrint(List<IRuntimeVal> args)
        {
            var str = "";

            for (int i = 0; i < args.Count; i++)
            {
                var item = args[i];

                switch (item.Type)
                {
                    case Values.ValueType.Int:
                        str += ((IIntVal)item).Value + " ";
                        break;

                    case Values.ValueType.Long:
                        str += ((ILongVal)item).Value + " ";
                        break;

                    case Values.ValueType.Boolean:
                        str += ((IBooleanVal)item).Value + " ";
                        break;

                    case Values.ValueType.String:
                        str += ((IStringVal)item).Value + " ";
                        break;

                    case Values.ValueType.Null:
                        str += ((INullVal)item).Value + " ";
                        break;
                }
            }

            return str;
        }
    }
}
