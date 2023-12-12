using System.Drawing;
using System.Formats.Asn1;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.IO;


namespace Compiler
{

    public abstract class Node : ICloneable
    {
        public abstract object Clone();

        public abstract Node Evaluate(State state);
        
    }
     
    
    public class ImportNode : Node
    {
        public string Path { get; set; }

        public override object Clone()
        {
            return new ImportNode(){Path = Path};
        }

        public override Node Evaluate(State state)
        {
            if(!state.imported.Contains(Path))
            {
                state.imported.Add(Path);
                state.Run(ReadFile(Path));
            }

            
            return new NullNode();
        }
        public string ReadFile(string filename)
    {
        if(File.Exists(filename))
        {
            return File.ReadAllText(filename);
        }
        else
        {
            throw new FileNotFoundException("File not found: " + filename);
        }  
    }
    }

    public class SequencDeclarationNode : Node
    {
        public List<Token> constants = new();
        public Node body {get; set;}

        public override object Clone()
        {
            return new SequencDeclarationNode(){constants = constants,body = (Node)body.Clone()};
        }

        public override Node Evaluate(State state)
        {
            if(body.Evaluate(state).GetType().ToString() != "Compiler.SequenceNode")
            {
                throw new Exception("Expected Sequence at sequence constants declarations");
            }
            else{
                SequenceNode seq = (SequenceNode)body.Evaluate(state);
                for (int i = 0; i < constants.Count; i++)
                {
                    if(constants[i].Type == TokenType.GuionBajo)
                    {
                        continue;
                    }
                    else{
                        if(i >= seq.nodes.Count)
                        state.AddConstant(new(){Name = constants[i].Value, Value = new UndefinedNode()});
                        else state.AddConstant(new(){Name = constants[i].Value, Value = seq.nodes[i]});
                    }
                }
            }
            return new NullNode();
        }
    }
    public class PointDeclarationNode : Node
    {
        private PointDeclarationNode()
        {

        }
        public PointNode point { get; set; }

