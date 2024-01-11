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
    public partial class Object
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
        protected bool isRepresentable;


    }

    public class Color : Object
    {

       public Color() : base(false)
       {

       }
        private Dictionary<string,float[]> colors = new()
        {
             //NOT IMPLEMENTED
        };
        float[] rbga;

        public override Object GetValue(State state)
        {
            return this;
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
            return new Point(Name,new(xValue),new(yValue));
        }

        public override  int GetCode()
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
        public Number(double number) : base(false)
        {

            Value = number;
        }

        public readonly double Value;

        public override Object GetValue(State state)
        {
            return this;
        }
    }

    public class Measure : Object
    {
        public Measure(Token token) : base(false)
        {
            if (token.Type is TokenType.Number)
            {
                Value = double.Parse(token.Value);
            }
            else throw new Exception("Invalid type at Number construction, expected Number, got: " + token.Type);
        }
        private double Value;

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

        public LineType lineType {get; private set;}
        public Line(string Name, LineType lineType) : base(true)
        {
            this.lineType = lineType;
            name = Name;
            pointA = new Point("");
            pointB = new Point("");
        }
        public Line(string Name, Point pointA, Point pointB,LineType lineType) : base(true)
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
        public int Count {get => objects.Count;}
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
                objects.Add(new UnaryExpressionNode( objet));
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
            return this;
        }
    }

    public class ConstantCall : Object
    {
        public string Name {get; set;}
        public ConstantCall() : base(false)
        {
            
        }

        

        public override Object GetValue(State state)
        {
             ConstantDeclarationNode constD = state.GetConstant(Name);
            UnaryExpressionNode result = (UnaryExpressionNode)constD.Value.Clone();
            return result.GetValue(state);
        }
    }

    public class String : Object
    {
        public string content {get; private set;}
        public String(string content) : base(false)
        {
           this.content = content;
        }

        public override Object GetValue(State state)
        {
            return this;
        }
    }
}