using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sulfur.Main.Ast;
using static Sulfur.Runtime.Values;
using ValueType = Sulfur.Runtime.Values.ValueType;

namespace Sulfur.Runtime.Eval
{
    public class Statements
    {
        public static IRuntimeVal EvalProgram(IProgram program, Environment env)
        {
            IRuntimeVal lastEvaluated = NullVal.Create();
            foreach (var statement in program.Body)
            {
                lastEvaluated = Interpreter.Evaluate(statement, env);
            }

            return lastEvaluated;
        }

        public static IRuntimeVal EvalVarDeclaration(IVarDeclaration varDeclaration, Environment env)
        {
            var value = varDeclaration.Value is null ? NullVal.Create() : Interpreter.Evaluate(varDeclaration.Value, env);
            var variable = env.DeclareVar(varDeclaration.Identifier, value, varDeclaration.IsConstant);

            return variable;
        }

        public static IRuntimeVal EvalFuncDeclaration(IFuncDeclaration funcDeclaration, Environment env)
        {
            var func = new FuncVal 
            { 
                Name = funcDeclaration.Name,
                Parameters = funcDeclaration.Params,
                DeclarationEnv = env,
                Body = funcDeclaration.Body
            };

            env.DeclareVar(func.Name, func, true);

            return func;
        }

        public static IRuntimeVal EvalReturnDeclaration(IReturnDeclaration returnDeclaration, Environment env)
        {
            var eval = Interpreter.Evaluate(returnDeclaration.Arg, env);
            return eval;
        }

        public static IRuntimeVal EvalIfDeclaration(IIfDeclaration ifDeclaration, Environment env, bool evalIfContents)
        {
            var condition = Interpreter.Evaluate(ifDeclaration.Condition, env);

            var declarationEnv = new Environment(env);

            var bodyToExecute = condition is IBooleanVal val && val.Value ? ifDeclaration.Body : ifDeclaration.ElseBody;

            var ifValue = new IfVal 
            { 
                DeclarationEnv = declarationEnv,
                Body = bodyToExecute,
            };

            if (evalIfContents == true)
            {
                return EvalIf(ifValue);
            }

            return ifValue;
        }

        public static IRuntimeVal EvalIf(IIfVal ifVal)
        {
            var env = ifVal.DeclarationEnv;

            for (int i = 0; i < ifVal.Body.Count; i++)
            {
                var item = ifVal.Body[i];

                var val = Interpreter.Evaluate(item, env);

                if (val.Type is ValueType.If)
                {
                    var ifDecla = val as IIfVal;
                    EvalIf(ifDecla);
                }
            }

            return NullVal.Create();
        }
    }
}
