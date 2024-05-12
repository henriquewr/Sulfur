using Sulfur.Runtime.Eval;
using System;
using static Sulfur.Main.Ast;
using static Sulfur.Runtime.Values;

namespace Sulfur.Runtime
{
    public class Interpreter
    {
        public static IRuntimeVal Evaluate(IStmt astNode, Environment env, bool evalIf = true)
        {
            switch (astNode.Kind)
            {
                case NodeType.IntLiteral:
                    var numericLiteral = new IntVal 
                    { 
                        Value = ((IIntLiteral)astNode).Value 
                    };
                    return numericLiteral;

                case NodeType.LongLiteral:
                    var longLiteral = new LongVal
                    {
                        Value = ((ILongLiteral)astNode).Value
                    };
                    return longLiteral;

                case NodeType.StringLiteral:
                    var stringLiteral = new StringVal
                    {
                        Value = ((IStringLiteral)astNode).Value
                    };
                    return stringLiteral;

                case NodeType.Identifier:
                    var identifierValue = Expressions.EvalIdentifier((IIdentifier)astNode, env);
                    return identifierValue;

                case NodeType.ObjectLiteral:
                    var objectLiteral = Expressions.EvalObjectExpr((IObjectLiteral)astNode, env);
                    return objectLiteral;

                case NodeType.CallExpr:
                    var callExpr = Expressions.EvalCallExpr((ICallExpr)astNode, env);
                    return callExpr;

                case NodeType.BinaryExpr:
                    var binaryExprValue = Expressions.EvalBinaryExpr((IBinaryExpr)astNode, env);
                    return binaryExprValue;

                case NodeType.Program:
                    var programValue = Statements.EvalProgram((IProgram)astNode, env);
                    return programValue;

                case NodeType.VariableDelaration:
                    var varDeclaration = Statements.EvalVarDeclaration((IVarDeclaration)astNode, env);
                    return varDeclaration;

                case NodeType.FunctionDeclaration:
                    var funcDeclaration = Statements.EvalFuncDeclaration((IFuncDeclaration)astNode, env);
                    return funcDeclaration;

                case NodeType.ReturnDeclaration:
                    var returnDeclaration = Statements.EvalReturnDeclaration((IReturnDeclaration)astNode, env);
                    return returnDeclaration;

                case NodeType.IfDeclaration:
                    var ifDeclaration = Statements.EvalIfDeclaration((IIfDeclaration)astNode, env, evalIf);
                    return ifDeclaration;

                case NodeType.AssignmentExpr:
                    var assignmentVal = Expressions.EvalAssignmentExpr((IAssignmentExpr)astNode, env);
                    return assignmentVal;

                case NodeType.LogicalExpr:
                    var logicalExprVal = Expressions.EvalLogicalExpr((ILogicalExpr)astNode, env);
                    return logicalExprVal;

                case NodeType.UnaryExpr:
                    var unaryExprVal = Expressions.EvalUnaryExpr((IUnaryExpr)astNode, env);
                    return unaryExprVal;

                case NodeType.MemberExpr:
                    var memberExprVal = Expressions.EvalMemberExpr((IMemberExpr)astNode, env);
                    return memberExprVal;

                default:
                    throw new InvalidOperationException($"This AST Node has not yet been setup for interpretation. {astNode}");
            }
        }
    }
}
