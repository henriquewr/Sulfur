using System;
using System.Collections.Generic;
using System.Linq;
using static Sulfur.Main.Ast;
using static Sulfur.Runtime.Values;
using ValueType = Sulfur.Runtime.Values.ValueType;

namespace Sulfur.Runtime.Eval
{
    public class Expressions
    {
        private static readonly Dictionary<string, Func<bool, bool, bool>> BoolComparators = new()
        {
            { "==", (a, b) => a == b },
            { "!=", (a, b) => a != b },
        };
        private static readonly Dictionary<string, Func<int, int, bool>> IntComparators = new()
        {
            { "==", (a, b) => a == b },
            { "!=", (a, b) => a != b },
            { ">", (a, b) => a > b },
            { "<", (a, b) => a < b },
            { "<=", (a, b) => a <= b },
            { ">=", (a, b) => a >= b },
        };

        private static readonly Dictionary<string, Func<long, long, bool>> LongComparators = new()
        {
            { "==", (a, b) => a == b },
            { "!=", (a, b) => a != b },
            { ">", (a, b) => a > b },
            { "<", (a, b) => a < b },
            { "<=", (a, b) => a <= b },
            { ">=", (a, b) => a >= b },
        };

        private static readonly Dictionary<string, Func<int, int, int>> IntExpr = new()
        {
            { "+", (a, b) => a + b },
            { "-", (a, b) => a - b },
            { "/", (a, b) => a / b },
            { "*", (a, b) => a * b },
            { "%", (a, b) => a % b },
        };

        private static readonly Dictionary<string, Func<string, string, string>> StringExpr = new()
        {
            { "+", (a, b) => a + b },
        };

        private static readonly Dictionary<string, Func<string, string, bool>> StringComparators = new()
        {
            { "==", (a, b) => a == b },
            { "!=", (a, b) => a != b },
        };

        public static IRuntimeVal EvalIdentifier(IIdentifier identifier, Environment env)
        {
            var value = env.GetValue(identifier.Symbol);
            return value;
        }

        public static IRuntimeVal EvalObjectExpr(IObjectLiteral objectLiteral, Environment env)
        {
            var obj = new ObjectVal
            {
                Properties = new Dictionary<string, IRuntimeVal>(),
            };

            foreach(var property in objectLiteral.Properties)
            {
                var runtimeVal = property.Value is null ? env.GetValue(property.Key) : Interpreter.Evaluate(property.Value, env);

                obj.Properties.Add(property.Key, runtimeVal);
            }

            return obj;
        }

        private static IRuntimeVal EvalIntBinaryExpr(IIntVal lhs, IIntVal rhs, string oper)
        {
            if (IntComparators.TryGetValue(oper, out var comparator))
            {
                var boolVal = BooleanVal.Create();
                boolVal.Value = comparator(lhs.Value, rhs.Value);
                return boolVal;
            }

            if (IntExpr.TryGetValue(oper, out var expr))
            {
                var numberVal = IntVal.Create();

                numberVal.Value = expr(lhs.Value, rhs.Value);
                return numberVal;
            }

            return NullVal.Create();
        }
        
        private static IRuntimeVal EvalLongBinaryExpr(ILongVal lhs, ILongVal rhs, string oper)
        {
            if (LongComparators.TryGetValue(oper, out var comparator))
            {
                var boolVal = BooleanVal.Create();
                boolVal.Value = comparator(lhs.Value, rhs.Value);
                return boolVal;
            }

            return NullVal.Create();
        }
        private static IRuntimeVal EvalBooleanBinaryExpr(IBooleanVal lhs, IBooleanVal rhs, string oper)
        {
            if (BoolComparators.TryGetValue(oper, out var comparator))
            {
                var boolVal = BooleanVal.Create();
                boolVal.Value = comparator(lhs.Value, rhs.Value);
                return boolVal;
            }

            return NullVal.Create();
        }

