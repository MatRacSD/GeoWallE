

namespace Compiler
{

    public static class Parser
    {
        /// <summary>
        /// Crea un árbol de tipos Node a partir de la lista de tokens que se le pasa como parámetro.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="startlevel"></param>
        /// <returns></returns>
        public static Node Parse(List<Token> tokens, int startlevel)
        {
            levels.Add(LV0);
            levels.Add(LV0_1);
            levels.Add(LV0_2);
            levels.Add(LV1);
            levels.Add(LV2);
            levels.Add(LV2_1);

            levels.Add(LV3);
            levels.Add(LV4);

            return ParseLevel(tokens, levels[startlevel - 1], startlevel);

        }
        /// <summary>
        /// Parsea una lista de tokens según el nivel que se le haya pasado
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="lvls"></param>
        /// <param name="clevel"></param>
        /// <returns></returns>
        public static Node ParseLevel(List<Token> tokens, List<string> lvls, int clevel)
        {
            List<Token> aux_token_list = new List<Token>();
            Node? cNode = null;




            if (clevel == 2)
            {
                if (tokens[0].Content == "!")
                {
                    Node node = new Node { token = tokens[0], childs = new Node[1] };
                    tokens.RemoveAt(0);
                    node.childs[0] = Parse(tokens, 3);
                    return node;
                }
            }

            else if (clevel == 8)
            {
                if (tokens.Count > 1)
                {
                    throw new ArgumentException();
                }

                else if (tokens[0].Type == "number" || tokens[0].Type == "bool" || tokens[0].Type == "strings")
                {
                    return new Node() { token = tokens[0], IsTerminal = true };
                }

                else if (tokens[0].Content == "cpar")
                {
                    Node node = new Node() { token = tokens[0], childs = new Node[1] };
                    node.childs[0] = Parse(tokens[0].exp, 1);
                    return node;

                }

                else if (tokens[0].Content == "let-in")
                {
                    Node node = new Node() { token = tokens[0] };
                    return node;
                }

                else if (tokens[0].Content == "if-else")
                {
                    Node node = new Node() { token = tokens[0] };
                    return node;
                }
                else if (tokens[0].Type == "func")
                {
                    Node node = new Node() { token = tokens[0] };
                    return node;
                }

                Error.errors.Add(new Error() { Type = "SEMANTIC ERROR: no se encontraron tokens atomicos al intentar parsear" });
                return null;
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                if (lvls.Contains(tokens[i].Content))
                {
                    if (tokens[i].Content == "-" && aux_token_list.Count == 0)
                    {
                        aux_token_list.Add(tokens[i]);
                        continue;
                    }
                    else if (cNode == null)
                    {
                        Node node = new Node() { token = tokens[i], childs = new Node[2] };
                        node.childs[0] = Parse(aux_token_list, clevel + 1);
                        aux_token_list = new List<Token>();
                        cNode = node;
                    }
                    else
                    {
                        cNode.childs[1] = Parse(aux_token_list, clevel + 1);
                        aux_token_list = new List<Token>();
                        Node aux_node = cNode;
                        cNode = new Node() { token = tokens[i], childs = new Node[2] };
                        cNode.childs[0] = aux_node;
                    }
                }
                else aux_token_list.Add(tokens[i]);


            }
            if (cNode != null)
            {
                cNode.childs[1] = Parse(aux_token_list, clevel + 1);
                return cNode;
            }

            else return Parse(tokens, clevel + 1);
        }









        /// /
        /// /////////////////////
        ///         /// ///////////////////////////////////////////////////////
        /// ////////////////////////
        /// ///////////////////////////////////
        static List<List<string>> levels = new List<List<string>>();
        //static List<string> LV1 = new List<string> {"+"};

        static List<string> LV0 = new List<string> { "&", "|" };

        static List<string> LV0_1 = new List<string> { "!" };
        static List<string> LV0_2 = new List<string> { "<=", ">=", "==", "!=", "<", ">", };
        static List<string> LV1 = new List<string> { "+", "-", "@" };

        static List<string> LV2 = new List<string> { "*", "/", "%" };

        static List<string> LV2_1 = new List<string> { "^" };


        static List<string> LV3 = new List<string> { "-" };
        static List<string> LV4 = new List<string> { "let-in", "print", "number", "cpar", "bool", "if-else", "strings", "func" };





    }

    /// <summary>
    /// Representa un nodo
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Retorna un Token que representa el valor al evaluar el nodo actual y sus hijos
        /// </summary>
        /// <returns></returns>
        public Token GetValue()
        {
            if (IsTerminal)
                return token;
            else if (token.Content == "+" || token.Content == "-" || token.Content == "*" || token.Content == "/" || token.Content == "^" || token.Content == "%")
            {
                return Operations.BinaryOperation(childs[0].GetValue(), childs[1].GetValue(), token.Content);

            }
            else if (token.Content == "!")
            {
                return Operations.UnaryNegation(childs[0].GetValue());
            }

            else if (token.Content == "<=" || token.Content == ">=" || token.Content == "==" || token.Content == "!=" || token.Content == "<" || token.Content == ">" || token.Content == "&" || token.Content == "|")
            {
                return Operations.LogicOperation(childs[0].GetValue(), childs[1].GetValue(), token.Content);
            }

            else if (token.Content == "@")
            {
                return Operations.StringSum(childs[0].GetValue(), childs[1].GetValue());
            }
            else if (token.Type == "mix")
            {
                if (token.Content == "cpar")
                {
                    return childs[0].GetValue();
                }
                else if (token.Content == "let-in")
                {
                    return Operations.LetIn(token);
                }

                else if (token.Content == "if-else")
                {
                    return Operations.IfElse(token);
                }
            }
            else if (token.Type == "func")
            {
                foreach (Function f in Function.ActiveFunctions)
                {
                    if (f.name == token.Content)
                    {
                        return f.Call(token.exp[0]);
                    }

                }

                Error.errors.Add(new Error() { Type = "SEMANTIC ERROR: la función " + token.Content + " no está definida" });
                return null;


            }

            Error.errors.Add(new Error() { Type = "SEMANTIC ERROR: no se pudo retornar el valor del token: " + token });
            return null;

        }

        /// <summary>
        /// Un nodo terminal no tiene hijos.
        /// </summary>
        public bool IsTerminal = false;

        /// <summary>
        /// Token que representa al nodo actual
        /// </summary>
        /// <value></value>
        public Token token { get; set; }

        /// <summary>
        /// Array que contiene los posibles nodos hijos del nodo actual
        /// </summary>
        /// <value></value>
        public Node[]? childs { get; set; }

    }




}