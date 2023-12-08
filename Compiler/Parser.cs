

using System.Drawing;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Compiler
{

    public static class Parser
    {

        public static void LevelsInit()
        {
            levels.Add(LV0);
            levels.Add(LV1);
            levels.Add(LV2);
            levels.Add(LV3);
            levels.Add(LV4);
            levels.Add(LV5);
            levels.Add(LV6);
            levels.Add(LV7);

        }

        /// <summary>
        /// Realiza una primera revision del codigo para obtener los nodos raiz de los AST a construir
        /// </summary>
        /// <param name="state">Contiene el ambito del programa que se ejecuta</param>
        /// <param name="tokens"></param>

        /// <returns>Los posibles nodos raiz o nodos de errores</returns>
        public static List<Node> Parse(this State state, List<Token> tokens)
        {

            int parentesisCount = 0;
            bool isLetInOpen = false;
            List<Node> mainNodes = new List<Node>(); //Representa los nodos raíz del programa
            List<Token> auxTokenList = new List<Token>();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == "Parnt")
                {
                    if (tokens[i].Content == "(") parentesisCount += 1;

                    else parentesisCount -= 1;
                    auxTokenList.Add(tokens[i]);

                }
                else if (parentesisCount < 0)
                {
                    List<Node> errors = new List<Node>();
                    errors.Add(new ErrorNode { Error = "uso incorrecto de parentesis" });
                    return errors;
                }
                else if (parentesisCount == 0)
                {
                    if (isLetInOpen)
                    {
                        auxTokenList.Add(tokens[i]);
                        if (tokens[i].Content == "in")
                        {

                            isLetInOpen = false;
                        }

                    }
                    else if (tokens[i].Content == "let")
                    {
                        auxTokenList.Add(tokens[i]);
                        isLetInOpen = true;
                    }
                    else if (tokens[i].Content == ";")
                    {
                        mainNodes.Add(new MainNode(auxTokenList));
                        auxTokenList = new();
                    }
                    else auxTokenList.Add(tokens[i]);
                }
                else auxTokenList.Add(tokens[i]);
            }

            //Se chequean algunos posibles errores
            if (parentesisCount != 0)
            {
                mainNodes = new List<Node>() { new ErrorNode() { Error = "Parentesis sin cerrar" } };
                return mainNodes;
            }

            else if (isLetInOpen)
            {
                mainNodes = new List<Node>() { new ErrorNode() { Error = "falta in en let-in" } };
                return mainNodes;
            }
            else if (auxTokenList.Count > 0)
            {
                mainNodes = new List<Node>() { new ErrorNode() { Error = "falta ;" } };
                return mainNodes;
            }


            List<Token> unasignedTokens = new();
            bool isParentesisOpen = false;
            List<Node> nodes = new();

            foreach (MainNode node in mainNodes)
            {
                if (node.GetType() == "MainNode")
                {
                    
                    for (int i = 0; i < node.tokenList.Count; i++)
                    {

                        if (node.tokenList[i].Type == "Parnt")
                        {
                            if (node.tokenList[i].Content == "(")
                            {
                                parentesisCount += 1;
                                if (!isParentesisOpen)
                                {
                                    isParentesisOpen = true;
                                    if(unasignedTokens.Count > 0){
                                    nodes.Add(new UnparsedNode() { tokenList = unasignedTokens.Clone() });
                                    unasignedTokens = new();}
                                    continue;
                                }
                                unasignedTokens.Add(node.tokenList[i]);


                            }
                            else parentesisCount -= 1;
                            if (parentesisCount < 0)
                            {
                                return new List<Node>() { new ErrorNode() { Error = "Parentesis sin cerrar" } };
                            }
                            else if (parentesisCount == 0)
                            {
                                isParentesisOpen = false;
                                
                                nodes.Add(new ParentesisNode() { tokenList = unasignedTokens.Clone() });
                                unasignedTokens = new();
                                continue;
                            }
                            unasignedTokens.Add(node.tokenList[i]);
                        }
                        else unasignedTokens.Add(node.tokenList[i]);
                    }
                    if (isParentesisOpen)
                    {
                        return new List<Node>() { new ErrorNode() { Error = "Parentesis sin cerrar" } };

                    }
                    else if (unasignedTokens.Count > 0) nodes.Add(new UnparsedNode() { tokenList = unasignedTokens.Clone() });
                    if (nodes.Count > 0)
                    {
                        //throw new Exception(nodes.Count.ToString());
                        ((MainNode)node).nodes = nodes;

                    }
                    nodes = new();
                    unasignedTokens.Clear();
                }

            }



            return mainNodes;
        }

        public static Node ParseLevel(List<Node> inputNodes, int parseLevel, State state)
        {
            
            string UN = "UnparsedNode";
            string PN = "ParentesisNode";
            //throw new Exception("asdasdsda" + inputNodes[0].GetType() + ">>>" + inputNodes[0].tokenList.Count);
            List<Node> nodes = new();
            if(inputNodes == null)
            {
                return new NullNode();
            }
            for (int i = 0; i < inputNodes.Count; i++)
            {
                nodes.Add(inputNodes[i]);
            }
            List<Node> nodes1 = new();
            for (int k = 0; k < 8; k++) // k es el parseLevel actual
            {



                if (k == 0) //Se parsea el primer nivel (funciones, declaraciones, if-else , let-in)
                {
                    {
                        for (int i = 0; i < nodes.Count; i++) //iteracion por cada nodo
                        {
                            if (nodes[i].GetType() == "ErrorNode") { nodes1.Add(nodes[i]); continue; }
                            if (nodes[i].GetType() == UN )
                            {
                                List<Token> unusedToknes = new();
                                for (int j = 0; j < nodes[i].tokenList.Count; j++) //iteracion por cada token del nodo
                                {

                                    if (nodes[i].tokenList[j].Type == "iden") //Si el nodo actual es un identificador
                                    {
                                        if (state.IsReserved(nodes[i].tokenList[j].Content)) //Si es una palabra reservada
                                        {

                                            string reservedWord = nodes[i].tokenList[j].Content;
                                            if (nodes[i].tokenList[j].Content == "draw")
                                            {
                                                List<Token> tokensToDraw = new();
                                                for (int p = 1; p < nodes[i].tokenList.Count; p++)
                                                {
                                                    tokensToDraw.Add(nodes[i].tokenList[p]);
                                                }
                                                if (tokensToDraw.Count == 0) return new ErrorNode() { Error = "Se esperaba expresion to draw" };
                                                UnparsedNode unparsedNode = new() { tokenList = tokensToDraw.Clone() };
                                                DrawNode drawNode = new() { Childs = new Node[] { unparsedNode } };
                                                drawNode.Parse(state);
                                                nodes1.Add(drawNode);
                                                break;
                                            }
                                            else if (reservedWord == "if") //Se verifica si es el inicio de un nodo if-else
                                            {
                                                IfElseNode ifelseNode = new IfElseNode() { Childs = new Node[3] };
                                                if (j != nodes[i].tokenList.Count - 1 || nodes.Count - 1 == i || nodes[i + 1].GetType() != "ParentesisNode")
                                                {
                                                    return new ErrorNode() { Error = "falta cuerpo del if en expresion if-else" };
                                                }
                                                ifelseNode.Childs[0] = nodes[i + 1];
                                                if (nodes.Count - 1 <= i + 2)
                                                {
                                                    return new ErrorNode() { Error = "falta cuerpo then del if en expresion if-else" };
                                                }
                                                List<Token> thenBody = new();
                                                List<Token> elseBody = new();
                                                int ifelsePart = 0;
                                                for (int l = 0; l < nodes[i + 2].tokenList.Count; l++)
                                                {
                                                    if (nodes[i + 2].tokenList[0].Content != "then")
                                                    {
                                                        return new ErrorNode() { Error = "falta palabra reservada then  en expresion if-else" };

                                                    }
                                                    else if (ifelsePart == 0)
                                                    {
                                                        ifelsePart = 1;
                                                        continue;
                                                    }
                                                    else if (ifelsePart == 1)
                                                    {
                                                        if (nodes[i + 2].tokenList[l].Content == "else")
                                                        {
                                                            ifelsePart = 2;
                                                            continue;
                                                        }
                                                        else thenBody.Add(nodes[i + 2].tokenList[l]);
                                                    }
                                                    else if (ifelsePart == 2)
                                                    {
                                                        elseBody.Add(nodes[i + 2].tokenList[l]);
                                                    }
                                                }
                                                if (ifelsePart != 2)
                                                {
                                                    return new ErrorNode() { Error = "falta palabra reservada else  en expresion if-else" };

                                                }
                                                ifelseNode.Childs[1] = new UnparsedNode() { tokenList = thenBody.Clone() };
                                                ifelseNode.Childs[2] = new UnparsedNode() { tokenList = elseBody.Clone() };
                                                ifelseNode.Parse(state);
                                                nodes1.Add(ifelseNode);
                                                i = i + 2;
                                                break;
                                            }
                                            else if (reservedWord == "else") return new ErrorNode() { Error = "uso incorrecto de reservada else  en expresion if-else" };
                                            else if (reservedWord == "then") return new ErrorNode() { Error = "uso incorrecto de palabra reservada then  en expresion if-else" };
                                            else if (reservedWord == "let")
                                            {
                                                List<Token> letinTokens = new();
                                                LetInNode letInNode = new LetInNode() { Identificadores = new() };
                                                for (int o = j + 1; o < nodes[i].tokenList.Count; o++)
                                                {
                                                    letinTokens.Add(nodes[i].tokenList[o]);
                                                }
                                                List<Node> letMainNodes = Parse(state, letinTokens);
                                                if (letMainNodes.Last().tokenList.Count == 0)
                                                {
                                                    return new ErrorNode() { Error = "falta palabra reservada in  en expresion let-in" };
                                                }
                                                else if (letMainNodes.Last().tokenList[0].Content != "in")
                                                {
                                                    return new ErrorNode() { Error = "falta palabra reservada in  en expresion let-in" };

                                                }
                                                int lastletMainNode = letMainNodes.Count - 1;
                                                for (int o = 0; o < lastletMainNode; o++)
                                                {
                                                    if (letMainNodes[o].tokenList.Count < 3)
                                                    {
                                                        return new ErrorNode() { Error = "falta cuerpo del let" };

                                                    }
                                                    else if (letMainNodes[o].tokenList[0].Type != "iden")
                                                    {
                                                        return new ErrorNode() { Error = "Se esperaba identificador  en expresion let-in" };

                                                    }
                                                    else if (letMainNodes[o].tokenList[1].Content != "=")
                                                    {
                                                        return new ErrorNode() { Error = "Se esperaba simbolo de asignación  en expresion let-in" };

                                                    }

                                                    else
                                                    {
                                                        Token assignationTokens = letMainNodes[o].tokenList[0].Clone();
                                                        UnparsedNode node = new();

                                                        for (int h = 2; h < letMainNodes[o].tokenList.Count; h++)
                                                        {
                                                            node.tokenList.Add(letMainNodes[o].tokenList[h]);
                                                        }
                                                        if (letInNode.Identificadores.ContainsKey(letMainNodes[o].tokenList[0]))
                                                        {
                                                            return new ErrorNode() { Error = "Reasignación de la constante asignada: " + letMainNodes[o].tokenList[0] };

                                                        }
                                                        else letInNode.Identificadores.Add(assignationTokens, node);
                                                    }

                                                }
                                                letInNode.Childs = new Node[1];
                                                letInNode.Childs[0] = letMainNodes.Last();
                                                letInNode.Parse(state);
                                                nodes1.Add(letInNode);
                                                break;
                                            }
                                            else if (reservedWord == "point")
                                            {
                                                PointNode point;
                                                if (nodes[i].tokenList.Count < 2) return new ErrorNode() { Error = "Se esperaba identificador en declaracion de point" };
                                                else if (nodes[i].tokenList[1].Type != "iden") return new ErrorNode() { Error = "Se esperaba identificador en declaracion de point" };
                                                else if (nodes[i].tokenList.Count == 2)
                                                {
                                                    if (state.pointsDeclared.Contains(nodes[i].tokenList[1].Content))
                                                    {
                                                        return new ErrorNode() { Error = "Redefinición del punto: " + nodes[i].tokenList[1].Content };
                                                    }
                                                    point = new PointNode(nodes[i].tokenList[1].Content);

                                                    nodes1.Add(point);
                                                    state.pointsDeclared.Add(nodes[i].tokenList[1].Content);
                                                    break;
                                                }
                                                else if (nodes[i].tokenList.Count > 3) return new ErrorNode() { Error = "Se esperaba expresion en parentesis" };
                                                else
                                                {
                                                    if (nodes.Count == i + 1) return new ErrorNode() { Error = "Se esperaba expresion entre parentesis" };
                                                    else if (nodes.Count == i + 2) return new ErrorNode() { Error = "Declaración de punto inválida" };
                                                    else if (nodes[i + 1].GetType() != "ParentesisNode") return new ErrorNode() { Error = "Se esperaba expresion entre parentesis" };
                                                    else
                                                    {
                                                        throw new NotImplementedException("La asignacion de punto no esta implementada");
                                                        /// Aqui va la implementación de la asignación de punto
                                                    }
                                                }


                                            }
                                            else if(reservedWord == "line")
                                            {
                                                LineNode lineNode;
                                                if (nodes[i].tokenList.Count - 1< j + 1) return new ErrorNode() { Error = "Se esperaba identificador en declaracion de point" };
                                                else if (nodes[i].tokenList[j + 1].Type != "iden") return new ErrorNode() { Error = "Se esperaba identificador en declaracion de point" };
                                                if(!(nodes[i].tokenList.Count - 1 < j + 2) )
                                                {
                                                    if(nodes[i].tokenList[j + 2].Content == "=")
                                                    {
                                                        
                                                    }
                                                }
                                            }




                                        }
                                        //Aqui termina la revision de las palabras reservadas
                                        //Ahora se proceden a construir nodos de funciones , constantes e identificadores
                                        else
                                        {

                                            //Se revisa que es lo que viene despues
                                            if (nodes.Count > i + 1)
                                            {
                                                if (nodes[i].tokenList.Count == j + 1)
                                                {

                                                    if (nodes[i + 1].GetType() == PN) // Si es una llamada o declaracion de funcion
                                                    {

                                                        if (state.ConstantExist(nodes[i].tokenList[j].Content))
                                                        {
                                                            return new ErrorNode() { Error = "Ya existe una constante con ese nombre: " + nodes[i].tokenList[j].Content };
                                                        }
                                                        else if (state.FunctionExist(nodes[i].tokenList[j].Content)) //Se considera llamada de funcion definida
                                                        {

                                                            if (nodes.Count > i + 2) //Se revisa que no se esté redefiniendo
                                                            {
                                                                if (nodes[i + 2].GetType() == UN)
                                                                {
                                                                    if (nodes[i + 2].tokenList.Count > 0)
                                                                    {
                                                                        if (nodes[i + 2].tokenList[0].Content == "=")
                                                                            return new ErrorNode() { Error = "redefinición de la función: " + nodes[i].tokenList[j].Content };



                                                                    }
                                                                }
                                                            }


                                                        }

                                                        else if (nodes.Count > i + 2) //Definicion de funcion
                                                        {

                                                            FunctionNode function = new FunctionNode(nodes[i].tokenList[j].Content);
                                                            UnparsedNode functionBody = new();
                                                            List<Identificador> funcParams = new();
                                                            bool expectedId = true; //Verdadero si se espera identificador y falso si se espera coma
                                                            if (nodes[i + 2].GetType() == UN)
                                                            {
                                                                if (nodes[i + 2].tokenList.Count > 0)
                                                                {
                                                                    if (nodes[i + 2].tokenList[0].Content == "=") //Se guarda el cuerpo de la funcion
                                                                    {

                                                                        for (int g = 1; g < nodes[i + 2].tokenList.Count; g++)
                                                                        {
                                                                            functionBody.tokenList.Add(nodes[i + 2].tokenList[g]);
                                                                        }

                                                                        for (int g = 0; g < nodes[i + 1].tokenList.Count; g++)
                                                                        {
                                                                            if (expectedId)
                                                                            {
                                                                                if (nodes[i + 1].tokenList[g].Type != "iden")
                                                                                {
                                                                                    return new ErrorNode() { Error = "se esperaba identificador en declaracion de funcion" };
                                                                                }
                                                                                else funcParams.Add(new Identificador(nodes[i + 1].tokenList[g].Content));
                                                                                expectedId = false;
                                                                                continue;
                                                                            }
                                                                            else if (nodes[i + 1].tokenList[g].Content != ",")
                                                                            {
                                                                                return new ErrorNode() { Error = "Se esperaba ," };
                                                                            }
                                                                            expectedId = true;
                                                                        }

                                                                        function.SetFunctionBody(funcParams, functionBody);

                                                                        state.activeFunctions.Add(function);
                                                                        nodes1.Add(function);
                                                                        i += 2;

                                                                        break; // Se guarda como definicion de funcincion
                                                                    }

                                                                    //return new ErrorNode(){Error = "redefinición de la función: " + nodes[i].tokenList[j].Content};
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            FunctionNode functionNode = new FunctionNode(nodes[i].tokenList[j].Content);
                                                            functionNode.SetParameters((ParentesisNode)nodes[i + 1]);
                                                            nodes1.Add(functionNode);
                                                            i += 1;
                                                            if (unusedToknes.Count > 0)
                                                            {
                                                                nodes1.Add(new UnparsedNode() { tokenList = unusedToknes.Clone() });
                                                            }
                                                            break; ; //Se guarda como una llamada de funcion
                                                        }
                                                    }
                                                    else    //Entonces es una llamada de constante , declaracion de constante o identificador
                                                    {

                                                        if (state.FunctionExist(nodes[i].tokenList[j].Content))
                                                        {
                                                            return new ErrorNode() { Error = "Ya existe una function con ese nombre: " + nodes[i].tokenList[j].Content };
                                                        }
                                                        else if (state.ConstantExist(nodes[i].tokenList[j].Content))
                                                        {
                                                            nodes1.Add(state.GetConstantNode(nodes[i].tokenList[j].Content));
                                                            break;

                                                        }
                                                        else if (nodes[i].tokenList.Count > j + 1) ///Declaración de constante 
                                                        {

                                                            if (nodes[i].tokenList[j + 1].Content == "=")
                                                            {
                                                                if (state.ConstantExist(nodes[i].tokenList[j].Content))
                                                                {
                                                                    return new ErrorNode() { Error = "Ya existe una constante con ese nombre: " + nodes[i].tokenList[j].Content };
                                                                }

                                                                ConstantNode constantNode = new ConstantNode(nodes[i].tokenList[j].Content);
                                                                List<Token> tokens = new();
                                                                List<Node> nodes2 = new();
                                                                for (int g = i + 1; g < nodes.Count; g++)
                                                                {
                                                                    nodes2.Add(nodes[g]);
                                                                }
                                                                for (int g = j + 2; g < nodes[i].tokenList.Count; g++)
                                                                {
                                                                    tokens.Add(nodes[i].tokenList[g]);
                                                                }

                                                                UnparsedNode unparsedNode = new() { tokenList = tokens.Clone() };
                                                                nodes2.Insert(0, unparsedNode);
                                                                constantNode.SetBody(nodes2, state);
                                                                nodes1.Add(constantNode);
                                                                i = nodes.Count() - 1;
                                                                state.constantNodes.Add(constantNode);

                                                                break;
                                                            }



                                                        }
                                                        nodes1.Add(new Identificador(nodes[i].tokenList[j].Content));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //Entonces es una llamada de constante

                                                if (state.FunctionExist(nodes[i].tokenList[j].Content))
                                                {
                                                    return new ErrorNode() { Error = "Ya existe una function con ese nombre: " + nodes[i].tokenList[j].Content };
                                                }
                                                else if (state.ConstantExist(nodes[i].tokenList[j].Content))
                                                {
                                                    nodes1.Add(state.GetConstantNode(nodes[i].tokenList[j].Content));
                                                    break;

                                                }
                                                else if (nodes[i].tokenList.Count > j + 1) ///Declaración de constante 
                                                {

                                                    if (nodes[i].tokenList[j + 1].Content == "=")
                                                    {
                                                        if (state.ConstantExist(nodes[i].tokenList[j].Content))
                                                        {
                                                            return new ErrorNode() { Error = "Ya existe una constante con ese nombre: " + nodes[i].tokenList[j].Content };
                                                        }
                                                        ConstantNode constantNode = new ConstantNode(nodes[i].tokenList[j].Content);
                                                        List<Token> tokens = new();
                                                        List<Node> nodes2 = new();
                                                        for (int g = i + 1; g < nodes.Count; g++)
                                                        {
                                                            nodes2.Add(nodes[g]);
                                                        }
                                                        for (int g = j + 2; g < nodes[i].tokenList.Count; g++)
                                                        {
                                                            tokens.Add(nodes[i].tokenList[g]);
                                                        }

                                                        UnparsedNode unparsedNode = new() { tokenList = tokens.Clone() };
                                                        nodes2.Insert(0, unparsedNode);
                                                        constantNode.SetBody(nodes2, state);
                                                        nodes1.Add(constantNode);
                                                        i = nodes.Count() - 1;
                                                        state.constantNodes.Add(constantNode);
                                                        break;
                                                    }



                                                }
                                                if (unusedToknes.Count > 0)
                                                {
                                                    nodes1.Add(new UnparsedNode() { tokenList = unusedToknes.Clone() });
                                                }
                                                nodes1.Add(new Identificador(nodes[i].tokenList[j].Content));
                                                unusedToknes = new();

                                            }
                                        }

                                    }
                                    unusedToknes.Add(nodes[i].tokenList[j]);

                                }
                                if (unusedToknes.Count > 0)
                                {
                                    nodes1.Add(new UnparsedNode() { tokenList = unusedToknes.Clone() });
                                }
                            }
                            else nodes1.Add(nodes[i]);                       
                        }

                     
                        nodes = nodes1;
                        nodes1 = new();
                        
                    }
                }
                else if(k < 7)
                {
                   //throw new Exception("REPINGAAAA -->>" + nodes.Count  + " >->" + nodes[0].GetType());
                    bool nodeAdded = false;
                    List<Node> auxiliarNodeList = new();
                    List<Token> auxiliarTokensList = new();
                    BinaryOperationNode? binaryOP = null;
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        if (nodes[i].GetType() == "ParentesisNode")
                        {
                            //throw new Exception("chupachupa -->>" + nodes.Count + " >-<" + nodes[i].tokenList.Count );
                            auxiliarNodeList.Add(ParseLevel(new List<Node> { new UnparsedNode() { tokenList = nodes[i].tokenList.Clone() } }, 1, state));
                            //throw new Exception("amaterasu -->> " + auxiliarNodeList.Last().GetType());
                            continue;
                        }
                        else if (nodes[i].GetType() == "UnparsedNode")
                        {
                            for (int j = 0; j < nodes[i].tokenList.Count; j++)
                            {
                                 
                                if (levels[k].Contains(nodes[i].tokenList[j].Content))
                                {
                                     
                                    
                                    if (binaryOP == null)
                                    {
                                        nodeAdded = true;
                                        binaryOP = new();
                                        if (auxiliarTokensList.Count > 0)
                                        {
                                            auxiliarNodeList.Add(new UnparsedNode() { tokenList = auxiliarTokensList.Clone() });
                                            auxiliarTokensList = new();
                                        }
                                        binaryOP = new() { LeftChild = auxiliarNodeList };
                                        auxiliarNodeList = new();
                                    }
                                    else
                                    {
                                        nodeAdded = true;
                                        binaryOP.RightChild = auxiliarNodeList;
                                        auxiliarNodeList = new();
                                        BinaryOperationNode auxiliarBinaryOP = binaryOP;
                                        binaryOP = new() { LeftChild = new List<Node>(){auxiliarBinaryOP} };
                                    }
                                }
                                else
                                {
                                    
                                    auxiliarTokensList.Add(nodes[i].tokenList[j]);
                                }
                            }
                            
                        }
                        if(!nodeAdded)
                        {
                            auxiliarNodeList.Add(nodes[i]);
                        }
                        nodeAdded = false;
                        
                    }
                    if(binaryOP != null)
                    {
                        binaryOP.RightChild = auxiliarNodeList;
                        binaryOP.Parse(state);
                        auxiliarNodeList = new();
                        nodes1.Add(binaryOP);
                    }
                    else 
                    {
                        if(auxiliarNodeList.Count > 0)
                        {
                            
                            foreach (Node item in auxiliarNodeList)
                            {
                                nodes1.Add(item);
                            }
                        }
                    }

                    nodes = nodes1;
                        nodes1 = new();
                }

                else if(k == 7)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        if(nodes[i].GetType() == UN)
                        {
                            
                            if(nodes[i].tokenList != null && nodes[i].tokenList.Count == 1)
                            {
                                if(nodes[i].tokenList[0].Type == "number")
                                {
                                    nodes1.Add(new NumberNode(nodes[i].tokenList[0]));
                                }
                            }

                        }
                    }
                }
            }
            if (!(nodes.Count > 1 || nodes.Count == 0))
            {
                return nodes[0];
            }
            else if(nodes.Count > 1)
            {
                 string errors = "";
                 foreach (Node item in nodes)
                 {
                    errors += "nodo -->>"+ item.GetType() + "\n";
                 }
                 return new ErrorNode() { Error = errors};
            }
            return new ErrorNode() { Error = "Undhandled Exception At Parsing 0 elements" };


        }




        ///
        /// ////////////////////////////////////////////////////////////////////////////
        ///         /// <summary>
        /// Crea un árbol de tipos Node a partir de la lista de tokens que se le pasa como parámetro.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="startlevel"></param>
        /// <returns></returns>
        public static NodeObsolete ParseObsolete(List<Token> tokens, int startlevel)
        {


            return ParseLevel(tokens, levels[startlevel - 1], startlevel);

        }
        /// <summary>
        /// Parsea una lista de tokens según el nivel que se le haya pasado
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="lvls"></param>
        /// <param name="clevel"></param>
        /// <returns></returns>
        public static NodeObsolete ParseLevel(List<Token> tokens, List<string> lvls, int clevel)
        {
            List<Token> aux_token_list = new List<Token>();
            NodeObsolete? cNode = null;




            if (clevel == 2)
            {
                if (tokens[0].Content == "!")
                {
                    NodeObsolete node = new NodeObsolete { token = tokens[0], childs = new NodeObsolete[1] };
                    tokens.RemoveAt(0);
                    node.childs[0] = ParseObsolete(tokens, 3);
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
                    return new NodeObsolete() { token = tokens[0], IsTerminal = true };
                }

                else if (tokens[0].Content == "cpar")
                {
                    NodeObsolete node = new NodeObsolete() { token = tokens[0], childs = new NodeObsolete[1] };
                    node.childs[0] = ParseObsolete(tokens[0].exp, 1);
                    return node;

                }

                else if (tokens[0].Content == "let-in")
                {
                    NodeObsolete node = new NodeObsolete() { token = tokens[0] };
                    return node;
                }

                else if (tokens[0].Content == "if-else")
                {
                    NodeObsolete node = new NodeObsolete() { token = tokens[0] };
                    return node;
                }
                else if (tokens[0].Type == "func")
                {
                    NodeObsolete node = new NodeObsolete() { token = tokens[0] };
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
                        NodeObsolete node = new NodeObsolete() { token = tokens[i], childs = new NodeObsolete[2] };
                        node.childs[0] = ParseObsolete(aux_token_list, clevel + 1);
                        aux_token_list = new List<Token>();
                        cNode = node;
                    }
                    else
                    {
                        cNode.childs[1] = ParseObsolete(aux_token_list, clevel + 1);
                        aux_token_list = new List<Token>();
                        NodeObsolete aux_node = cNode;
                        cNode = new NodeObsolete() { token = tokens[i], childs = new NodeObsolete[2] };
                        cNode.childs[0] = aux_node;
                    }
                }
                else aux_token_list.Add(tokens[i]);


            }
            if (cNode != null)
            {
                cNode.childs[1] = ParseObsolete(aux_token_list, clevel + 1);
                return cNode;
            }

            else return ParseObsolete(tokens, clevel + 1);
        }









        /// /
        /// /////////////////////
        ///         /// ///////////////////////////////////////////////////////
        /// ////////////////////////
        /// ///////////////////////////////////
        static List<List<string>> levels = new List<List<string>>();
        //static List<string> LV1 = new List<string> {"+"};

        static List<string> LV1 = new List<string> { "&", "|" };
        static List<string> LV2 = new List<string> { "!" };
        static List<string> LV3 = new List<string> { "<=", ">=", "==", "!=", "<", ">", };
        static List<string> LV4 = new List<string> { "+", "-", "@" };
        static List<string> LV5 = new List<string> { "*", "/", "%" };
        static List<string> LV6 = new List<string> { "^" };
        static List<string> LV7 = new List<string> { "-" };
        static List<string> LV0 = new List<string> { "let", "print", "number", "cpar", "bool", "if-else", "strings", "func" };





    }

    /// <summary>
    /// Representa un nodo
    /// </summary>
    public class NodeObsolete
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
        public NodeObsolete[]? childs { get; set; }

    }




}