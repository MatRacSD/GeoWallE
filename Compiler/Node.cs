using System.Drawing;
using System.Formats.Asn1;
using System.Reflection.Metadata;

namespace Compiler
{

    public abstract class Node
    {
        public abstract Node Evaluate(State state);
        //public abstract Type GetType();
    }
     
    
    public class ImportNode : Node
    {
        public string Path { get; set; }

        public override Node Evaluate(State state)
        {
            throw new NotImplementedException();
        }
    }

    public class SequencDeclarationNode : Node
    {
        public List<Token> constants = new();
        public Node body {get; set;}
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
    }
    public class PointNode : Node
    {

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
    }


    public class ColorNode : Node
    {
        public string Id { get; set; }

        public override Node Evaluate(State state)
        {
            throw new NotImplementedException();
        }
    }

    public class MeasureNode : Node
    {
        public double mValue;

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
            for (int i = 0; i < args.Count; i++)
            {
                state.AddConstant(new ConstantDeclarationNode() { Name = Parameters[i], Value = args[i] });
            }
            Node result = Body.Evaluate(state);
            for (int i = 0; i < Parameters.Count; i++)
            {
                state.RemoveConstant(Parameters[i]);
            }
            return result;
        }

    }

    public class FunctionCallNode : Node
    {
        public string Name { get; set; }
        public List<Node> Arguments { get; set; }

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

        public void Add(Node node,State state)
        {
            
                nodes.Add(node);
                
            
            
        }

        public NumberNode count{get => new NumberNode(){Value = nodes.Count};}

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

        public override Node Evaluate(State state)
        {
            return state.GetConstant(Name).Value.Evaluate(state);
        }
        //public Node Value { get; set; }
    }


    public class NumberNode : Node
    {
        public double Value { get; set; }

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
    }
    public class SegmentDeclarationNode : Node
    {
        SegmentNode line;
        public SegmentDeclarationNode(string Name)
        {
            line = new SegmentNode(Name);
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
    }
    public class RayDeclarationNode : Node
    {
        RayNode ray;
        public RayDeclarationNode(string Name)
        {
            ray = new RayNode(Name);
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
    }
    public class LineDeclarationNode : Node
    {
        LineNode line;
        public LineDeclarationNode(string Name)
        {
            line = new LineNode(Name);
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
        public override Node Evaluate(State state)
        {
            return this;
        }
    }
    public class CircleDeclarationNode : Node
    {
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

        public override Node Evaluate(State state)
        {
            return this;
        }
    }
    public class ArcDeclarationNode : Node
    {
        public ArcNode arc;
        public ArcDeclarationNode(string arcName)
        {
            arc = new(arcName);
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

        public override Node Evaluate(State state)
        {
            return Operations.BinaryOperation(Left.Evaluate(state), Right.Evaluate(state), Operator);
        }
    }

    public class UnaryOperationNode : Node
    {
        public TokenType Operator { get; set; }
        public Node Operand { get; set; }

        public override Node Evaluate(State state)
        {
            return Operations.UnaryOperation(Operand.Evaluate(state), Operator);
        }
    }

    public class BlockNode : Node
    {
        public List<Node> statements;

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
        public override Node Evaluate(State state)
        {
            return this;
        }
    }

    public class UndefinedNode : Node
    {
        public override Node Evaluate(State state)
        {
            return this;
        }
    }
}