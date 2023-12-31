using System.Runtime.InteropServices;

namespace Compiler
{
    public class State : ICloneable
    {
        /// <summary>
        /// funciones declaradas
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, FunctionDeclarationNode> functions = new();
        /// <summary>
        /// constantes declaradas
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ConstantDeclarationNode> constants = new();
        
        /// <summary>
        /// Archivos importados
        /// </summary>
        /// <returns></returns>
        public List<string> imported = new();
        /// <summary>
        /// Para imprimir
        /// </summary>
        /// <returns></returns>
        public List<string> toPrint = new();
        /// <summary>
        /// Para pintar
        /// </summary>
        /// <returns></returns>
        public List<Node> toDraw = new();
        /// <summary>
        /// Errores encontrados
        /// </summary>
        /// <returns></returns>
        public List<ErrorNode> errors = new();
        
        public object Clone()
        {
           Dictionary<string,ConstantDeclarationNode> constantNodes = new();
           foreach (var item in constants)
           {
              constantNodes.Add(item.Key,(ConstantDeclarationNode)item.Value.Clone());
           }
            return new State(){functions = functions, constants = constantNodes,toDraw = toDraw,toPrint = toPrint,errors = errors};
        }
        /// <summary>
        /// Parsea y evalua el input
        /// </summary>
        /// <param name="input"></param>
        public void Run(string input)
        {
            try
            {
                Lexer.Tokenizer tokenizer = new Lexer.Tokenizer(input); 
                TokenList tokens = new(tokenizer.Tokenize().ToList()); //Se tokeniza
                Parser parser = new(tokens, this); 
                Node node = parser.Parse(); //Se parsea
                node.Evaluate(this); //Se Evalua
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
        /// <summary>
        /// Retorna verdadero si contiene una funcion con functionName
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public bool ContainsFunction(string functionName)
        {
            return functions.ContainsKey(functionName);
        }
        /// <summary>
        /// Retorna verdadero si contiene una constante con nombre constantName
        /// </summary>
        /// <param name="constantName"></param>
        /// <returns></returns>
        public bool ContainsConstant(string constantName)
        {
            return constants.ContainsKey(constantName);
        }
        /// <summary>
        /// Borra la constante con nombre name
        /// </summary>
        /// <param name="name"></param>

        public void RemoveConstant(string name)
        {
            constants.Remove(name);
        }

        
        public bool Contains(string word) //verdadero si contiene una funcion o constante con ese nombre
        {
            return ContainsFunction(word) || ContainsConstant(word);
        }
        /// <summary>
        /// Agrega la declaracion de funcion
        /// </summary>
        /// <param name="function"></param>
        public void AddFunction(FunctionDeclarationNode function)
        {
            if (functions.ContainsKey(function.Name) || constants.ContainsKey(function.Name))
            {
                throw new Exception("A function or constant with the name '" + function.Name + "' already exists.");
            }
            functions[function.Name] = function;
        }
        /// <summary>
        /// Agrega la declaracion de constante
        /// </summary>
        /// <param name="constant"></param>
        public void AddConstant(ConstantDeclarationNode constant)
        {
            if (functions.ContainsKey(constant.Name) || constants.ContainsKey(constant.Name))
            {
                throw new Exception("A function or constant with the name '" + constant.Name + "' already exists.");
            }
            constants[constant.Name] = constant;
        }
        /// <summary>
        /// Fuerza la addición de una constante nueva
        /// </summary>
        /// <param name="constant"></param>
        public void ForceAddConstant(ConstantDeclarationNode constant)
        {
            if (functions.ContainsKey(constant.Name))
            {
                throw new Exception("A function with the name '" + constant.Name + "' already exists.");
            }
            else if(  constants.ContainsKey(constant.Name))
            {
                constants[constant.Name] = (ConstantDeclarationNode)constant.Clone();
            }
            else constants.Add(constant.Name,  constant);
        }
        /// <summary>
        /// Retorna un clon de la declaracion de funcion
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FunctionDeclarationNode CallFunction(string name)
        {
            if (!functions.ContainsKey(name))
            {
                throw new Exception("No function with the name '" + name + "' exists.");
            }
            return (FunctionDeclarationNode)functions[name].Clone();
        }
        /// <summary>
        /// Returna un clon de la declaracion de constante
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConstantDeclarationNode GetConstant(string name)
        {
            if (!constants.ContainsKey(name))
            {
                throw new Exception("No constant with the name '" + name + "' exists.");
            }
            return (ConstantDeclarationNode)constants[name].Clone();
        }

        
    }
}