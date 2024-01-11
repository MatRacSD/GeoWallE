using System.Data.Common;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace Compiler
{

    public class Parser
    {

        private TokenList tokenList; //tipo que contiene los tokens parseados
        private State state;


        public Parser(TokenList tokenList, State state)
        {
            this.tokenList = tokenList;
            this.state = state;
        }

        public Node Parse() //Parsea la tokenList
        {
            List<Node> nodes = new List<Node>();

            while (tokenList.HasMoreTokens())
            {

                nodes.Add(Statement());
                ExpectToken(TokenType.Semicolon);



            }


            return new BlockNode { statements = nodes };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Node Statement()
        {
            Token token = tokenList.Peek();
            switch (token.Type)
            {
                case TokenType.Import:
                    return Import();
                case TokenType.Let:
                    return LogicExpression();
                case TokenType.Draw:
                    return DrawExpression();
                case TokenType.Point:
                    return PointExpression();
                case TokenType.Circle:
                    return CircleExpression();
                case TokenType.Color:
                    return ColorExpression();
                case TokenType.Line:
                    return LineExpression();
                case TokenType.Ray:
                    return RayExpression();
                case TokenType.Arc:
                    return ArcExpression();
                case TokenType.Segment:
                    return SegmetnExpresion();
                case TokenType.OpenParenthesis:
                    return LogicExpression();
                case TokenType.Minus:
                    return LogicExpression();
                case TokenType.If:
                    return LogicExpression();
                case TokenType.Identifier:
                    return IdentifierExpression();
                case TokenType.Number:
                    return LogicExpression();
                case TokenType.GuionBajo:
                    return SequencDeclarationExpression();
                case TokenType.End:
                    ExpectToken(TokenType.End);
                    return new NullNode();
                case TokenType.OpenBrace:
                    return SequenceExpression();


                default:
                    throw new Exception("Unexpected token: " + token.Type);
            }
        }
        /// <summary>
        /// Parsea una expresionde tipo secuencia
        /// </summary>
        /// <returns></returns>

        private Node SequenceExpression()
        {
            ExpectToken(TokenType.OpenBrace);
            Sequence seq = new(false);
            while (Peek().Type != TokenType.CloseBrace && tokenList.HasMoreTokens())
            {
                UnaryExpressionNode exp = (UnaryExpressionNode)Expression();
                if(exp.IsValueAConstant())
                seq.Add(exp.obj);
                else seq.Add(exp.GetValue(state));
                if (Peek().Type != TokenType.CloseBrace)
                {
                    ExpectToken(TokenType.Comma);
                }
            }
            ExpectToken(TokenType.CloseBrace);
            return new UnaryExpressionNode( seq);
        }

        /// <summary>
        /// Parsea una expresion de tipo arco
        /// </summary>
        /// <returns></returns>
        private Node ArcExpression()
        {
            ExpectToken(TokenType.Arc);
            if (Peek().Type == TokenType.OpenParenthesis)
            {
                return FunctionCall("arc");
            }
            else
            {
                return new ArcDeclarationNode(ExpectToken(TokenType.Identifier).Value);
            }
        }

        /// <summary>
        /// Parsea una expresion de tipo segmento
        /// </summary>
        /// <returns></returns>
        private Node SegmetnExpresion()
        {
            ExpectToken(TokenType.Segment);
            if (Peek().Type == TokenType.OpenParenthesis)
            {
                return FunctionCall("segment");
            }
            else
            {
                return new SegmentDeclarationNode(ExpectToken(TokenType.Identifier).Value);
            }
        }

        /// <summary>
        /// Parsea una expresion de tipo identificador de nivel superior
        /// </summary>
        /// <returns></returns>
        private Node IdentifierExpression()
        {

            //xpectToken(TokenType.Identifier).Value;
            if (PeekNext().Type == TokenType.OpenParenthesis)
            {
                if (tokenList.IsThereAnEqual())
                {
                    return FunctionDeclaration();
                }
                else return LogicExpression();
            }
            else if (PeekNext().Type == TokenType.Comma)
            {
                return SequencDeclarationExpression();
            }
            else if (PeekNext().Type == TokenType.Equals)
            {
                return ConstantDeclaration();
            }
            else return LogicExpression();
        }
        /// <summary>
        /// Parsea una expresion de tipo declaracion de secuencia
        /// </summary>
        /// <returns></returns>
        private Node SequencDeclarationExpression()
        {

            SequencDeclarationNode seq = new();
            while (Peek().Type != TokenType.Equals && tokenList.HasMoreTokens())
            {
                if (Peek().Type == TokenType.GuionBajo)
                {
                    seq.constants.Add(ExpectToken(TokenType.GuionBajo));
                }
                else seq.constants.Add(ExpectToken(TokenType.Identifier));
                if (Peek().Type != TokenType.Equals)
                {
                    ExpectToken(TokenType.Comma);
                }
            }
            ExpectToken(TokenType.Equals);
            seq.body = Expression();
            return seq;
        }

        /// <summary>
        /// Parsea una expresion de tipo linea
        /// </summary>
        /// <returns></returns>
        private Node LineExpression()
        {
            ExpectToken(TokenType.Line);
            if (Peek().Type == TokenType.OpenParenthesis)
            {
                return FunctionCall("line");
            }
            else
            {
                return new LineDeclarationNode(ExpectToken(TokenType.Identifier).Value);
            }
        }
        /// <summary>
        /// Parsea una expresion de tipo color
        /// </summary>
        /// <returns></returns>

        private Node ColorExpression()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parsea una expresion de tipo circulo
        /// </summary>
        /// <returns></returns>
        private Node CircleExpression()
        {

            ExpectToken(TokenType.Circle);
            if (Peek().Type == TokenType.OpenParenthesis)
            {
                return FunctionCall("circle");
            }
            else
            {

                return new CircleDeclarationNode(ExpectToken(TokenType.Identifier).Value);
            }
        }

        /// <summary>
        /// Parsea una expresion de tipo punto
        /// </summary>
        /// <returns></returns>
        private Node PointExpression()
        {
            ExpectToken(TokenType.Point);
            Token tokenName = ExpectToken(TokenType.Identifier);


            return new PointDeclarationNode(tokenName.Value);
        }
        /// <summary>
        /// Parsea una expresion de tipo draw
        /// </summary>
        /// <returns></returns>
        private Node DrawExpression()
        {

            ExpectToken(TokenType.Draw);
            //Token tokenName = ExpectToken(TokenType.Identifier);
            Node toDraw = Expression();
            string figLabel = "";
            if (Peek().Type == TokenType.String)
            {
                figLabel = tokenList.NextToken().Value;


            }
            //ExpectToken(TokenType.Semicolon);

            return new DrawNode { figToDraw = toDraw, label = figLabel };
        }
        /// <summary>
        /// Parsea una expresion de tipo declaracion de funcion
        /// </summary>
        /// <returns></returns>
        private Node FunctionDeclaration()
        {

            string nameToken = ExpectToken(TokenType.Identifier).Value;

            if (state.Contains(nameToken))
            {
                throw new Exception("Redefinicion de una constante o funcion declarada");
            }


            ExpectToken(TokenType.OpenParenthesis);

            List<string> parameters = GetParameters();


            ExpectToken(TokenType.CloseParenthesis);

            ExpectToken(TokenType.Equals);


            Node body = Expression();



            return new FunctionDeclarationNode
            {
                Name = nameToken,
                Parameters = parameters,
                Body = body
            };
        }
        /// <summary>
        /// Parsea una expresion de tipo declaracion de constante
        /// </summary>
        /// <returns></returns>
        private Node ConstantDeclaration()
        {

            string nameToken = ExpectToken(TokenType.Identifier).Value;

            if (state.Contains(nameToken))
            {
                throw new Exception("Redefinicion de una constante o una función");
            }


            ExpectToken(TokenType.Equals);


            Node value = Expression();

            return new ConstantDeclarationNode
            {
                Name = nameToken,
                Value = value
            };

        }

        /// <summary>
        /// Parsea una expresion de tipo llamada de funcion
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        private Node FunctionCall(string name)
        {


            string funcName = name;
            List<Node> parametersNodes = new();
            ExpectToken(TokenType.OpenParenthesis);
            while (Peek().Type != TokenType.CloseParenthesis)
            {
                parametersNodes.Add(Expression());
                if (Peek().Type != TokenType.CloseParenthesis)
                {
                    ExpectToken(TokenType.Comma);
                }
            }
            ExpectToken(TokenType.CloseParenthesis);
            return new FunctionCallNode { Name = funcName, Arguments = parametersNodes };
        }

        /// <summary>
        /// Parsea una expresion de tipo llamada de constante
        /// </summary>
        /// <param name="name"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Node ConstantCall(string name, Token token)
        {
            return new UnaryExpressionNode( new ConstantCall() { Name = token.Value} );
        }

        /// <summary>
        /// Devuelve una excepcion si se introduce un token inesperado
        /// </summary>
        /// <param name="expectedType"></param>
        /// <returns></returns>

        private Token ExpectToken(TokenType expectedType)
        {
            Token token = tokenList.NextToken();
            if (token.Type != expectedType)
            {
                throw new Exception("Expected token of type " + expectedType + " and got " + token.Type);
            }
            return token;
        }
        /// <summary>
        /// Parsea una expresion
        /// </summary>
        /// <returns></returns>
        private Node Expression()
        {
            Token token = tokenList.Peek();
            switch (token.Type)
            {

                case TokenType.Let:
                    return LogicExpression();
                case TokenType.Line:
                    return LineExpression();
                case TokenType.Draw:
                    return DrawExpression();
                case TokenType.Minus:
                    return LogicExpression();
                case TokenType.If:
                    return LogicExpression();
                case TokenType.Identifier:
                    return LogicExpression();
                case TokenType.Number:
                    return LogicExpression();
                case TokenType.OpenParenthesis:
                    return LogicExpression();

                case TokenType.String:
                case TokenType.Point:
                    return PointExpression();
                case TokenType.Circle:
                    return CircleExpression();
                case TokenType.Segment:
                    return SegmetnExpresion();
                case TokenType.Ray:
                    return RayExpression();
                case TokenType.Arc:
                    return ArcExpression();
                case TokenType.OpenBrace:
                    return SequenceExpression();


                default:
                    throw new Exception("Unexpected token: " + token.Type);
            }
        }
        /// <summary>
        /// Parsea una expresion de tipo rayo
        /// </summary>
        /// <returns></returns>
        private Node RayExpression()
        {
            ExpectToken(TokenType.Ray);
            if (Peek().Type == TokenType.OpenParenthesis)
            {
                return FunctionCall("ray");
            }
            else
            {
                return new RayDeclarationNode(ExpectToken(TokenType.Identifier).Value);
            }
        }


        /// <summary>
        /// Parsea una expresion de tipo logica
        /// </summary>
        /// <returns></returns>
        private Node LogicExpression()
        {
            Node left = AdditiveExpression();

            while (tokenList.HasMoreTokens() && (Peek().Type == TokenType.EqualEqual || Peek().Type == TokenType.GreaterThanEqual || Peek().Type == TokenType.LessThanEqual || Peek().Type == TokenType.LessThan || Peek().Type == TokenType.GreaterThan))
            {
                Token op = tokenList.NextToken();
                Node right = AdditiveExpression();
                left = new BinaryOperationNode { Left = left, Operator = op.Type, Right = right };
            }

            return left;
        }
        /// <summary>
        /// Parsea una expresion de additiva
        /// </summary>
        /// <returns></returns>

        private Node AdditiveExpression()
        {
            Node left = MultiplicativeExpression();

            while (tokenList.HasMoreTokens() && (Peek().Type == TokenType.Plus || Peek().Type == TokenType.Minus))
            {
                Token op = tokenList.NextToken();
                Node right = MultiplicativeExpression();
                left = new BinaryOperationNode { Left = left, Operator = op.Type, Right = right };
            }

            return left;
        }
        /// <summary>
        /// Parsea una expresion de tipo multiplicativa
        /// </summary>
        /// <returns></returns>
        private Node MultiplicativeExpression()
        {
            Node left = PrimaryExpression();

            while (tokenList.HasMoreTokens() && (Peek().Type == TokenType.Multiply || Peek().Type == TokenType.Divide || Peek().Type == TokenType.DivideRest))
            {
                Token op = tokenList.NextToken();
                Node right = PrimaryExpression();
                left = new BinaryOperationNode { Left = left, Operator = op.Type, Right = right };
            }

            return left;
        }
        /// <summary>
        /// Parsea una expresion de tipo primaria
        /// </summary>
        /// <returns></returns>
        private Node PrimaryExpression()
        {
            if (Peek().Type == TokenType.OpenParenthesis)
            {
                ExpectToken(TokenType.OpenParenthesis);  // Consume '('
                Node inner = Expression();
                ExpectToken(TokenType.CloseParenthesis);  // Consume ')'
                return inner;
            }
            else
            {
                return Last();
            }

        }
        /// <summary>
        /// Parsea las ultimas expresiones posibles
        /// </summary>
        /// <returns></returns>

        private Node Last()
        {
            Token token = tokenList.NextToken();
            if (token.Type == TokenType.Minus)
            {
                Node operand = Last();
                return new UnaryOperationNode { Operator = TokenType.Minus, Operand = operand };
            }
            else if (token.Type == TokenType.Number)
            {
                return new UnaryExpressionNode( new Number( double.Parse(token.Value) ));
            }
            else if (token.Type == TokenType.Let)
                return LetInExpression();
            else if (token.Type == TokenType.If)
                return IfElseExpression();
            else if (token.Type == TokenType.Identifier)
            {
                if (Peek().Type == TokenType.OpenParenthesis)
                {

                    return FunctionCall(token.Value);
                }

                else return ConstantCall(token.Value, token);
            }

            else
            {
                throw new Exception("Expected number or identifier and got " + token.Type);
            }
        }
        /// <summary>
        /// Parsea una expresion de tipo import
        /// </summary>
        /// <returns></returns>

        private Node Import()
        {


            Token token = tokenList.NextToken();
            if (Peek().Type == TokenType.String)
            {

                return new ImportNode { Path = ExpectToken(TokenType.String).Value };
            }
            else throw new Exception("Expected string afther import and got: " + token.Type);
        }
        /// <summary>
        /// Parsea una expresion de tipo let-in 
        /// </summary>
        /// <returns></returns>
        private Node LetInExpression()
        {


            List<Node> declarations = new List<Node>();
            while (Peek().Type != TokenType.In)
            {
                declarations.Add(Statement());
                ExpectToken(TokenType.Semicolon);
            }

            ExpectToken(TokenType.In);
            Node body = Expression();


            return new LetInNode { Declarations = declarations, Body = body };
        }
        /// <summary>
        /// Parsea una expresion de tipo if-else
        /// </summary>
        /// <returns></returns>

        private Node IfElseExpression()
        {
            //ExpectToken(TokenType.If);
            Node condition = Expression();

            ExpectToken(TokenType.Then);
            Node thenBranch = Expression();

            ExpectToken(TokenType.Else);
            Node elseBranch = Expression();



            return new IfElseNode { Condition = condition, ThenBranch = thenBranch, ElseBranch = elseBranch };
        }
        /// <summary>
        /// Guarda los parametros de una funcion
        /// </summary>
        /// <returns></returns>
        private List<string> GetParameters()
        {
            List<string> parameters = new();
            while (Peek().Type != TokenType.CloseParenthesis)
            {
                parameters.Add(ExpectToken(TokenType.Identifier).Value);
                if (Peek().Type != TokenType.CloseParenthesis && Peek().Type != TokenType.CloseParenthesis)
                {
                    ExpectToken(TokenType.Comma);
                }
            }
            return parameters;
        }
        /// <summary>
        /// Devuelve el token actual sin consumirlo
        /// </summary>
        /// <returns></returns>
        private Token Peek()
        {
            return tokenList.Peek();
        }

        /// <summary>
        /// Devuelve el token siguientesin consumirlo
        /// </summary>
        /// <returns></returns>

        private Token PeekNext()
        {
            return tokenList.PeekNext();
        }
    }
}
