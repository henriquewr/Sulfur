using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sulfur.Main.Ast;

namespace Sulfur.Main
{
    public class Ast
    {
        public enum NodeType
        {
            Program,
            VariableDelaration,
            FunctionDeclaration,
            ReturnDeclaration,
            IfDeclaration,

            AssignmentExpr,
            MemberExpr,
            CallExpr,

            Property,
            ObjectLiteral,
            IntLiteral,
            StringLiteral,
            LongLiteral,

            Identifier,
            BinaryExpr,
            UnaryExpr,
            LogicalExpr,
        }
        public interface IStmt
        {
            NodeType Kind { get; }
        }

        public interface IProgram : IStmt
        {
            List<IStmt> Body { get; set; }
        }
        public class Program : IProgram
        {
            public Program()
            {
                Body = new List<IStmt>();
            }
            public NodeType Kind => NodeType.Program;
            public List<IStmt> Body { get; set; }
        }

        public interface IVarDeclaration : IStmt
        {
            bool IsConstant { get; set; }
            string Identifier { get; set; }
            IExpr Value { get; set; }
        }

        public class VarDeclaration : IVarDeclaration
        {
            public NodeType Kind => NodeType.VariableDelaration;
            public bool IsConstant { get; set; }
            public string Identifier { get; set; }
            public IExpr Value { get; set; }
        }


        public interface IFuncDeclaration : IStmt
        {
            List<string> Params { get; set; }
            string Name { get; set; }
            List<IStmt> Body { get; set; }
        }

        public class FuncDeclaration : IFuncDeclaration
        {
            public NodeType Kind => NodeType.FunctionDeclaration;
            public List<string> Params { get; set; }
            public string Name { get; set; }
            public List<IStmt> Body { get; set; }
        }

        public interface IReturnDeclaration : IStmt
        {
            IStmt Arg { get; set; }
        }

        public class ReturnDeclaration : IReturnDeclaration
        {
            public NodeType Kind => NodeType.ReturnDeclaration;
            public IStmt Arg { get; set; }
        }

        public interface IIfDeclaration : IStmt
        {
            IStmt Condition { get; set; }
            List<IStmt> Body { get; set; }
            List<IStmt> ElseBody { get; set; }
        }

        public class IfDeclaration : IIfDeclaration
        {
            public NodeType Kind => NodeType.IfDeclaration;
            public IStmt Condition { get; set; }
            public List<IStmt> Body { get; set; }
            public List<IStmt> ElseBody { get; set; }
        }

        public interface IBinaryExpr : IExpr
        {
            IExpr Left { get; set; }
            IExpr Right { get; set; }
            string Operator { get; set; }
        }

        public class BinaryExpr : IBinaryExpr
        {
            public NodeType Kind => NodeType.BinaryExpr;
            public IExpr Left { get; set; }
            public IExpr Right { get; set; }
            public string Operator { get; set; }
        }

        public interface IUnaryExpr : IExpr
        {
            IExpr Arg { get; set; }
            string Operator { get; set; }
        }

        public class UnaryExpr : IUnaryExpr
        {
            public NodeType Kind => NodeType.UnaryExpr;
            public IExpr Arg { get; set; }
            public string Operator { get; set; }
        }

        public interface ILogicalExpr : IExpr
        {
            IExpr Left { get; set; }
            IExpr Right { get; set; }
            string Operator { get; set; }
        }

        public class LogicalExpr : ILogicalExpr
        {
            public NodeType Kind => NodeType.LogicalExpr;
            public IExpr Left { get; set; }
            public IExpr Right { get; set; }
            public string Operator { get; set; }
        }

        public interface IExpr : IStmt { }

        public interface IMemberExpr : IExpr
        {
            IExpr Property { get; set; }
            IExpr Object { get; set; }
            bool Computed { get; set; }
        }
        public class MemberExpr : IMemberExpr
        {
            public NodeType Kind => NodeType.MemberExpr;
            public IExpr Property { get; set; }
            public IExpr Object { get; set; }
            public bool Computed { get; set; }
        }

        public interface ICallExpr : IExpr
        {
            List<IExpr> Args { get; set; }
            IExpr Caller { get; set; }
        }
        public class CallExpr : ICallExpr
        {
            public NodeType Kind => NodeType.CallExpr;
            public List<IExpr> Args { get; set; }
            public IExpr Caller { get; set; }
        }

        public interface IProperty : IExpr
        {
            string Key { get; set; }
            IExpr Value { get; set; }
        }
        public class Property : IProperty
        {
            public NodeType Kind => NodeType.Property;
            public string Key { get; set; }
            public IExpr Value { get; set; }
        }

        public interface IObjectLiteral : IExpr
        {
            List<IProperty> Properties { get; set; }
        }
        public class ObjectLiteral : IObjectLiteral
        {
            public NodeType Kind => NodeType.ObjectLiteral;
            public List<IProperty> Properties { get; set; }
        }

        public interface IIdentifier : IExpr
        {
            string Symbol { get; set; }
        }
        public class Identifier : IIdentifier
        {
            public NodeType Kind => NodeType.Identifier;
            public string Symbol { get; set; }
        }

        public interface IAssignmentExpr : IExpr
        {
            IExpr Assigne { get; set; }
            IExpr Value { get; set; }
        }

        public class AssignmentExpr : IAssignmentExpr
        {
            public NodeType Kind => NodeType.AssignmentExpr;
            public IExpr Assigne { get; set; }
            public IExpr Value { get; set; }
        }

        public interface IIntLiteral : IExpr
        {
            int Value { get; set; }
        }
        public class IntLiteral : IIntLiteral
        {
            public NodeType Kind => NodeType.IntLiteral;
            public int Value { get; set; }
        }

        public interface ILongLiteral : IExpr
        {
            long Value { get; set; }
        }
        public class LongLiteral : ILongLiteral
        {
            public NodeType Kind => NodeType.LongLiteral;
            public long Value { get; set; }
        }


        public interface IStringLiteral : IExpr
        {
            string Value { get; set; }
        }
        public class StringLiteral : IStringLiteral
        {
            public NodeType Kind => NodeType.StringLiteral;
            public string Value { get; set; }
        }
    }
}
