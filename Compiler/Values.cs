using System.Runtime.InteropServices;

namespace Compiler
{

    public enum LineType
    {
        Line,
        Segment,
        Ray,
    }
    public abstract partial class Object
    {
        public abstract Object GetValue(State state);
    }
    public partial class Object : ICloneable
    {
        public Object(bool IsRepresentable)
        {
            isRepresentable = IsRepresentable;
        }

        public Object GetObject()
        {
            return this;
        }





        public virtual int GetCode()
        {
            return 0;
        }

        public bool IsRepresentable()
        {
            return isRepresentable;
        }

        public virtual object Clone()
        {
            return this;
        }

        protected bool isRepresentable;


    }


    public class Color : Object
    {

        public Color(string color) : base(false)
        {
            rbga = colors[color];
        }
        private Dictionary<string, float[]> colors = new()
        {
            {"blue", new float[]{0, 0, 1, 1}},
    {"red", new float[]{1, 0, 0, 1}},
    {"yellow", new float[]{1, 1, 0, 1}},
    {"green", new float[]{0, 1, 0, 1}},
    {"cyan", new float[]{0, 1, 1, 1}},
    {"magenta", new float[]{1, 0, 1, 1}},
    {"white", new float[]{1, 1, 1, 1}},
    {"gray", new float[]{0.5f, 0.5f, 0.5f, 1}},
    {"black", new float[]{0, 0, 0, 1}}
        };
        public float[] rbga;

        public override Object GetValue(State state)
        {
            state.activeColors.Add(this);
            return new Undefined();
        }
    }



    public class Point : Object, ICloneable
    {
        public string Name;
        public double xValue;
        public double yValue;
        public Point(string Name) : base(true)
        {
            this.Name = Name;
            Random random = new();
            xValue = random.Next(300);
            yValue = random.Next(300);
            if (random.Next(2) == 0)
            {
                xValue *= -1;
            }
            if (random.Next(2) == 0)
            {
                yValue *= -1;
            }
        }
        public Point(string Name, Number x, Number y) : base(true)
        {
            this.Name = Name;
            xValue = x.Value;
            yValue = y.Value;
        }

        public object Clone()
        {
            return new Point(Name, new(xValue), new(yValue));
        }

        public override int GetCode()
        {
            return 1;
        }
        public double[] GetPair()
        {
            return new double[] { xValue, yValue };
        }

        public override Object GetValue(State state)
        {
            return this;
        }
    }

    public class FunctionCall : Object
    {
        public FunctionCall() : base(false)
        {

        }
        public string Name { get; set; }
        public List<Node> Arguments { get; set; }









        public override Object GetValue(State state)
        {

            if (BuiltInFunctions.IsBUiltIn(Name))
                return ((UnaryExpressionNode)BuiltInFunctions.GetNodeResult(Arguments.ToArray(), state, Name)).obj;

            FunctionDeclarationNode func = state.CallFunction(Name);

            UnaryExpressionNode result = func.GetValue(Arguments, state).Evaluate(state) as UnaryExpressionNode;
            //if(func.Parameters.Count )
            return result.GetValue(state);

        }
    }

    internal interface ICloneable<T>
    {
    }

    public class Undefined : Object
    {
        public Undefined() : base(false)
        {

        }

        public override Object GetValue(State state)
        {
            return this;
        }
    }


    public class Number : Object
    {

        public static Number operator +(Number n1, Number n2)
        {
            return new(n1.Value + n2.Value);
        }
        public static Number operator -(Number n1, Number n2)
        {
            return new(n1.Value - n2.Value);
        }
        public static Number operator *(Number n1, Number n2)
        {
            return new(n1.Value * n2.Value);
        }
        public static Number operator /(Number n1, Number n2)
        {
            return new(n1.Value / n2.Value);
        }
        public static Number operator %(Number n1, Number n2)
        {
            return new(n1.Value % n2.Value);
        }
        public Number(double number) : base(false)
        {

            Value = number;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public readonly double Value;

        public override Object GetValue(State state)
        {
            return this;
        }
    }

    public class Measure : Object
    {
        public Measure(double Value) : base(false)
        {
            this.Value = Value;
        }

        public static Measure operator +(Measure m1, Measure m2)
        {
            return new Measure(m1.Value + m2.Value);
        }

        public static Measure operator /(Measure m1, Number n)
        {
            return new Measure(m1.Value / n.Value);
        }