        public PointDeclarationNode(string pName)
        {
            point = new PointNode(pName);
        }
        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = point.pointName, Value = point });
            return new NullNode();
        }

        public override object Clone()
        {
            return new PointDeclarationNode(){point = (PointNode)point.Clone()};
        }
    }
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
            return new PointNode(){pointName = pointName, xValue = xValue, yValue = yValue};
        }
    }


    public class ColorNode : Node
    {
        public string Id { get; set; }

        public override object Clone()
        {
            return new ColorNode(){Id = Id};
        }

        public override Node Evaluate(State state)
        {
            throw new NotImplementedException();
        }
    }

    public class MeasureNode : Node
    {
        public double mValue;

        public override object Clone()
        {
            return new MeasureNode(){mValue = mValue};
        }

        public override Node Evaluate(State state)
        {
            return new NumberNode() { Value = mValue };
        }
    }

    public class DrawNode : Node
    {
        public DrawNode()
        {

        }
        public string figToDraw { get; set; }
        public string label { get; set; }

        public override object Clone()
        {
            return new DrawNode(){figToDraw = figToDraw, label = label};
        }

        public override Node Evaluate(State state)
        {
            if(state.GetConstant(figToDraw).Value.Evaluate(state).GetType().ToString() == "Compiler.SequenceNode")
            {
                SequenceNode sequenceNode = (SequenceNode)state.GetConstant(figToDraw).Value.Evaluate(state);
                if(((SequenceNode)sequenceNode.Evaluate(state)).Type != "undefined")
                {
                    for (int i = 0; i < sequenceNode.count.Value; i++)
                    {
                        state.AddToDraw(sequenceNode.nodes[i].Evaluate(state));
                    }
                }
            }
           else  state.AddToDraw(state.GetConstant(figToDraw).Value.Evaluate(state));

            return new NullNode();
        }
    }

    public class FunctionDeclarationNode : Node
    {
        public string Name { get; set; }
        public List<string> Parameters { get; set; }
        public Node Body { get; set; }

        public override object Clone()
        {
            return new FunctionDeclarationNode(){Name = Name,Parameters = Parameters,Body = (Node)Body.Clone()};
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
            return new FunctionCallNode(){Name = Name, Arguments = nodesTemp};
        }

        public override Node Evaluate(State state)
        {
            switch (Name)
            {
                case "line":
                    if (Arguments.Count != 2) throw new Exception("Expected 2 arguments in function line call");
                    return new LineNode("", (PointNode)Arguments[0].Evaluate(state), (PointNode)Arguments[1].Evaluate(state));
                case "segment":
                    if (Arguments.Count != 2) throw new Exception("Expected 2 arguments in function line call");
                    return new SegmentNode("", (PointNode)Arguments[0].Evaluate(state), (PointNode)Arguments[1].Evaluate(state));
                case "ray":
                    if (Arguments.Count != 2) throw new Exception("Expected 2 arguments in function line call");
                    return new RayNode("", (PointNode)Arguments[0].Evaluate(state), (PointNode)Arguments[1].Evaluate(state));
                case "circle":
                    if (Arguments.Count != 2) throw new Exception("Expected 2 arguments in function circle call");
                    return new CircleNode("", (PointNode)Arguments[0].Evaluate(state), (NumberNode)Arguments[1].Evaluate(state));
                case "arc":
                    if (Arguments.Count != 4) throw new Exception("Expected 4 arguments in function arc call");
                    return new ArcNode("", (PointNode)Arguments[0].Evaluate(state), (PointNode)Arguments[1].Evaluate(state), (PointNode)Arguments[2].Evaluate(state), (NumberNode)Arguments[3].Evaluate(state));
                case "print":
                    if (Arguments.Count != 1) throw new Exception("Expected 1 argument in function print");
                    {
                        //throw new Exception("amaterasu -->>"+Arguments[0].Evaluate(state)+"<<-- asd");
                        Node resultNode = Arguments[0].Evaluate(state);
                        if (resultNode.GetType().ToString() == "Compiler.NumberNode")
                        {
                            state.toPrint.Add(((NumberNode)resultNode).Value.ToString());
                        }
                        else state.toPrint.Add("No se pudo imprimir -->> " + resultNode.GetType());
                    }
                    return new NullNode();
                case "measure":
                    if (Arguments.Count != 2) throw new Exception("Expected 2 argument in function ");
                    return Operations.Distance(Arguments[0].Evaluate(state), Arguments[1].Evaluate(state));


                default:
                    FunctionDeclarationNode func = state.CallFunction(Name);
                    Node result = func.GetValue(Arguments, state).Evaluate(state);
                    //if(func.Parameters.Count )
                    return result;
            }
        }
    }
    public class SequenceNode : Node
    {
        public List<Node> nodes = new();
        public string Type = "undefined";
        public NumberNode count{get => new NumberNode(){Value = nodes.Count};}

        public void Add(Node node,State state)
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
            return new SequenceNode(){nodes = nodesTemp};
        }

        public override Node Evaluate(State state)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if(i == 0)
                {
                    Type = nodes[0].Evaluate(state).GetType().ToString();
                }
                else{
                    if(nodes[i].Evaluate(state).GetType().ToString() != Type)
                    throw new Exception("A sequence must be of the same type, Expected: " + Type + "--Got: "+nodes[i].Evaluate(state).GetType());
                }
            }
            return this;
        }
    }

    public class ConstantDeclarationNode : Node
    {
        public string Name { get; set; }
        public Node Value { get; set; }

        public override object Clone()
        {
            return new ConstantDeclarationNode(){Name = Name, Value = (Node)Value.Clone()};
        }

        public override Node Evaluate(State state)
        {
            Value = Value.Evaluate(state);
            state.AddConstant(this);
            return new NullNode();
        }
    }

    public class ConstantCallNode : Node
    {
        public string Name { get; set; }

        public override object Clone()
        {
            return new ConstantCallNode(){Name = Name};
        }

        public override Node Evaluate(State state)
        {
            ConstantDeclarationNode constD = state.GetConstant(Name);
            Node result = (Node)constD.Value.Clone();
            return (Node)result.Evaluate(state).Clone();
        }
        //public Node Value { get; set; }
    }


    public class NumberNode : Node
    {
        public double Value { get; set; }

        public override object Clone()
        {
            return new NumberNode(){Value = Value};
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
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
            return new SegmentNode(lineName,(PointNode)pointA.Clone(),(PointNode)pointB.Clone());
        }
    }
    public class SegmentDeclarationNode : Node
    {
        SegmentNode line;
        public SegmentDeclarationNode(string Name)
        {
            line = new SegmentNode(Name);
        }
        private SegmentDeclarationNode(SegmentNode segmentNode)
        {
            line = segmentNode;
        }

        public override object Clone()
        {
            return new SegmentDeclarationNode((SegmentNode)line.Clone());
        }

        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = line.lineName, Value = line });
            return new NullNode();
        }
    }
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
            return new RayNode(lineName,(PointNode)pointA.Clone(),(PointNode)pointB.Clone());
        }
    }
    public class RayDeclarationNode : Node
    {
        RayNode ray;
        public RayDeclarationNode(string Name)
        {
            ray = new RayNode(Name);
        }
        private RayDeclarationNode(RayNode Ray)
        {
            ray = Ray;
        }

        public override object Clone()
        {
            return new RayDeclarationNode((RayNode)ray.Clone());
        }

        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = ray.lineName, Value = ray });
            return new NullNode();
        }
    }
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
            return new LineNode(lineName,(PointNode)pointA.Clone(),(PointNode)pointB.Clone());
        }
    }
    public class LineDeclarationNode : Node
    {
        LineNode line;
        public LineDeclarationNode(string Name)
        {
            line = new LineNode(Name);
        }

        private LineDeclarationNode(LineNode Line)
        {
            line = Line;
        }

        public override object Clone()
        {
            return new LineDeclarationNode((LineNode)line.Clone());
        }

        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = line.lineName, Value = line });
            return new NullNode();
        }
    }
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
            return new CircleNode(Name,(PointNode)center.Clone(),(NumberNode)radio.Clone());
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
    public class CircleDeclarationNode : Node
    {
        private CircleDeclarationNode(CircleNode Circle)
        {
circle = Circle;
        }
        public CircleDeclarationNode(string Name)
        {
            circle = new(Name);
        }
        public CircleNode circle;
        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = circle.Name, Value = circle });
            return new NullNode();
        }

        public override object Clone()
        {
            return new CircleDeclarationNode((CircleNode)circle.Clone());
        }
    }
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
            return new ArcNode(Name,(PointNode)center.Clone(),(PointNode)p1.Clone(),(PointNode)p2.Clone(),(NumberNode)radio.Clone());
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
    public class ArcDeclarationNode : Node
    {
         public ArcNode arc;
        private ArcDeclarationNode(ArcNode arcNode)
        {
            arc = arcNode;
        }
       
        public ArcDeclarationNode(string arcName)
        {
            arc = new(arcName);
        }

        public override object Clone()
        {
            return new ArcDeclarationNode((ArcNode)arc.Clone());
        }

        public override Node Evaluate(State state)
        {
            state.AddConstant(new ConstantDeclarationNode() { Name = arc.Name, Value = arc });
            return new NullNode();
        }
    }
    public class BinaryOperationNode : Node
    {
        public Node Left { get; set; }
        public TokenType Operator { get; set; }
        public Node Right { get; set; }

        public override object Clone()
        {
            return new BinaryOperationNode(){Left = (Node)Left.Clone(),Operator = Operator, Right = (Node)Right.Clone()};
        }

        public override Node Evaluate(State state)
        {
            

            return Operations.BinaryOperation(Left.Evaluate(state), Right.Evaluate(state), Operator);
        }
    }

    public class UnaryOperationNode : Node
    {
        public TokenType Operator { get; set; }
        public Node Operand { get; set; }

        public override object Clone()
        {
            return new UnaryOperationNode(){Operator = Operator, Operand = (Node)Operand.Clone()};
        }

        public override Node Evaluate(State state)
        {
            return Operations.UnaryOperation(Operand.Evaluate(state), Operator);
        }
    }

    public class BlockNode : Node
    {
        public List<Node> statements{get;set;}

        public override object Clone()
        {
            List<Node> statementsNodes = new();
            foreach (var item in statements)
            {
              statementsNodes.Add((Node)item.Clone());   
            }
            return new BlockNode(){statements = statementsNodes};
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
    public class LetInNode : Node
    {
        public List<ConstantDeclarationNode> Declarations { get; set; }
        public Node Body { get; set; }

        public override object Clone()
        {
            List<ConstantDeclarationNode> declarationNodes = new();
            foreach (var item in Declarations)
            {
                declarationNodes.Add((ConstantDeclarationNode)item.Clone());
            }
            return new LetInNode(){Declarations = declarationNodes, Body = (Node)Body.Clone()};
        }

        public override Node Evaluate(State state)
        {
            foreach (var item in Declarations)
            {
                state.AddConstant((ConstantDeclarationNode)item);
            }
            Node node = Body.Evaluate(state);
            foreach (var item in Declarations)
            {
                state.RemoveConstant(item.Name);
            }
            return node;
        }
    }

    public class ErrorNode : Node
    {
        public string Error;

        public override object Clone()
        {
            return new ErrorNode(){Error = Error};
        }

        public override Node Evaluate(State state)
        {
            return this;
        }
    }

    public class IfElseNode : Node
    {
        public Node Condition { get; set; }
        public Node ThenBranch { get; set; }
        public Node ElseBranch { get; set; }

        public override object Clone()
        {
            return new IfElseNode(){Condition = (Node)Condition.Clone(),ThenBranch = (Node)ThenBranch.Clone(),ElseBranch = (Node)ElseBranch.Clone()};
        }

        public override Node Evaluate(State state)
        {
            if (((NumberNode)Condition.Evaluate(state)).Value == 0)
            {
                return ElseBranch.Evaluate(state);
            }
            else return ThenBranch.Evaluate(state);
        }
    }
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
}