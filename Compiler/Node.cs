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
        public abstract Node Parse();
        public abstract  string GetType();

        public abstract object Clone();
    }

    

    public class ParentesisNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            return "ParentesisNode";
        }

        public override Node Parse()
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

        public override Node Parse()
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

        public override Node Parse()
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
        public FunctionNode(string functionName)
        {
            Name = functionName;
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
            throw new NotImplementedException();
        }

        public override Node Parse()
        {
            throw new NotImplementedException();
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

        public override Node Parse()
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

        public override Node Parse()
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

        public override Node Parse()
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

        public override Node Parse()
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
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse()
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

        public override Node Parse()
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

        public override Node Parse()
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

        public override Node Parse()
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

    public class LineDeclarationNode : Node
    {
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse()
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

        public override Node Parse()
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

        public override Node Parse()
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

        public override Node Parse()
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
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override Node Parse()
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

        public override Node Parse()
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
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

        public override Node Parse()
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

        public override Node Parse()
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

        public override string GetType()
        {
            return "iden";
        }

        public override Node Parse()
        {
            throw new NotImplementedException();
        }

        public override void Remplace(Dictionary<Token, Token> valuePairs)
        {
            throw new NotImplementedException();
        }
    }
}