        public static Measure operator -(Measure m1, Measure m2)
        {
            return new Measure(Math.Abs(m1.Value - m2.Value));
        }

        public static Measure operator /(Measure m1, Measure m2)
        {
            return new(m1.Value % m2.Value);
        }

        public static Measure operator *(Measure m1, Number n)
        {
            return new(m1.Value * Math.Abs(Math.Floor(n.Value)));
        }
        public static Measure operator *(Number n, Measure m1)
        {
            return new(m1.Value * Math.Abs(Math.Floor(n.Value)));
        }
        public override string ToString()
        {
            return Value.ToString();
        }


        public double Value;

        public override Object GetValue(State state)
        {
            return this;
        }
    }

    public class Segment : Object
    {
        public string name { get; private set; }
        public Point pointA { get; set; }
        public Point pointB { get; set; }
        public Segment(string name) : base(true)
        {
            this.name = name;
            pointA = new Point("");
            pointB = new Point("");
        }
        public Segment(string Name, Point pointA, Point pointB) : base(true)
        {

            name = Name;
            this.pointA = pointA;
            this.pointB = pointB;
        }

        public override int GetCode()
        {
            return 5;
        }

        public override Object GetValue(State state)
        {
            throw new NotImplementedException();
        }
    }
    public class Ray : Object
    {
        public string name { get; private set; }
        public Point pointA { get; set; }
        public Point pointB { get; set; }
        public Ray(string Name) : base(true)
        {
            name = Name;
            pointA = new Point("");
            pointB = new Point("");
        }
        public Ray(string Name, Point pointA, Point pointB) : base(true)
        {
            name = Name;
            this.pointA = pointA;
            this.pointB = pointB;
        }
        public override int GetCode()
        {
            return 10;
        }

        public override Object GetValue(State state)
        {
            throw new NotImplementedException();
        }
    }
    public class Line : Object
    {
        public string name { get; private set; }
        public Point pointA { get; private set; }
        public Point pointB { get; private set; }

        public LineType lineType { get; private set; }
        public Line(string Name, LineType lineType) : base(true)
        {
            this.lineType = lineType;
            name = Name;
            pointA = new Point("");
            pointB = new Point("");
        }
        public Line(string Name, Point pointA, Point pointB, LineType lineType) : base(true)
        {
            this.lineType = lineType;
            name = Name;
            this.pointA = pointA;
            this.pointB = pointB;
        }
        public override int GetCode()
        {
            return 5;
        }

        public override Object GetValue(State state)
        {
            return this;
        }
    }

    public class Circle : Object
    {
        public string Name;
        public Point center;
        public Number radius;
        public Circle(string Name) : base(true)
        {
            this.Name = Name;
            center = new("");
            radius = new(100);
        }
        public Circle(string Name, Point center, Number radius) : base(true)
        {
            this.Name = Name;
            this.center = center;
            this.radius = radius;
        }
        public override int GetCode()
        {
            return 50;
        }

        public override Object GetValue(State state)
        {
            return this;
        }
    }
    public class Arc : Object
    {
        public string Name;
        public Point center;
        public Point p1;
        public Point p2;
        public Number radio;
        public Arc(string Name) : base(true)
        {
            this.Name = Name;
            center = new("");
            p1 = new("");
            p2 = new("");
            radio = new(100);
        }
        public Arc(string Name, Point center, Point p1, Point p2, Number radio) : base(true)
        {
            this.Name = Name;
            this.center = center;
            this.p1 = p1;
            this.p2 = p2;
            this.radio = radio;
        }

        public override int GetCode()
        {
            return 100;
        }

        public override Object GetValue(State state)
        {
            return this;
        }
    }
    public class Sequence : Object
    {


        public List<Node> objects = new();
        private string Type = "none";
        public int Count { get => objects.Count; }
        public bool isUndefined { get; set; }
        private bool isInfinite;
        public Sequence(bool isInfinite) : base(false)
        {
            this.isInfinite = isInfinite;
        }
        public void Add(Object objet)
        {

            if (Type == "none")
            {
                objects.Add(new UnaryExpressionNode(objet));
                Type = objet.GetType().ToString();
            }
            else if (Type != objet.GetType().ToString())
            {
                throw new Exception("Sequence must be of the same type, expected: " + Type + " ,got: " + objet.GetType());
            }

            else objects.Add(new UnaryExpressionNode(objet));
        }

