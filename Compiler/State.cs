namespace Compiler
{
    public class State
    {
        public List<PointNode> RunTest1(string input)
        {
            Parser.LevelsInit();
            List<string> stringTokens = new List<string>();

            List<Token> tokens = Lexer.TokensInit(input);

            List<Node> nodes = Parser.Parse(this, tokens);
            
            
            foreach (Node item in nodes)
            {
                if (item.GetType() == "ErrorNode") errorNodes.Add((ErrorNode)item);
            }

            
            //return new List<PointNode>(){new PointNode(((MainNode)nodes[0]).nodes[0].tokenList[0].ToString())};

            foreach (Node node in nodes)
            {
                Node nod;
                if (node.GetType() == "MainNode") { 
                    nod = Parser.ParseLevel(((MainNode)node).nodes, 1, this);
                    
                    if (nod.GetType() == "PointNode") PointsToDraw.Add((PointNode)nod);
                     else if (nod.GetType() == "ErrorNode") errorNodes.Add((ErrorNode)nod);
                 }
                 else if(node.GetType() == "ErrorNode") errorNodes.Add((ErrorNode)node);
                
                
            }
           
            return PointsToDraw;

        }
        public List<ErrorNode> errorNodes = new();
        private readonly string[] reservedWords = { "if", "else", "then", "let", "in", "point", "segment", "line", "circle", "draw" };
        public List<ConstantNode> constantNodes = new List<ConstantNode>();
        
        public string toPrint = "";
        public List<string> pointsDeclared = new();
        public List<PointNode> PointsToDraw = new();

        public List<FunctionNode> activeFunctions = new List<FunctionNode>();

        public bool ConstantExist(string constantName)
        {
            foreach (ConstantNode node in constantNodes)
            {
                if (node != null)
                {
                    if (node.GetName() == constantName) return true;
                }
            }

            return false;
        }
        public ConstantNode GetConstantNode(string name)
        {
            foreach (ConstantNode c in constantNodes)
            {
                if(c.GetName() == name) return (ConstantNode)c.Clone();
            }
            throw new Exception("no se encontró la constante -->") ;
        }
        public FunctionNode GetFunction(string name)
        {
            foreach (FunctionNode f in activeFunctions)
            {
                if(f.GetName() == name) return (FunctionNode)f.Clone();
            }
            throw new Exception("no se encontró la función");
        }
        public bool FunctionExist(string functionName)
        {
            foreach (FunctionNode node in activeFunctions)
            {
                if (node != null)
                {
                    if (node.GetName() == functionName) return true;
                }
            }

            return false;
        }

        public bool IsReserved(string word)
        {
            return reservedWords.Contains(word);

        }
    }
}