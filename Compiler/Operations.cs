using System.Data;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.VisualBasic.CompilerServices;
namespace Compiler
{
    public static class Operations
    {
        private static Dictionary<int, Func<Object, Object, Sequence>> interceptOperations = new()
        {
            {1,InterceptPoints},
            {5,InterceptPointLine},
            {25,InterceptLines},
            {50,InterceptPointCircle},
            {100,InterceptPointArc},
            {250,InterceptLineCircle},
            {500,InterceptLineArc},
            {2500,InterceptCircles},
            {5000,InterceptCircleArc},
            {10000,InterceptArcs}
        };
        public static Node BinaryOperation(Node left, Node right, TokenType Operator)
        {
            switch (Operator)
            {
                case TokenType.Plus:
                    if (left.GetType().ToString() == "Compiler.NumberNode")
                    {
                        if (right.GetType().ToString() == "Compiler.NumberNode")
                        {
                            return new NumberNode() { Value = ((NumberNode)left).Value + ((NumberNode)right).Value };
                        }
                        throw new Exception("Expected number in + operation and got: " + left.GetType());
                    }
                    else throw new Exception("Expected number in + operation and got: " + right.GetType());
                case TokenType.Minus:
                    if (left.GetType().ToString() == "Compiler.NumberNode")
                    {
                        if (right.GetType().ToString() == "Compiler.NumberNode")
                        {
                            return new NumberNode() { Value = ((NumberNode)left).Value - ((NumberNode)right).Value };
                        }
                        throw new Exception("Expected number in + operation and got: " + left.GetType());
                    }

                    else throw new Exception("Expected number in + operation and got: " + right.GetType());
                case TokenType.GreaterThan:
                    if (left.GetType().ToString() == "Compiler.NumberNode")
                    {
                        if (right.GetType().ToString() == "Compiler.NumberNode")
                        {
                            if (((NumberNode)left).Value > ((NumberNode)right).Value)
                                return new NumberNode() { Value = 1 };
                            else return new NumberNode() { Value = 0 };
                        }
                        throw new Exception("Expected number in > operation and got: " + left.GetType());
                    }

                    else throw new Exception("Expected number in > operation and got: " + right.GetType());
                case TokenType.LessThan:
                    if (left.GetType().ToString() == "Compiler.NumberNode")
                    {
                        if (right.GetType().ToString() == "Compiler.NumberNode")
                        {
                            if (((NumberNode)left).Value < ((NumberNode)right).Value)
                                return new NumberNode() { Value = 1 };
                            else return new NumberNode() { Value = 0 };
                        }
                        throw new Exception("Expected number in < operation and got: " + left.GetType());
                    }

                    else throw new Exception("Expected number in < operation and got: " + right.GetType());
                case TokenType.GreaterThanEqual:
                    if (left.GetType().ToString() == "Compiler.NumberNode")
                    {
                        if (right.GetType().ToString() == "Compiler.NumberNode")
                        {
                            if (((NumberNode)left).Value >= ((NumberNode)right).Value)
                                return new NumberNode() { Value = 1 };
                            else return new NumberNode() { Value = 0 };
                        }
                        throw new Exception("Expected number in >= operation and got: " + left.GetType());
                    }

                    else throw new Exception("Expected number in >= operation and got: " + right.GetType());
                case TokenType.LessThanEqual:
                    if (left.GetType().ToString() == "Compiler.NumberNode")
                    {
                        if (right.GetType().ToString() == "Compiler.NumberNode")
                        {
                            if (((NumberNode)left).Value <= ((NumberNode)right).Value)
                                return new NumberNode() { Value = 1 };
                            else return new NumberNode() { Value = 0 };
                        }
                        throw new Exception("Expected number in <= operation and got: " + left.GetType());
                    }

                    else throw new Exception("Expected number in <= operation and got: " + right.GetType());
                case TokenType.EqualEqual:
                    if (left.GetType().ToString() == "Compiler.NumberNode")
                    {
                        if (right.GetType().ToString() == "Compiler.NumberNode")
                        {
                            if (((NumberNode)left).Value == ((NumberNode)right).Value)
                                return new NumberNode() { Value = 1 };
                            else return new NumberNode() { Value = 0 };
                        }
                        throw new Exception("Expected number in == operation and got: " + left.GetType());
                    }

                    else throw new Exception("Expected number in == operation and got: " + right.GetType());
                case TokenType.Divide:
                    if (left.GetType().ToString() == "Compiler.NumberNode")
                    {
                        if (right.GetType().ToString() == "Compiler.NumberNode")
                        {
                            return new NumberNode() { Value = ((NumberNode)left).Value / ((NumberNode)right).Value };
                        }
                        throw new Exception("Expected number in / operation and got: " + left.GetType());
                    }
                    else throw new Exception("Expected number in / operation and got: " + right.GetType());
                case TokenType.Multiply:
                    if (left.GetType().ToString() == "Compiler.NumberNode")
                    {
                        if (right.GetType().ToString() == "Compiler.NumberNode")
                        {
                            return new NumberNode() { Value = ((NumberNode)left).Value * ((NumberNode)right).Value };
                        }
                        throw new Exception("Expected number in * operation and got: " + left.GetType());
                    }
                    else throw new Exception("Expected number in * operation and got: " + right.GetType());
                case TokenType.DivideRest:

                    if (left.GetType().ToString() == "Compiler.NumberNode")
                    {
                        if (right.GetType().ToString() == "Compiler.NumberNode")
                        {
                            return new NumberNode() { Value = ((NumberNode)left).Value % ((NumberNode)right).Value };
                        }
                        throw new Exception("Expected number in % operation and got: " + left.GetType());
                    }
                    else throw new Exception("Expected number in % operation and got: " + right.GetType());
                default:
                    throw new Exception("Unexpected Operator :" + Operator);

            }
        }
        public static Node UnaryOperation(Node Operand, TokenType Operator)
        {
            switch (Operator)
            {
                case TokenType.Minus:
                    if (Operand.GetType().ToString() == "Compiler.NumberNode")
                    {
                        return new NumberNode() { Value = ((NumberNode)Operand).Value * -1 };
                    }
                    else throw new Exception("Expected number at - operator and got: " + Operand.GetType());

                default:
                    throw new Exception("Unexpected Operator at Unary Operation: " + Operand.GetType());
            }
        }
        public static Node Distance(Node pointA, Node pointB)
        {
            if (pointA.GetType().ToString() != pointB.GetType().ToString() && pointB.GetType().ToString() != "Compiler.PointNode")
                throw new Exception("Expected Type number at measure function and got: " + pointA.GetType().ToString() + " and " + pointA.GetType().ToString());

            double[] p1 = ((PointNode)pointA).GetPair();
            double[] p2 = ((PointNode)pointB).GetPair();

            double dx = p2[0] - p1[0];
            double dy = p2[1] - p1[1];

            return new MeasureNode() { mValue = Math.Sqrt(dx * dx + dy * dy) };

        }