        public override Object GetValue(State state)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                
                    objects[i] = new UnaryExpressionNode((objects[i].Evaluate(state) as UnaryExpressionNode).GetValue(state));
                
            }
            return this;
        }
    }

    public class ConstantCall : Object
    {
        public string Name { get; set; }
        public ConstantCall() : base(false)
        {

        }
        public override object Clone()
        {
            return new ConstantCall() { Name = Name };
        }


        public override Object GetValue(State state)
        {
            ConstantDeclarationNode constD = state.GetConstant(Name);
            UnaryExpressionNode result = ((UnaryExpressionNode)constD.Value.Clone()).Evaluate(state) as UnaryExpressionNode;

            return result.GetValue(state).GetValue(state);
        }
    }

    public class String : Object
    {
        public string content { get; private set; }
        public String(string content) : base(false)
        {
            this.content = content;
        }

        public static String operator +(String str1, String str2)
        {
            return new String(str1.content + str2.content);
        }

        public static String operator +(String str, Number n)
        {
            return new String(str.content + n);
        }



        public override string ToString()
        {
            return content;
        }

        public override Object GetValue(State state)
        {
            return this;
        }
    }

    public static class BuiltInFunctions
    {
        static readonly List<string> builtInFuctions = new()
        {
            "line",
            "segment",
            "ray",
            "circle",
            "arc",
            "print",
            "measure",
            "intersect",
            "point",
            "count",
            "randoms",
            "samples"
        };
        static readonly Dictionary<string, Func<Node[], State, Object>> functions = new()
        {
             {"point", GetPoint},
           {"line", GetLine},
           {"segment",GetSegment},
           {"ray",GetRay},
           {"circle",GetCircle},
           { "arc",GetArc},
           { "print",Print},
           { "measure",Measure},
           { "intersect", Intersect},
           {"count",Count},
           {"randoms",Randoms},
           {"samples",Samples}

        };

        private static Object Samples(Node[] arg1, State state)
        {
            if(arg1.Length != 0)
            {
                throw new Exception("EXPECTED 0 ELEMENT AT FUNCTION SAMPLES");
            }
            Sequence seq = new(false);
            seq.Add(new Point(""));
            seq.Add(new Point(""));
            seq.Add(new Point(""));
            seq.Add(new Point(""));
            seq.Add(new Point(""));
            seq.Add(new Point(""));
            return seq;
        }

        private static Object Randoms(Node[] arg1, State state)
        {
            Random random = new();
            if(arg1.Length != 0)
            {
                throw new Exception("EXPECTED 0 ELEMENT AT FUNCTION RANDOM");
            }
            Sequence seq = new(false);
            seq.Add(new Number(random.Next(2)));
            seq.Add(new Number(random.Next(2)));
            seq.Add(new Number(random.Next(2)));
            seq.Add(new Number(random.Next(2)));
            seq.Add(new Number(random.Next(2)));
            seq.Add(new Number(random.Next(2)));
            return seq;
        }

        private static Object Count(Node[] arg1, State state)
        {
            if(arg1.Length != 1)
            {
                throw new Exception("EXPECTED 1 ELEMENT AT FUNCTION COUNT");
            }
            Object @object = (arg1[0].Evaluate(state) as UnaryExpressionNode).GetValue(state);
            if(@object is Sequence)
            {
                return new Number((@object as Sequence).Count);

            }
            throw new Exception("EXPECTED SEQUENCE AT FUNCTION COUNT");
        }

        public static bool IsBUiltIn(string Name)
        {
            return builtInFuctions.Contains(Name);
        }
       
        public static Node GetNodeResult(Node[] objects, State state, string fname)
        {
            return new UnaryExpressionNode(functions[fname](objects, state));
        }
        
        public static Object GetPoint(Node[] objects, State state)
        {
            
            if (objects.Count() != 2) throw new Exception("EXPECTED 2 PARAMETERS AT LINE FUNCTION");
            Object obj1,obj2;
            if(((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) is Number)
            {
                obj1 =  ((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) as Number;
            }
            else if(((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) is Measure)
            {
                obj1 =  new Number((((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) as Measure).Value);
            }
            else throw new Exception("EXPECTED NUMBER AT point declaration");
            if(((UnaryExpressionNode)objects[1].Evaluate(state)).GetValue(state) is Number)
            {
                obj2 =  ((UnaryExpressionNode)objects[1].Evaluate(state)).GetValue(state) as Number;
            }
            else if(((UnaryExpressionNode)objects[1].Evaluate(state)).GetValue(state) is Measure)
            {
                obj2 =  new Number((((UnaryExpressionNode)objects[1].Evaluate(state)).GetValue(state) as Measure).Value);
            }
            else throw new Exception("EXPECTED NUMBER AT point declaration");
            
            return new Point("", obj1 as Number, obj2 as Number);
        
        }
        public static Object GetLine(Node[] objects, State state)
        {
            if (objects.Count() != 2) throw new Exception("EXPECTED 2 PARAMETERS AT LINE FUNCTION");
            
            return new Line("", ((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)objects[1].Evaluate(state)).GetValue(state) as Point, LineType.Line);
        }
        public static Object GetSegment(Node[] objects, State state)
        {
            if (objects.Count() != 2) throw new Exception("EXPECTED 2 PARAMETERS AT SEGMENT FUNCTION");

            return new Line("", ((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)objects[1].Evaluate(state)).GetValue(state) as Point, LineType.Segment);
        }
        public static Object GetRay(Node[] objects, State state)
        {
            if (objects.Count() != 2) throw new Exception("EXPECTED 2 PARAMETERS AT RAY FUNCTION");

            return new Line("", ((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)objects[1].Evaluate(state)).GetValue(state) as Point, LineType.Ray);
        }

        public static Object GetCircle(Node[] objects, State state)

        {
            if (objects.Count() != 2) throw new Exception("EXPECTED 2 PARAMETERS AT CIRCLE FUNCTION");
            //if (Arguments.Count != 2) throw new Exception("Expected 2 arguments in function circle call");
            Object obj = ((UnaryExpressionNode)objects[1].Evaluate(state)).GetValue(state);
            if(obj is Number)
            return new Circle("", ((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) as Point,  obj as Number);
            else if(obj is Measure)
            return new Circle("", ((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) as Point, new Number(( obj as Measure).Value));
            else throw new Exception("Expected number at circle declaration");

        }
        public static Object GetArc(Node[] objects, State state)
        {
            if (objects.Count() != 4) throw new Exception("Expected 4 arguments in function arc call");
            Object obj = ((UnaryExpressionNode)objects[3].Evaluate(state)).GetValue(state);
            if(obj is Number)
            return new Arc("", ((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)objects[1].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)objects[2].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)objects[3].Evaluate(state)).GetValue(state) as Number);
            else if(obj is Measure)
            return new Arc("", ((UnaryExpressionNode)objects[0].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)objects[1].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)objects[2].Evaluate(state)).GetValue(state) as Point, new Number((obj as Measure).Value));
         else throw new Exception("Expected number at arc declaration");
        }   

        public static Object Print(Node[] objects, State state)
        {
            if (objects.Length != 1) throw new Exception("Expected 1 argument in function print");
            else
            {
                //throw new Exception("amaterasu -->>"+ ((UnaryExpressionNode)objects[0]).GetValue(state)+"<<-- asd");
                Node resultNode = objects[0].Evaluate(state);
                Object objectToPrint = (resultNode as UnaryExpressionNode).GetValue(state).GetValue(state);
                if (objectToPrint is Number || objectToPrint is Measure||objectToPrint is String)
                {
                   
                    //state.toPrint.Add("Se ha agregado: " + objectToPrint.GetType() + " exitosamente");
                    state.toPrint.Add(objectToPrint.ToString());
                }
                else state.toPrint.Add("No se pudo imprimir -->> " + resultNode.GetType());
            }
            return new Undefined();
        }

        public static Object Measure(Node[] objects, State state)
        {
            if (objects.Length != 2) throw new Exception("Expected 2 args in function");
            return Operations.Distance(((UnaryExpressionNode)objects[0]).GetValue(state), ((UnaryExpressionNode)objects[1]).GetValue(state));
        }
        public static Object Intersect(Node[] objects, State state)
        {
            if (objects.Length != 2) throw new Exception("Expected 2 argument in function ");
            return Operations.Intercept(((UnaryExpressionNode)objects[0]).GetValue(state), ((UnaryExpressionNode)objects[1]).GetValue(state));
        }


    }
    public class Continuity : Object
    {
        public Continuity() : base(false)
        {

        }

        public override Object GetValue(State state)
        {
            return this;
        }
    }
}