        private static IRuntimeVal EvalStringBinaryExpr(IStringVal lhs, IStringVal rhs, string oper)
        {
            if (StringExpr.TryGetValue(oper, out var expr))
            {
                var strVal = StringVal.Create();
                strVal.Value = expr(lhs.Value, rhs.Value);
                return strVal;
            }

            if (StringComparators.TryGetValue(oper, out var comparator))
            {
                var boolVal = BooleanVal.Create();
                boolVal.Value = comparator(lhs.Value, rhs.Value);
                return boolVal;
            }
            
            return NullVal.Create();
        }

        public static IRuntimeVal EvalBinaryExpr(IBinaryExpr binaryExpr, Environment env)
        {
            var leftHandSide = Interpreter.Evaluate(binaryExpr.Left, env);
            var rightHandSide = Interpreter.Evaluate(binaryExpr.Right, env);

            if (leftHandSide.Type == ValueType.Int && rightHandSide.Type == ValueType.Int)
            {
                return EvalIntBinaryExpr((IIntVal)leftHandSide, (IIntVal)rightHandSide, binaryExpr.Operator);
            }

            if (leftHandSide.Type == ValueType.Boolean && rightHandSide.Type == ValueType.Boolean)
            {
                return EvalBooleanBinaryExpr((IBooleanVal)leftHandSide, (IBooleanVal)rightHandSide, binaryExpr.Operator);
            }

            if (leftHandSide.Type == ValueType.String && rightHandSide.Type == ValueType.String)
            {
                return EvalStringBinaryExpr((IStringVal)leftHandSide, (IStringVal)rightHandSide, binaryExpr.Operator);
            }

            if (leftHandSide.Type == ValueType.Long && rightHandSide.Type == ValueType.Long)
            {
                return EvalLongBinaryExpr((ILongVal)leftHandSide, (ILongVal)rightHandSide, binaryExpr.Operator);
            }

            return NullVal.Create();
        }

        public static IRuntimeVal EvalAssignmentExpr(IAssignmentExpr assignmentExpr, Environment env)
        {
            var value = Interpreter.Evaluate(assignmentExpr.Value, env);

            if (assignmentExpr.Assigne.Kind is NodeType.Identifier)
            {
                var varName = ((IIdentifier)assignmentExpr.Assigne).Symbol;
                var newValue = env.AssignVar(varName, value);
                return newValue;
            }
            else if(assignmentExpr.Assigne.Kind is NodeType.MemberExpr)
            {
                var memberExpr = (IMemberExpr)assignmentExpr.Assigne;
                var val = env.GetValue(((IIdentifier)memberExpr.Object).Symbol);
                if(val.Type == ValueType.Object)
                {
                    var objVal = (IObjectVal)val;
                    return objVal.Properties[((IIdentifier)memberExpr.Property).Symbol] = value;
                }
            }
            else
            {
                throw new Exception("Invalid assigne");
            }

            return NullVal.Create();
        }

        public static IRuntimeVal EvalLogicalExpr(ILogicalExpr logicalExpr, Environment env)
        {
            var leftHandSide = Interpreter.Evaluate(logicalExpr.Left, env);
            var rightHandSide = Interpreter.Evaluate(logicalExpr.Right, env);

            if (leftHandSide.Type == ValueType.Boolean && rightHandSide.Type == ValueType.Boolean)
            {
                var leftHandSideBool = (IBooleanVal)leftHandSide;
                var rightHandSideBool = (IBooleanVal)rightHandSide;

                var boolVal = BooleanVal.Create();

                if (logicalExpr.Operator == "&&")
                {
                    boolVal.Value = leftHandSideBool.Value && rightHandSideBool.Value;
                    return boolVal;
                }
                else if (logicalExpr.Operator == "||")
                {
                    boolVal.Value = leftHandSideBool.Value || rightHandSideBool.Value;
                    return boolVal;
                }
            }

            return NullVal.Create();
        }

