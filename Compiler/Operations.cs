using System.ComponentModel;
using System.Data;

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
            /*
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
            */
            throw new Exception("BINARY OPERATIONS UNDER CHANGE");
        }
        public static Node UnaryOperation(Node Operand, TokenType Operator)
        {
            /*
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
            }*/
            throw new Exception("UNARY OPERATIONS UNDER CHANGE");
        }
        public static Node Distance(Node pointA, Node pointB)
        {
            /*
            if (pointA.GetType().ToString() != pointB.GetType().ToString() && pointB.GetType().ToString() != "Compiler.PointNode")
                throw new Exception("Expected Type number at measure function and got: " + pointA.GetType().ToString() + " and " + pointA.GetType().ToString());

            double[] p1 = ((PointNode)pointA).GetPair();
            double[] p2 = ((PointNode)pointB).GetPair();

            double dx = p2[0] - p1[0];
            double dy = p2[1] - p1[1];

            return new MeasureNode() { mValue = Math.Sqrt(dx * dx + dy * dy) };
            */
            throw new Exception("DISTANCE OPERATIONS UNDER CHANGE");

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
            Object[] objects = { o1, o2 };
            Arc arc = (Arc)objects.Where(a => a.GetType().ToString() == "Compiler.Arc").First();
            Point point = (Point)objects.Where(a => a.GetType().ToString() == "Compiler.Point").First();
            // Check if the distance equals the radius
            // Allow for a small error margin due to the precision of floating point arithmetic
            double distance = Math.Sqrt(Math.Pow(point.xValue - arc.center.xValue, 2) + Math.Pow(point.yValue - arc.center.yValue, 2));

            Sequence sequence = new(false);
            if (Math.Abs(distance - arc.radio.Value) < 0.000001 && point.IsInArc(arc))
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
        private static Sequence InterceptLines(Object o1, Object o2)
        {
            Line line1 = o1 as Line;
            Line line2 = o2 as Line;
            double a1 = line1.pointB.yValue - line1.pointA.yValue;
            double b1 = line1.pointA.xValue - line1.pointB.xValue;
            double c1 = a1 * line1.pointA.xValue + b1 * line1.pointA.yValue;

            double a2 = line2.pointB.yValue - line2.pointA.yValue;
            double b2 = line2.pointA.xValue - line2.pointB.xValue;
            double c2 = a2 * line2.pointA.xValue + b2 * line2.pointA.yValue;

            double determinant = a1 * b2 - a2 * b1;
            Sequence sequence = new(false);

            if (determinant == 0)
            {
                //Lines are parallel or coincident
                sequence = new(false) { isUndefined = true };
                return sequence;
            }
            else
            {
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;

                Point p = new("") { xValue = x, yValue = y };
                if (p.IsInLine(line1) && p.IsInLine(line2))
                {
                    sequence = new(false);
                    sequence.Add(p);
                    return sequence;
                }

            }
            sequence = new(false) { isUndefined = true };
            return sequence;
        }
        private static Sequence InterceptLineCircle(Object o1, Object o2)
        {
            Object[] objects = { o1, o2 };
            Line line = (Line)objects.Where(a => a.GetType().ToString() == "Compiler.Line").First();
            Circle circle = (Circle)objects.Where(a => a.GetType().ToString() == "Compiler.Circle").First();

            List<Point> intersections = new List<Point>();
            double dx, dy, A, B, C, det, t;

            dx = line.pointB.xValue - line.pointA.xValue;
            dy = line.pointB.yValue - line.pointA.yValue;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (line.pointA.xValue - circle.center.xValue) + dy * (line.pointA.yValue - circle.center.yValue));
            C = (line.pointA.xValue - circle.center.xValue) * (line.pointA.xValue - circle.center.xValue) +
                (line.pointA.yValue - circle.center.yValue) * (line.pointA.yValue - circle.center.yValue) -
                circle.radius.Value * circle.radius.Value;

            det = B * B - 4 * A * C;

            if (A <= 0.0000001 || det < 0)
            {
                // No real solutions.
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                intersections.Add(new Point("") { xValue = line.pointA.xValue + t * dx, yValue = line.pointA.yValue + t * dy });
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersections.Add(new Point("") { xValue = line.pointA.xValue + t * dx, yValue = line.pointA.yValue + t * dy });
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersections.Add(new Point("") { xValue = line.pointA.xValue + t * dx, yValue = line.pointA.yValue + t * dy });
            }
            Sequence sequence = new(false);
            foreach (Point point in intersections)
            {
                if (point.IsInLine(line))
                {
                    sequence.Add(point);
                }
            }
            if (sequence.Count > 0)
            {
                return sequence;
            }
            else return new Sequence(false) { isUndefined = true };
        }
        private static Sequence InterceptLineArc(Object o1, Object o2)
        {
            Object[] objects = { o1, o2 };
            Line line = (Line)objects.Where(a => a.GetType().ToString() == "Compiler.Line").First();
            Arc arc = (Arc)objects.Where(a => a.GetType().ToString() == "Compiler.Arc").First();

            List<Point> intersections = new List<Point>();
            double dx, dy, A, B, C, det, t;

            dx = line.pointB.xValue - line.pointA.xValue;
            dy = line.pointB.yValue - line.pointA.yValue;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (line.pointA.xValue - arc.center.xValue) + dy * (line.pointA.yValue - arc.center.yValue));
            C = (line.pointA.xValue - arc.center.xValue) * (line.pointA.xValue - arc.center.xValue) +
                (line.pointA.yValue - arc.center.yValue) * (line.pointA.yValue - arc.center.yValue) -
                arc.radio.Value * arc.radio.Value;

            det = B * B - 4 * A * C;

            if (A <= 0.0000001 || det < 0)
            {
                // No real solutions.
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                intersections.Add(new Point("") { xValue = line.pointA.xValue + t * dx, yValue = line.pointA.yValue + t * dy });
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersections.Add(new Point("") { xValue = line.pointA.xValue + t * dx, yValue = line.pointA.yValue + t * dy });
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersections.Add(new Point("") { xValue = line.pointA.xValue + t * dx, yValue = line.pointA.yValue + t * dy });
            }
            Sequence sequence = new(false);
            foreach (Point point in intersections)
            {
                if (point.IsInLine(line) && point.IsInArc(arc))
                {
                    sequence.Add(point);
                }
            }
            if (sequence.Count > 0)
            {
                return sequence;
            }
            else return new Sequence(false) { isUndefined = true };
        }
        private static Sequence InterceptCircles(Object o1, Object o2)
        {
            Circle circle1 = o1 as Circle;
            Circle circle2 = o2 as Circle;
            List<Point> intersections = new List<Point>();
            Sequence sequence;

            double d = Math.Sqrt((circle1.center.xValue - circle2.center.xValue) * (circle1.center.xValue - circle2.center.xValue) +
                                  (circle1.center.yValue - circle2.center.yValue) * (circle1.center.yValue - circle2.center.yValue));

            // Check for solvability
            if (d > (circle1.radius.Value + circle2.radius.Value) || d < Math.Abs(circle1.radius.Value - circle2.radius.Value))
            {
                // No solution
                return sequence = new(false) { isUndefined = true };

            }
            else if (d == 0 && circle1.radius.Value == circle2.radius.Value)
            {
                // Circles are coincident
                return sequence = new(false) { isUndefined = true };
            }

            double a = (circle1.radius.Value * circle1.radius.Value - circle2.radius.Value * circle2.radius.Value + d * d) / (2.0 * d);
            double h = Math.Sqrt(circle1.radius.Value * circle1.radius.Value - a * a);

            // Determine the coordinates of point P2 which is the center of the line which connects the intersection points
            double x2 = circle1.center.xValue + a * (circle2.center.xValue - circle1.center.xValue) / d;
            double y2 = circle1.center.yValue + a * (circle2.center.yValue - circle1.center.yValue) / d;

            // Now find the coordinates of the intersection points
            double x3 = x2 + h * (circle2.center.yValue - circle1.center.yValue) / d;
            double y3 = y2 - h * (circle2.center.xValue - circle1.center.xValue) / d;

            double x4 = x2 - h * (circle2.center.yValue - circle1.center.yValue) / d;
            double y4 = y2 + h * (circle2.center.xValue - circle1.center.xValue) / d;

            sequence = new(false);
            sequence.Add(new Point("") { xValue = x3, yValue = y3 });
            sequence.Add(new Point("") { xValue = x4, yValue = y4 });



            return sequence;
        }
        private static Sequence InterceptCircleArc(Object o1, Object o2)
        {
            Object[] objects = { o1, o2 };
            Arc arc = (Arc)objects.Where(a => a.GetType().ToString() == "Compiler.Arc").First();
            Circle circle = (Circle)objects.Where(a => a.GetType().ToString() == "Compiler.Circle").First();
            List<Point> intersections = new List<Point>();
            Sequence sequence;

            double d = Math.Sqrt((arc.center.xValue - circle.center.xValue) * (arc.center.xValue - circle.center.xValue) +
                                  (arc.center.yValue - circle.center.yValue) * (arc.center.yValue - circle.center.yValue));

            // Check for solvability
            if (d > (arc.radio.Value + circle.radius.Value) || d < Math.Abs(arc.radio.Value - circle.radius.Value))
            {
                // No solution
                return sequence = new(false) { isUndefined = true };

            }
            else if (d == 0 && arc.radio.Value == circle.radius.Value)
            {
                // Circles are coincident
                return sequence = new(false) { isUndefined = true };
            }

            double a = (arc.radio.Value * arc.radio.Value - circle.radius.Value * circle.radius.Value + d * d) / (2.0 * d);
            double h = Math.Sqrt(arc.radio.Value * arc.radio.Value - a * a);

            // Determine the coordinates of point P2 which is the center of the line which connects the intersection points
            double x2 = arc.center.xValue + a * (circle.center.xValue - arc.center.xValue) / d;
            double y2 = arc.center.yValue + a * (circle.center.yValue - arc.center.yValue) / d;

            // Now find the coordinates of the intersection points
            double x3 = x2 + h * (circle.center.yValue - arc.center.yValue) / d;
            double y3 = y2 - h * (circle.center.xValue - arc.center.xValue) / d;

            double x4 = x2 - h * (circle.center.yValue - arc.center.yValue) / d;
            double y4 = y2 + h * (circle.center.xValue - arc.center.xValue) / d;

            sequence = new(false);
            Point p1 = new("") { xValue = x3, yValue = y3 };
            Point p2 = new("") { xValue = x4, yValue = y4 };
            if (p1.IsInArc(arc)) sequence.Add(p1);
            if (p2.IsInArc(arc)) sequence.Add(p2);
            if (sequence.Count > 0)
            {
                return sequence;
            }
            else return new Sequence(false) { isUndefined = true };
        }
        private static Sequence InterceptArcs(Object o1, Object o2)
        {

            Arc arc1 = o1 as Arc;
            Arc arc2 = o2 as Arc;
            List<Point> intersections = new List<Point>();
            Sequence sequence;

            double d = Math.Sqrt((arc1.center.xValue - arc2.center.xValue) * (arc1.center.xValue - arc2.center.xValue) +
                                  (arc1.center.yValue - arc2.center.yValue) * (arc1.center.yValue - arc2.center.yValue));

            // Check for solvability
            if (d > (arc1.radio.Value + arc2.radio.Value) || d < Math.Abs(arc1.radio.Value - arc2.radio.Value))
            {
                // No solution
                return sequence = new(false) { isUndefined = true };

            }
            else if (d == 0 && arc1.radio.Value == arc2.radio.Value)
            {
                // Circles are coincident
                return sequence = new(false) { isUndefined = true };
            }

            double a = (arc1.radio.Value * arc1.radio.Value - arc2.radio.Value * arc2.radio.Value + d * d) / (2.0 * d);
            double h = Math.Sqrt(arc1.radio.Value * arc1.radio.Value - a * a);

            // Determine the coordinates of point P2 which is the center of the line which connects the intersection points
            double x2 = arc1.center.xValue + a * (arc2.center.xValue - arc1.center.xValue) / d;
            double y2 = arc1.center.yValue + a * (arc2.center.yValue - arc1.center.yValue) / d;

            // Now find the coordinates of the intersection points
            double x3 = x2 + h * (arc2.center.yValue - arc1.center.yValue) / d;
            double y3 = y2 - h * (arc2.center.xValue - arc1.center.xValue) / d;

            double x4 = x2 - h * (arc2.center.yValue - arc1.center.yValue) / d;
            double y4 = y2 + h * (arc2.center.xValue - arc1.center.xValue) / d;

            sequence = new(false);
            Point p1 = new("") { xValue = x3, yValue = y3 };
            Point p2 = new("") { xValue = x4, yValue = y4 };
            if (p1.IsInArc(arc1) && p1.IsInArc(arc2)) sequence.Add(p1);
            if (p2.IsInArc(arc1) && p2.IsInArc(arc2)) sequence.Add(p2);
            if (sequence.Count > 0)
            {
                return sequence;
            }
            else return new Sequence(false) { isUndefined = true };
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
        public static bool IsInArc(this Point inPoint, Arc arc)
        {Point inputPoint = inPoint.Clone() as Point;
    Point arcCenter = arc.center.Clone() as Point;
    Point arcP1 = arc.p1.Clone() as Point;
    Point arcP2 = arc.p2.Clone() as Point;

    // Center the points at the origin
    inputPoint.xValue -= arcCenter.xValue;
    inputPoint.yValue -= arcCenter.yValue;
    arcP1.xValue -= arcCenter.xValue;
    arcP1.yValue -= arcCenter.yValue;
    arcP2.xValue -= arcCenter.xValue;
    arcP2.yValue -= arcCenter.yValue;

    // Calculate the distances
    double distanceInputPoint = Math.Sqrt(Math.Pow(inputPoint.xValue, 2) + Math.Pow(inputPoint.yValue, 2));
    double distanceP1 = Math.Sqrt(Math.Pow(arcP1.xValue, 2) + Math.Pow(arcP1.yValue, 2));

    if (Math.Abs(distanceInputPoint - distanceP1) > 1e-10)
    {
        // The point is not on the circle defined by the arc
        return false;
    }

    // Calculate the angles
    double angleInputPoint = Math.Atan2(inputPoint.yValue,inputPoint.xValue);
    double angleP1 = Math.Atan2(arcP1.yValue,arcP1.xValue);
    double angleP2 = Math.Atan2(arcP2.yValue,arcP2.xValue);

    // Normalize the angles to [0, 2π]
    if (angleInputPoint < 0) angleInputPoint += 2 * Math.PI;
    if (angleP1 < 0) angleP1 += 2 * Math.PI;
    if (angleP2 < 0) angleP2 += 2 * Math.PI;

    // Check if the point's angle is within the arc's angles
    if (angleP1 > angleP2)
    {
        return angleP1 <= angleInputPoint || angleInputPoint <= angleP2;
    }
    else
    {
        return angleP1 <= angleInputPoint && angleInputPoint <= angleP2;
    }
        }

        public static double RotateAngle(double inputAngle, double rotateRadians)
        {
            
            double angleResult = inputAngle + rotateRadians;

            if(angleResult > 2 * Math.PI)
            {
                return angleResult - 2 * Math.PI;
            }
            else return angleResult;
            
            
        }

    }


}