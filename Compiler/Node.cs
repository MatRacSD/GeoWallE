

namespace Compiler
{
    /// <summary>
    /// Clase principal del nodo
    /// </summary>
    public abstract class Node : ICloneable
    {
        public abstract object Clone();

        public abstract Node Evaluate(State state);

    }
    /// <summary>
    /// Nodo que contiene el archivo a importar
    /// </summary>

    public class ImportNode : Node
    {
        public string Path { get; set; }

        public override object Clone()
        {
            return new ImportNode() { Path = Path };
        }

        public override Node Evaluate(State state)
        {
            if (!state.imported.Contains(Path))
            {
                state.imported.Add(Path);
                state.Run(ReadFile(Path));
            }


            return new NullNode();
        }
        public string ReadFile(string filename)
        {
            if (File.Exists(filename))
            {
                return File.ReadAllText(filename);
            }
            else
            {
                throw new FileNotFoundException("File not found: " + filename);
            }
        }
    }
    /// <summary>
    /// Nodo que contiene una declaracion de secuencia
    /// </summary>
    public class SequencDeclarationNode : Node
    {
        public List<Token> constants = new();
        public Node body { get; set; }

        public override object Clone()
        {
            return new SequencDeclarationNode() { constants = constants, body = (Node)body.Clone() };
        }

        public override Node Evaluate(State state)
        {
            if (body.Evaluate(state).GetType().ToString() != "Compiler.SequenceNode")
            {
                throw new Exception("Expected Sequence at sequence constants declarations");
            }
            else
            {

                Sequence seq = (Sequence)((UnaryExpressionNode)body.Evaluate(state)).GetValue(state);

                for (int i = 0; i < constants.Count; i++)
                {
                    if (constants[i].Type == TokenType.GuionBajo)
                    {
                        continue;
                    }
                    else
                    {
                        if (i >= seq.Count)
                            state.AddConstant(new() { Name = constants[i].Value, Value = new UndefinedNode() });
                        else state.AddConstant(new() { Name = constants[i].Value, Value = seq.objects[i] });
                    }
                }
            }
            return new NullNode();
        }
    }
    /// <summary>
    /// Nodo que contiene una declaracion de punto
    /// </summary>
    public class PointDeclarationNode : Node
    {
        private PointDeclarationNode()
        {

        }
        public Point point { get; set; }

