namespace Compiler
{
    public class State
    {
        private Dictionary<string, FunctionDeclarationNode> functions = new Dictionary<string, FunctionDeclarationNode>();
        public Dictionary<string, ConstantDeclarationNode> constants = new Dictionary<string, ConstantDeclarationNode>();
        public List<string> toPrint = new();
        public List<Node> toDraw = new();
        public List<ErrorNode> errors = new();

        public void Run(string input)
        {
            try
            {
                Lexer.Tokenizer tokenizer = new Lexer.Tokenizer(input);
                TokenList tokens = new(tokenizer.Tokenize().ToList());
                Parser parser = new(tokens, this);
                Node node = parser.Parse();
                node.Evaluate(this);
            }
            catch (Exception e)
            {
                errors.Add(new() { Error = e.ToString() });
            }
        }
        public void AddToDraw(Node node)
        {
            toDraw.Add(node);
        }
        public bool ContainsFunction(string functionName)
        {
            return functions.ContainsKey(functionName);
        }
        public bool ContainsConstant(string constantName)
        {
            return constants.ContainsKey(constantName);
        }

        public void RemoveConstant(string name)
        {
            constants.Remove(name);
        }

        //
        public bool Contains(string word) //verdadero si contiene una funcion o constante con ese nombre
        {
            return ContainsFunction(word) || ContainsConstant(word);
        }

        public void AddFunction(FunctionDeclarationNode function)
        {
            if (functions.ContainsKey(function.Name) || constants.ContainsKey(function.Name))
            {
                throw new Exception("A function or constant with the name '" + function.Name + "' already exists.");
            }
            functions[function.Name] = function;
        }

        public void AddConstant(ConstantDeclarationNode constant)
        {
            if (functions.ContainsKey(constant.Name) || constants.ContainsKey(constant.Name))
            {
                throw new Exception("A function or constant with the name '" + constant.Name + "' already exists.");
            }
            constants[constant.Name] = constant;
        }
        public FunctionDeclarationNode CallFunction(string name)
        {
            if (!functions.ContainsKey(name))
            {
                throw new Exception("No function with the name '" + name + "' exists.");
            }
            return functions[name];
        }

        public ConstantDeclarationNode GetConstant(string name)
        {
            if (!constants.ContainsKey(name))
            {
                throw new Exception("No constant with the name '" + name + "' exists.");
            }
            return constants[name];
        }
    }
}