        public static IRuntimeVal EvalUnaryExpr(IUnaryExpr unaryExpr, Environment env)
        {
            var arg = Interpreter.Evaluate(unaryExpr.Arg, env);

            if (arg.Type == ValueType.Boolean)
            {
                var boolVal = BooleanVal.Create();
                var argBool = (IBooleanVal)arg;

                if (unaryExpr.Operator == "!")
                {
                    boolVal.Value = !argBool.Value;
                    return boolVal;
                }
            }

            return NullVal.Create();
        }

        public static IRuntimeVal EvalMemberExpr(IMemberExpr memberExpr, Environment env)
        {
            var val = Interpreter.Evaluate(memberExpr.Object, env);
            if (val.Type == ValueType.Object)
            {
                var valObj = (IObjectVal)val;
               
                if(valObj.Properties.TryGetValue(((IIdentifier)memberExpr.Property).Symbol, out var runtimeVal))
                {
                    return runtimeVal;
                }
                else
                {
                    return NullVal.Create();
                }
            }

            var prop = Interpreter.Evaluate(memberExpr.Property, env);

            if (val.Type == ValueType.String)
            {
                var stringVal = (IStringVal)val;

                if (prop.Type == ValueType.Int)
                {
                    var propInt = (IIntVal)prop;
                    var newVal = stringVal.Value[propInt.Value].ToString();
                    return StringVal.Create(newVal);
                }
                else if (prop.Type == ValueType.Long)
                {
                    var propLong = (ILongVal)prop;
                    var valInt = (int)Math.Min(propLong.Value, int.MaxValue);
                    var newVal = stringVal.Value[valInt].ToString();
                    return StringVal.Create(newVal);
                }
            }

            return NullVal.Create();
        }

        public static IRuntimeVal EvalCallExpr(ICallExpr callExpr, Environment env)
        {
            var args = callExpr.Args.Select(x => Interpreter.Evaluate(x, env)).ToList();

            var func = Interpreter.Evaluate(callExpr.Caller, env);

            if(func.Type is ValueType.NativeFunc)
            {
                var result = ((INativeFuncVal)func).Call(args, env);
                return result;
            }
            else if(func.Type is ValueType.Func)
            {
                var fn = (IFuncVal)func;

                if(fn.Parameters.Count != args.Count)
                {
                    throw new ArgumentException("Function does not contains the correct params");
                }

                var scope = new Environment(fn.DeclarationEnv);

                for (int i = 0; i < fn.Parameters.Count; i++)
                {
                    var param = fn.Parameters[i];
                    var arg = args[i];
                    scope.DeclareVar(param, arg, false);
                }

                IRuntimeVal lastEval = NullVal.Create(); 

                for (int i = 0; i < fn.Body.Count; i++)
                {
                    var stmt = fn.Body[i];

                    lastEval = Interpreter.Evaluate(stmt, scope, false);

                    if (lastEval.Type == ValueType.If)
                    {
                        var ifLastEval = (IIfVal)lastEval;

                        var returnValue = EvalIfFirstReturn(ifLastEval, ifLastEval.DeclarationEnv, false);
                        if (returnValue is not null)
                        {
                            return returnValue;
                        }
                    }
                }

                return lastEval;
            }

            throw new InvalidOperationException($"Cannot call value that is not a function: {func}");
        }

        private static IRuntimeVal EvalIfFirstReturn(IIfVal ifExpr, Environment env, bool evalIf)
        {
            for (int i = 0; i < ifExpr.Body.Count; i++)
            {
                var item = ifExpr.Body[i];

                var val = Interpreter.Evaluate(item, env, evalIf);

                if (val.Type is ValueType.If)
                {
                    var ifDecla = (IIfVal)val;
                    var ret = EvalIfFirstReturn(ifDecla, ifDecla.DeclarationEnv, evalIf);
                    if (ret is not null)
                    {
                        return ret;
                    }
                }
                else if (item.Kind == NodeType.ReturnDeclaration)
                {
                    return val;
                }
            }

            return null;
        }
    }
}