        public PointDeclarationNode(string pName)
        {
            point = new Point(pName);
        }
        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = point.Name, Value = new UnaryExpressionNode(point) });
            return new UnaryExpressionNode(point);
        }

        public override object Clone()
        {
            return new PointDeclarationNode() { point = (Point)point };
        }
    }
    /*
    public class PointNode : Node
    {
        private PointNode()
        {

        }
        public string pointName;
        public double xValue;
        public double yValue;

        public PointNode(string pointName)
        {
            this.pointName = pointName;
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
        public double[] GetPair()
        {
            return new double[] { xValue, yValue };
        }
        public override Node Evaluate(State state)
        {
            return this;
        }

        public override object Clone()
        {
            return new PointNode() { pointName = pointName, xValue = xValue, yValue = yValue };
        }
    }
    */
    /// <summary>
    /// Nodo que contiene el color///NO IMPLEMENTADO
    /// </summary>

    /*
        public class ColorNode : Node
        {
            public string Id { get; set; }

            public override object Clone()
            {
                return new ColorNode() { Id = Id };
            }

            public override Node Evaluate(State state)
            {
                throw new NotImplementedException();
            }
        }
        */
    /// <summary>
    /// Nodo que contiene el valor de una medida
    /// </summary>
    /*
    public class MeasureNode : Node
    {
        public double mValue;

        public override object Clone()
        {
            return new MeasureNode() { mValue = mValue };
        }

        public override Node Evaluate(State state)
        {
            return new NumberNode() { Value = mValue };
        }
    }
    */
    /// <summary>
    /// Nodo que contiene objeto a pintar
    /// </summary>
    public class DrawNode : Node
    {
        public DrawNode()
        {

        }
        public Node figToDraw { get; set; }
        public string label { get; set; }

        public override object Clone()
        {
            return new DrawNode() { figToDraw = figToDraw, label = label };
        }

        public override Node Evaluate(State state)
        {
            Object @object = ((UnaryExpressionNode)figToDraw.Evaluate(state)).GetValue(state);
            if (@object is Sequence)
            {
                Sequence seq = @object as Sequence;
                for (int i = 0; i < seq.Count; i++)
                {
                    state.AddToDraw(((UnaryExpressionNode)seq.objects[i].Evaluate(state)).obj.GetValue(state));
                }
                return new NullNode();
            }

            /*
            if (state.GetConstant(figToDraw).Value.Evaluate(state).GetType().ToString() == "Compiler.SequenceNode")
            {
                Sequence sequenceNode = (Sequence)state.GetConstant(figToDraw).Value.Evaluate(state);
                if (((Sequence)sequenceNode.Evaluate(state)).Type != "undefined")
                {
                    for (int i = 0; i < sequenceNode.count.Value; i++)
                    {
                        state.AddToDraw(sequenceNode.nodes[i].Evaluate(state));
                    }
                }
            } */
            state.AddToDraw(@object.GetValue(state));

            return new NullNode();
        }
    }
    /// <summary>
    /// Nodo que contiene la declaracion de una funcion
    /// </summary>
    public class FunctionDeclarationNode : Node
    {
        public string Name { get; set; }
        public List<string> Parameters { get; set; }
        public Node Body { get; set; }

        public override object Clone()
        {
            return new FunctionDeclarationNode() { Name = Name, Parameters = Parameters, Body = (Node)Body.Clone() };
        }

        public override Node Evaluate(State state)
        {
            state.AddFunction(this);
            return new NullNode();
        }
        public Node GetValue(List<Node> args, State state)
        {
            if (args.Count != Parameters.Count)
            {
                throw new Exception("Expected " + Parameters.Count + "arguments and got " + args.Count);
            }
            State funcScope = (State)state.Clone();
            for (int i = 0; i < args.Count; i++)
            {
                funcScope.ForceAddConstant(new ConstantDeclarationNode() { Name = Parameters[i], Value = ((Node)args[i].Clone()).Evaluate(funcScope) });
            }
            Node result = ((Node)Body.Clone()).Evaluate(funcScope);
            /*for (int i = 0; i < Parameters.Count; i++)
            {
                state.RemoveConstant(Parameters[i]);
            }*/
            return result;
        }

    }
    /// <summary>
    /// Nodo que contiene una llamada a una funcion
    /// </summary>
    public class FunctionCallNode : Node
    {
        public string Name { get; set; }
        public List<Node> Arguments { get; set; }

        public override object Clone()
        {
            List<Node> nodesTemp = new();
            foreach (var item in Arguments)
            {
                nodesTemp.Add((Node)item.Clone());
            }
            return new FunctionCallNode() { Name = Name, Arguments = nodesTemp };
        }

        public override Node Evaluate(State state)
        {
            switch (Name)
            {
                case "line":
                    if (Arguments.Count != 2) throw new Exception("Expected 2 arguments in function line call");
                    return new UnaryExpressionNode(new Line("", ((UnaryExpressionNode)Arguments[0].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)Arguments[1].Evaluate(state)).GetValue(state) as Point, LineType.Line));
                case "segment":
                    if (Arguments.Count != 2) throw new Exception("Expected 2 arguments in function line call");
                    return new UnaryExpressionNode(new Line("", ((UnaryExpressionNode)Arguments[0].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)Arguments[1].Evaluate(state)).GetValue(state) as Point, LineType.Segment));
                case "ray":
                    if (Arguments.Count != 2) throw new Exception("Expected 2 arguments in function line call");
                    return new UnaryExpressionNode(new Line("", ((UnaryExpressionNode)Arguments[0].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)Arguments[1].Evaluate(state)).GetValue(state) as Point, LineType.Ray));
                case "circle":
                    if (Arguments.Count != 2) throw new Exception("Expected 2 arguments in function circle call");
                    return new UnaryExpressionNode(new Circle("", ((UnaryExpressionNode)Arguments[0].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)Arguments[1].Evaluate(state)).GetValue(state) as Number));
                case "arc":
                    if (Arguments.Count != 4) throw new Exception("Expected 4 arguments in function arc call");
                    return new UnaryExpressionNode(new Arc("", ((UnaryExpressionNode)Arguments[0].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)Arguments[1].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)Arguments[2].Evaluate(state)).GetValue(state) as Point, ((UnaryExpressionNode)Arguments[3].Evaluate(state)).GetValue(state) as Number));
                case "print":
                    if (Arguments.Count != 1) throw new Exception("Expected 1 argument in function print");
                    {
                        //throw new Exception("amaterasu -->>"+Arguments[0].Evaluate(state)+"<<-- asd");
                        Node resultNode = Arguments[0].Evaluate(state);
                        if (resultNode.GetType().ToString() == "Compiler.NumberNode")
                        {
                            throw new Exception("PRINT NOT IMPLEMENTED");
                            //.toPrint.Add(((NumberNode)resultNode).Value.ToString());
                        }
                        else state.toPrint.Add("No se pudo imprimir -->> " + resultNode.GetType());
                    }
                    return new NullNode();
                case "measure":
                    if (Arguments.Count != 2) throw new Exception("Expected 2 argument in function ");
                    return Operations.Distance(Arguments[0].Evaluate(state), Arguments[1].Evaluate(state));

                case "intersect":
                if (Arguments.Count != 2) throw new Exception("Expected 2 argument in function ");
                return new UnaryExpressionNode(Operations.Intercept(((UnaryExpressionNode)Arguments[0]).GetValue(state),((UnaryExpressionNode)Arguments[1]).GetValue(state)));

                default:
                    FunctionDeclarationNode func = state.CallFunction(Name);
                    Node result = func.GetValue(Arguments, state).Evaluate(state);
                    //if(func.Parameters.Count )
                    return result;
            }
        }
    }
    /// <summary>
    /// Nodo que contiene una secuencia
    /// </summary>
    /*
    public class SequenceNode : Node
    {
        public List<Node> nodes = new();
        public string Type = "undefined";
        public NumberNode count { get => new NumberNode() { Value = nodes.Count }; }

        public void Add(Node node, State state)
        {

            nodes.Add(node);



        }

        public override object Clone()
        {
            List<Node> nodesTemp = new();
            foreach (var item in nodes)
            {
                nodesTemp.Add((Node)item.Clone());
            }
            return new SequenceNode() { nodes = nodesTemp };
        }

        public override Node Evaluate(State state)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i == 0)
                {
                    Type = nodes[0].Evaluate(state).GetType().ToString();
                }
                else
                {
                    if (nodes[i].Evaluate(state).GetType().ToString() != Type)
                        throw new Exception("A sequence must be of the same type, Expected: " + Type + "--Got: " + nodes[i].Evaluate(state).GetType());
                }
            }
            return this;
        }
    }
    */
    /// <summary>
    /// Nodo que contiene una declaracion de constante
    /// </summary>
    public class ConstantDeclarationNode : Node
    {
        public string Name { get; set; }
        public Node Value { get; set; }

        public override object Clone()
        {
            return new ConstantDeclarationNode() { Name = Name, Value = (Node)Value.Clone() };
        }

        public override Node Evaluate(State state)
        {
            Value = Value.Evaluate(state);
            state.AddConstant(this);
            return new NullNode();
        }
    }
    /// <summary>
    /// Nodo que contiene una llamada a una constante
    /// </summary>
    /*
    public class ConstantCallNode : Node
    {
        public string Name { get; set; }

        public override object Clone()
        {
            return new ConstantCallNode() { Name = Name };
        }

        public override Node Evaluate(State state)
        {
            ConstantDeclarationNode constD = state.GetConstant(Name);
            Node result = (Node)constD.Value.Clone();
            return (Node)result.Evaluate(state).Clone();
        }

    }
    */

    /// <summary>
    /// Nodo que contiene un numero
    /// </summary>
    /*
    public class NumberNode : Node
    {
        public double Value { get; set; }

        public override object Clone()
        {
            return new NumberNode() { Value = Value };
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
    */
    /// <summary>
    /// Nodo que contiene un segmento
    /// </summary>