        public static Sequence Intercept(Object obj1, Object obj2)
        {
            Func<Object, Object, Sequence> GetIntercept = GetInterceptOperation(obj1, obj2);
            return GetIntercept(obj1, obj2);
        }

        private static Func<Object, Object, Sequence> GetInterceptOperation(Object obj1, Object obj2)
        {
            if (interceptOperations.ContainsKey(obj1.GetCode() * obj2.GetCode()))
            {
                return interceptOperations[obj1.GetCode() * obj2.GetCode()];
            }
            else throw new Exception("Invalid Intercept Operations between: " + obj1.GetType() + " and " + obj2.GetType());
        }

        private static Sequence InterceptPoints(Object o1, Object o2)
        {
            Point p1 = (Point)o1;
            Point p2 = (Point)o2;
            Sequence sequence;
            if (p1.xValue == p2.xValue && p1.yValue == p2.yValue)
            {
                sequence = new(false);
                sequence.Add(p1);
                return sequence;
            }
            else
            {
                return sequence = new(false) { isUndefined = true };
            }
        }
        private static Sequence InterceptPointLine(Object o1, Object o2)
        {
            Object[] objects = { o1, o2 };
            Line line = (Line)objects.Where(a => a.GetType().ToString() == "Compiler.Line").First();
            Point point = (Point)objects.Where(a => a.GetType().ToString() == "Compiler.Point").First();


            double slope = (line.pointB.yValue - line.pointA.yValue) / (line.pointB.xValue - line.pointA.xValue);


            double yIntercept = line.pointA.yValue - (slope * line.pointA.xValue);

            Sequence sequence = new(false);
            if (point.yValue == slope * point.xValue + yIntercept)
            {
                sequence = new(false);
                sequence.Add(point);
                return sequence;
            }
            else
            {
                return sequence = new(false) { isUndefined = true };
            }
        }
        private static Sequence InterceptPointCircle(Object o1, Object o2)
        {
            Object[] objects = { o1, o2 };
            Circle circle = (Circle)objects.Where(a => a.GetType().ToString() == "Compiler.Circle").First();
            Point point = (Point)objects.Where(a => a.GetType().ToString() == "Compiler.Point").First();
            // Calculate the distance between the point and the center of the circle
            double distance = Math.Sqrt(Math.Pow(point.xValue - circle.center.xValue, 2) + Math.Pow(point.yValue - circle.center.yValue, 2));
            Sequence sequence = new(false);
            // Check if the distance equals the radius
            // Allow for a small error margin due to the precision of floating point arithmetic
            if (Math.Abs(distance - circle.radius.Value) < 0.000001)
            {
                sequence = new(false);
                sequence.Add(point);
                return sequence;
            }
            else
            {
                return sequence = new(false) { isUndefined = true };
            }
        }
        private static Sequence InterceptPointArc(Object o1, Object o2)
        {

        }
        private static Sequence InterceptLines(Object o1, Object o2)
        {

        }
        private static Sequence InterceptLineCircle(Object o1, Object o2)
        {

        }
        private static Sequence InterceptLineArc(Object o1, Object o2)
        {

        }
        private static Sequence InterceptCircles(Object o1, Object o2)
        {

        }
        private static Sequence InterceptCircleArc(Object o1, Object o2)
        {

        }
        private static Sequence InterceptArcs(Object o1, Object o2)
        {

        }

