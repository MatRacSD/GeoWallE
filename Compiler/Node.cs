using System.Reflection.Metadata;

namespace Compiler
{
    /// <summary>
    /// 
    /// </summary>
    public abstract  class  Node : ICloneable
    {
        

        public bool IsTerminal;

        public Token? token;

        public Node[]? Childs;

        public List<Token> tokenList = new();
        
        public abstract void Remplace(Dictionary<Token,Token> valuePairs);
        public abstract Node Parse(State state);
        public abstract  string GetType();

        public abstract object Clone();
    }

    

    public class ParentesisNode : Node
    {
        public override object Clone()
        {
            return new ParentesisNode(){tokenList = this.tokenList.Clone()};
        }

        public override string GetType()
        {
            return "ParentesisNode";
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class UnparsedNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }
        

        public override string GetType()
        {
            return "UnparsedNode";
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }
    public class MainNode : Node
    {
        public List<Node> nodes;

        public MainNode(List<Node> nods)
        {
            nodes = nods;
        }
        public MainNode(List<Token> tokens)
        {
            tokenList = tokens.Clone();
        }
        public List<Token> tokenList;

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            return  "MainNode";
        }

        public override MainNode Clone()
        {
            MainNode mainClone = new(nodes);
            return mainClone;
        }
    }

    public class FunctionNode : Node
    {
        private readonly string Name;

        private ParentesisNode parametros;
        public FunctionNode(string functionName)
        {
            Name = functionName;
        }

        public  void SetFunctionBody( List<Identificador> param, UnparsedNode functionBody)
        {
              
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return Name;
        }

        public override string GetType()
        {
            return "FunctionNode";
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        
        public void SetParameters(ParentesisNode param)
        {
             parametros = (ParentesisNode)param.Clone();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }



    public class AssignationNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class FunctionDeclarationNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }



    public class LetInNode : Node
    {
        
        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public Dictionary<Token,Node> Identificadores;
    }

    public class IfElseNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            return "IfElse";
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class BinaryOperationNode : Node
    {
        public Node parsedLefthChild;
        public Node parsedRightChild;
        public List<Node> LeftChild = new();
        public List<Node> RightChild = new();
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            return "BinaryOperationNode";
        }
       

        public override Node Parse(State state)
        {
            
            parsedLefthChild = Parser.ParseLevel(LeftChild,1,state);
            parsedRightChild = Parser.ParseLevel(RightChild,1,state);
            return new NullNode();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class NullNode : Node

    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            return "null";
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class UnaryOperationNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class DrawNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class PointNode : Node
    {
        public string name;
        public bool isDefined;

        public double x;
        public double y;

        public double[] GetPair()
        {
            return new double[]{x,y};
        }
        public PointNode(string Name)
        {
            Random random = new();
            isDefined = true;
            name = Name;
            x = (double)random.Next(301);
            y = -1 * (double)random.Next(301);
            if(random.Next(2) == 0) x *= -1;
            if(random.Next(2) == 0) y *= -1;

        }
        public override string GetType()
        {
            return  "PointNode";
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }

    public class LineNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class CircleDeclarationNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class SegmentDeclarationNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class ConstantDeclarationNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberNode : Node
    {
        public NumberNode(Token token)
        {
            this.token = token.Clone();
        }
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            return "NumberNode";
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class ConstantNode : Node
    {
        //private UnparsedNode unparsedConstantBody;
        private  Node parseConstantBody;
        public ConstantNode(string constantName)
        {
            Name = constantName;
        }

        public string GetName()
        {
            return Name;
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }

        

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            return "ConstantNode";
        }
    
        public void SetBody(List<Node> nodes, State state)
        {
           //UnparsedNode unparsedNode = new UnparsedNode(){tokenList = tokens.Clone()};
           parseConstantBody = Parser.ParseLevel(nodes,1,state);
           
        }

        public override ConstantNode Clone()
        {
            return this;
        }

        private readonly string Name;
    }

    public class GeometricObjectNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class ErrorNode : Node
    {
         public string Error {get; set;}

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            return "ErrorNode";
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }

    public class Identificador : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }
        public string GetName()
        {
            return name;
        }
        private readonly string name;
        public Identificador(string n)
        {
           name = n;
        }

        public override string GetType()
        {
            return "id";
        }

        public override Node Parse(State state)
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }
}