/*
    public class SegmentNode : Node

    {
        public string lineName { get; private set; }
        public PointNode pointA { get; set; }
        public PointNode pointB { get; set; }
        public SegmentNode(string Name)
        {
            lineName = Name;
            pointA = new PointNode("");
            pointB = new PointNode("");
        }
        public SegmentNode(string Name, PointNode pointA, PointNode pointB)
        {
            lineName = Name;
            this.pointA = pointA;
            this.pointB = pointB;
        }

        public override Node Evaluate(State state)
        {
            return this;
        }

        public override object Clone()
        {
            return new SegmentNode(lineName, (PointNode)pointA.Clone(), (PointNode)pointB.Clone());
        }
    }
    /// <summary>
    /// Nodo que contiene una declaracion de segmento
    /// </summary>
    /// */
    public class SegmentDeclarationNode : Node
    {
        Line line;
        public SegmentDeclarationNode(string Name)
        {
            line = new Line(Name, LineType.Segment);
        }
        private SegmentDeclarationNode(Line segment)
        {
            line = segment;
        }

        public override object Clone()
        {
            return new SegmentDeclarationNode(line);
        }

        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = line.name, Value = new UnaryExpressionNode(line) });
           return new UnaryExpressionNode(line);
        }
    }
    /// <summary>
    /// Nodo que contiene un rayo
    /// </summary>
    /// 
    /*
    public class RayNode : Node

    {
        public string lineName { get; private set; }
        public PointNode pointA { get; set; }
        public PointNode pointB { get; set; }
        public RayNode(string Name)
        {
            lineName = Name;
            pointA = new PointNode("");
            pointB = new PointNode("");
        }
        public RayNode(string Name, PointNode pointA, PointNode pointB)
        {
            lineName = Name;
            this.pointA = pointA;
            this.pointB = pointB;
        }

        public override Node Evaluate(State state)
        {
            return this;
        }

        public override object Clone()
        {
            return new RayNode(lineName, (PointNode)pointA.Clone(), (PointNode)pointB.Clone());
        }
    }
    */
    /// <summary>
    /// Nodo que contiene una declaracion de rayo
    /// </summary>
    public class RayDeclarationNode : Node
    {
        Line ray;
        public RayDeclarationNode(string Name)
        {
            ray = new Line(Name, LineType.Ray);
        }
        private RayDeclarationNode(Line Ray)
        {
            ray = Ray;
        }

        public override object Clone()
        {
            return new RayDeclarationNode(ray);
        }

        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = ray.name, Value = new UnaryExpressionNode(ray) });
            return new UnaryExpressionNode(ray);
        }
    }
    /// <summary>
    /// Nodo que contiene una linea
    /// </summary>
    /*
    public class LineNode : Node

    {
        public string lineName { get; private set; }
        public PointNode pointA { get; set; }
        public PointNode pointB { get; set; }
        public LineNode(string Name)
        {
            lineName = Name;
            pointA = new PointNode("");
            pointB = new PointNode("");
        }
        public LineNode(string Name, PointNode pointA, PointNode pointB)
        {
            lineName = Name;
            this.pointA = pointA;
            this.pointB = pointB;
        }

        public override Node Evaluate(State state)
        {
            return this;
        }

        public override object Clone()
        {
            return new LineNode(lineName, (PointNode)pointA.Clone(), (PointNode)pointB.Clone());
        }
    }
    */
    /// <summary>
    /// Nodo que contiene  una declaracion de linea
    /// </summary>
    public class LineDeclarationNode : Node
    {
        Line line;
        public LineDeclarationNode(string Name)
        {
            line = new Line(Name, LineType.Line);
        }

        private LineDeclarationNode(Line Line)
        {
            line = Line;
        }

        public override object Clone()
        {
            return new LineDeclarationNode(line);
        }

        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = line.name, Value = new UnaryExpressionNode(line) });
            return new UnaryExpressionNode(line);
        }
    }
    /// <summary>
    /// Nodo que contiene un circulo
    /// </summary>
    //
    /*
    public class CircleNode : Node
    {
        public string Name;
        public CircleNode(string Name)
        {
            this.Name = Name;
            center = new("");
            radio = new() { Value = 100 };
        }
        public CircleNode(string Name, PointNode center, NumberNode radio)
        {
            this.Name = Name;
            this.center = center;
            this.radio = radio;
        }

        public PointNode center { get; set; }
        public NumberNode radio { get; set; }

        public override object Clone()
        {
            return new CircleNode(Name, (PointNode)center.Clone(), (NumberNode)radio.Clone());
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
    */
    /// <summary>
    /// Nodo que contiene una declaracion de circulo
    /// </summary>
    public class CircleDeclarationNode : Node
    {
        private CircleDeclarationNode(Circle Circle)
        {
            circle = Circle;
        }
        public CircleDeclarationNode(string Name)
        {
            circle = new(Name);
        }
        public Circle circle;
        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = circle.Name, Value = new UnaryExpressionNode(circle) });
            return new UnaryExpressionNode(circle);
        }

        public override object Clone()
        {
            return new CircleDeclarationNode(circle);
        }
    }
    /// <summary>
    /// Nodo que contiene un arco
    /// </summary>
    /// 
    /*
    public class ArcNode : Node
    {
        public string Name;
        public PointNode center;
        public PointNode p1;
        public PointNode p2;

        public NumberNode radio;

        public ArcNode(string Name)
        {
            this.Name = Name;
            center = new("");
            p1 = new("");
            p2 = new("");
            radio = new NumberNode() { Value = 70 };
        }
        public ArcNode(string Name, PointNode center, PointNode p1, PointNode p2, NumberNode radio)
        {
            this.Name = Name;
            this.center = center;
            this.p1 = p1;
            this.p2 = p2;
            this.radio = radio;
        }

        public override object Clone()
        {
            return new ArcNode(Name, (PointNode)center.Clone(), (PointNode)p1.Clone(), (PointNode)p2.Clone(), (NumberNode)radio.Clone());
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
    */
    /// <summary>
    /// Nodo que contiene una declaracion de arco
    /// </summary>
    public class ArcDeclarationNode : Node
    {
        public Arc arc;
        private ArcDeclarationNode(Arc arcNode)
        {
            arc = arcNode;
        }

        public ArcDeclarationNode(string arcName)
        {
            arc = new(arcName);
        }

        public override object Clone()
        {
            return new ArcDeclarationNode(arc);
        }

        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = arc.Name, Value = new UnaryExpressionNode(arc) });
            return new UnaryExpressionNode(arc);
        }
    }
    /// <summary>
    /// Nodo que contiene una operacion binaria
    /// </summary>
    public class BinaryOperationNode : Node
    {
        public Node Left { get; set; }
        public TokenType Operator { get; set; }
        public Node Right { get; set; }

        public override object Clone()
        {
            return new BinaryOperationNode() { Left = (Node)Left.Clone(), Operator = Operator, Right = (Node)Right.Clone() };
        }

        public override Node Evaluate(State state)
        {


            return Operations.BinaryOperation(Left.Evaluate(state), Right.Evaluate(state), Operator);
        }
    }
    /// <summary>
    /// Nodo que contiene una operacion binaria
    /// </summary>
    public class UnaryOperationNode : Node
    {
        public TokenType Operator { get; set; }
        public Node Operand { get; set; }

        public override object Clone()
        {
            return new UnaryOperationNode() { Operator = Operator, Operand = (Node)Operand.Clone() };
        }

        public override Node Evaluate(State state)
        {
            return Operations.UnaryOperation(Operand.Evaluate(state), Operator);
        }
    }
    /// <summary>
    /// Nodo principal que contiene los statemes
    /// </summary>
    public class BlockNode : Node
    {
        public List<Node> statements { get; set; }

        public override object Clone()
        {
            List<Node> statementsNodes = new();
            foreach (var item in statements)
            {
                statementsNodes.Add((Node)item.Clone());
            }
            return new BlockNode() { statements = statementsNodes };
        }

        public override Node Evaluate(State state)
        {
            foreach (var statement in statements)
            {
                statement.Evaluate(state);
            }
            return new NullNode();
        }
    }
    /// <summary>
    /// Nodo que contiene una expresion let-in
    /// </summary>
    public class LetInNode : Node
    {
        public List<Node> Declarations { get; set; }
        public Node Body { get; set; }

        public override object Clone()
        {
            List<Node> declarationNodes = new();
            foreach (var item in Declarations)
            {
                declarationNodes.Add((Node)item.Clone());
            }
            return new LetInNode() { Declarations = declarationNodes, Body = (Node)Body.Clone() };
        }

        public override Node Evaluate(State state)
        {
            State localState = (State)state.Clone();
            foreach (var item in Declarations)
            {
                item.Evaluate(localState);
            }
            Node node = Body.Evaluate(localState);
            
            return node;
        }
    }
    /// <summary>
    /// Nodo que contiene un error
    /// </summary>
    public class ErrorNode : Node
    {
        public string Error;

        public override object Clone()
        {
            return new ErrorNode() { Error = Error };
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
    /// <summary>
    /// Nodo que contiene una expresion if-else
    /// </summary>
    public class IfElseNode : Node
    {
        public Node Condition { get; set; }
        public Node ThenBranch { get; set; }
        public Node ElseBranch { get; set; }

        public override object Clone()
        {
            return new IfElseNode() { Condition = (Node)Condition.Clone(), ThenBranch = (Node)ThenBranch.Clone(), ElseBranch = (Node)ElseBranch.Clone() };
        }

        public override Node Evaluate(State state)
        {
            if (((UnaryExpressionNode)Condition.Evaluate(state)).GetValue(state) is Number)
            {
                if ((((UnaryExpressionNode)Condition.Evaluate(state)).GetValue(state) as Number).Value == 0)
                    return ElseBranch.Evaluate(state);
                else return ThenBranch.Evaluate(state);
            }
            else return ThenBranch.Evaluate(state);
        }
    }
    /// <summary>
    /// Nodo que se trata como nulo
    /// </summary>
    public class NullNode : Node

    {
        public override object Clone()
        {
            return new NullNode();
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
    /// <summary>
    /// Nodo que se trata como undefined
    /// </summary>
    public class UndefinedNode : Node
    {
        public override object Clone()
        {
            return new UndefinedNode();
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
    public class UnaryExpressionNode : Node
    {

        public Object obj { get; private set; }
        public UnaryExpressionNode(Object @object)
        {
            obj = @object;
        }

        public bool IsValueAConstant()
        {
            return obj is ConstantCall;
        }

        public Object GetValue(State state)
        {
            return obj.GetValue(state);
        }
        public override object Clone()
        {
            return this;
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
}