        /// <summary>
        /// Verifica si el punto dado está contenido en la recta, se asume que el punto pertenece a la recta soporte de la línea
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool IsInLine(this Point point, Line line)
        {
            if (line.lineType is LineType.Ray)
            {
                double direction = line.pointB.xValue - line.pointA.xValue;
                if (direction > 0)
                {
                    return point.xValue >= line.pointA.xValue;
                }
                else if (direction < 0)
                {
                    return point.xValue <= line.pointA.xValue;
                }
                else
                {
                    direction = line.pointB.yValue - line.pointA.yValue;
                    if (direction > 0)
                    {
                        return point.yValue >= line.pointA.yValue;
                    }
                    else if (direction < 0)
                    {
                        return point.yValue <= line.pointA.yValue;
                    }
                    else return false;
                }
            }
            else if (line.lineType is LineType.Segment)
            {
                double direction = line.pointB.xValue - line.pointA.xValue;
                if (direction > 0)
                {
                    return point.xValue >= line.pointA.xValue && point.xValue <= line.pointB.xValue;
                }
                else if (direction < 0)
                {
                    return point.xValue <= line.pointA.xValue && point.xValue >= line.pointB.xValue;
                }
                else
                {
                    direction = line.pointB.yValue - line.pointA.yValue;
                    if (direction > 0)
                    {
                        return point.yValue >= line.pointA.yValue && point.yValue <= line.pointB.yValue;
                    }
                    else if (direction < 0)
                    {
                        return point.yValue <= line.pointA.yValue && point.yValue >= line.pointB.yValue;
                    }
                    else return false;
                }
            }

            else return true;
        }
        public static bool IsInArc(this Point inputPoint, Arc arc)
        {
            Point point = new("")
            {
                xValue = inputPoint.xValue - arc.center.xValue,
                yValue = inputPoint.yValue - arc.center.yValue,
            };

            Point p1 = new("")
            {
                xValue = arc.p1.xValue - arc.center.xValue,
                yValue = arc.p1.yValue - arc.center.yValue,
            };
            Point p2 = new("")
            {
                xValue = arc.p2.xValue - arc.center.xValue,
                yValue = arc.p2.yValue - arc.center.yValue,
            };

            // Define vectors relative to p1 and p2
            Point a = new Point(""){ xValue = point.xValue - p1.xValue, yValue = point.yValue - p1.yValue };
            Point b = new Point("") { xValue = p2.xValue - p1.xValue, yValue = p2.yValue - p1.yValue };
            Point c = new Point("") { xValue = point.xValue - p2.xValue, yValue = point.yValue - p2.yValue };
            Point d = new Point("") { xValue = p1.xValue - p2.xValue, yValue = p1.yValue - p2.yValue };

            // Calculate cross products
            double crossProduct1 = a.xValue * b.yValue - b.xValue * a.yValue;
            double crossProduct2 = c.xValue * d.yValue - d.xValue * c.yValue;

            // Calculate dot products
            double dotProduct1 = a.xValue * b.xValue + a.yValue * b.yValue;
            double dotProduct2 = c.xValue * d.xValue + c.yValue * d.yValue;

            // Point is within arc if the sign of the cross products are the same
            // and the dot product is greater than 0
            return (crossProduct1 * crossProduct2 >= 0) && dotProduct1 >= 0 && dotProduct2 >= 0;
        }

    }


}