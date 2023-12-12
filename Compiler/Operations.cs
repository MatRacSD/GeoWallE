using System.Data;
using System.Security.Cryptography;
using Microsoft.VisualBasic.CompilerServices;
namespace Compiler
{
    public static class Operations
    {
        public static Node BinaryOperation(Node left, Node right,TokenType Operator)
        {
           switch( Operator)
           {
              case TokenType.Plus:
              if(left.GetType().ToString() == "Compiler.NumberNode")
              {
                if(right.GetType().ToString() == "Compiler.NumberNode")
                {
                    return new NumberNode(){Value = ((NumberNode)left).Value + ((NumberNode)right).Value};
                }
                throw new Exception("Expected number in + operation and got: " + left.GetType());
              }
              else throw new Exception("Expected number in + operation and got: " + right.GetType());
              case TokenType.Minus:
              if(left.GetType().ToString() == "Compiler.NumberNode")
              {
                if(right.GetType().ToString() == "Compiler.NumberNode")
                {
                    return new NumberNode(){Value = ((NumberNode)left).Value - ((NumberNode)right).Value};
                }
                throw new Exception("Expected number in + operation and got: " + left.GetType());
              }

              else throw new Exception("Expected number in + operation and got: " + right.GetType());
              default:
              throw new Exception("Unexpected Operator :" + Operator);
           }
        }
        public static Node UnaryOperation(Node Operand,TokenType Operator)
        {
            switch (Operator)
            {
                case TokenType.Minus:
                if(Operand.GetType().ToString() == "Compiler.NumberNode")
                {
                           return new NumberNode(){Value = ((NumberNode)Operand).Value * -1};
                }
                else throw new Exception("Expected number at - operator and got: " +Operand.GetType());
                
                default:
                throw new Exception("Unexpected Operator at Unary Operation: " + Operand.GetType());
            }
        }
        public static Node Distance(Node pointA, Node pointB)
    {
        if(pointA.GetType().ToString() != pointB.GetType().ToString() && pointB.GetType().ToString() != "Compiler.PointNode")
        throw new Exception("Expected Type number at measure function and got: " + pointA.GetType().ToString() + " and "+pointA.GetType().ToString() );

    double[] p1 = ((PointNode)pointA).GetPair();
    double[] p2 = ((PointNode)pointB).GetPair();

    double dx = p2[0] - p1[0];
    double dy = p2[1] - p1[1];

    return  new MeasureNode(){mValue=Math.Sqrt(dx * dx + dy * dy)};

    }
    }

    
}