namespace Compiler
{
    public class State
    {
        public List<PointNode> RunTest1(string input)
        {
           
            List<string> stringTokens = new List<string>();
            
            List<Token> tokens = Lexer.TokensInit(input);
            
            List<Node> nodes = Parser.Parse(this,tokens);

            //return new List<PointNode>(){new PointNode(((MainNode)nodes[0]).nodes[0].tokenList[0].ToString())};
             
            foreach (Node node in nodes)
            {
                var nod = Parser.ParseLevel(((MainNode)node).nodes,1,this);
                if(nod.GetType() == "PointNode") PointsToDraw.Add((PointNode)nod);
            }

            return PointsToDraw;
 
        }
        private readonly string[] reservedWords = {"if","else","then","let","in","point","segment","line","circle","draw"};
        public List<ConstantNode> constantNodes = new List<ConstantNode>();

        public List<PointNode> PointsToDraw = new();
        
        public List<FunctionNode> activeFunctions = new List<FunctionNode>();

        public bool ConstantExist(string constantName)
        {
            foreach (ConstantNode node in constantNodes)
            {
                if(node != null)
                {
                    if(node.GetName() == constantName) return true;
                }
            }

             return false;
        }

        public bool FunctionExist(string functionName)
        {
            foreach (FunctionNode node in activeFunctions)
            {
                if(node != null )
                {
                    if(node.GetName() == functionName) return true;
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