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

        public Node Parse()
        {
            List<Node> nodes = new List<Node>();

            while (tokenList.HasMoreTokens())
            {

                nodes.Add(Statement());
                ExpectToken(TokenType.Semicolon);



            }

            // This example assumes that the top level of your program is a sequence of statements.
            // You may need to adjust this if your language has a different structure.
            return new BlockNode { statements = nodes };
        }

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

                // Handle other types of statements...
                default:
                    throw new Exception("Unexpected token: " + token.Type);
            }
        }

        private Node SequenceExpression()
        {
            ExpectToken(TokenType.OpenBrace);
            SequenceNode seq = new();
            while (Peek().Type != TokenType.CloseBrace && tokenList.HasMoreTokens())
            {
                seq.Add(Expression(), state);
                if (Peek().Type != TokenType.CloseBrace)
                {
                    ExpectToken(TokenType.Comma);
                }
            }
            ExpectToken(TokenType.CloseBrace);
            return seq;
        }


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


        private Node ColorExpression()
        {
            throw new NotImplementedException();
        }


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


        private Node PointExpression()
        {
            ExpectToken(TokenType.Point);
            Token tokenName = ExpectToken(TokenType.Identifier);


            return new PointDeclarationNode(tokenName.Value);
        }

        private Node DrawExpression()
        {
            //throw new Exception("amaterasu");
            ExpectToken(TokenType.Draw);
            Token tokenName = ExpectToken(TokenType.Identifier);
            string figLabel = "";
            if (Peek().Type == TokenType.String)
            {
                figLabel = tokenList.NextToken().Value;


            }
            //ExpectToken(TokenType.Semicolon);
            return new DrawNode { figToDraw = tokenName.Value, label = figLabel };
        }
        private Node FunctionDeclaration()
        {
            // Expect an identifier token (the function name)
            string nameToken = ExpectToken(TokenType.Identifier).Value;
            //throw new Exception("AMATERASI");
            if (state.Contains(nameToken))
            {
                throw new Exception("Redefinicion de una constante o funcion declarada");
            }

            // Expect an open parenthesis token
            ExpectToken(TokenType.OpenParenthesis);

            List<string> parameters = GetParameters();

            // Expect a close parenthesis token
            ExpectToken(TokenType.CloseParenthesis);

            ExpectToken(TokenType.Equals);
            // Expect an open brace token
            // ExpectToken(TokenType.OpenBrace);

            Node body = Expression();

            // Expect a close brace token
            //ExpectToken(TokenType.CloseBrace);

            return new FunctionDeclarationNode
            {
                Name = nameToken,
                Parameters = parameters,
                Body = body
            };
        }

        private Node ConstantDeclaration()
        {
            // Expect an identifier token (the constant name)
            string nameToken = ExpectToken(TokenType.Identifier).Value;

            if (state.Contains(nameToken))
            {
                throw new Exception("Redefinicion de una constante o una función");
            }

            // Expect an equals token
            ExpectToken(TokenType.Equals);

            // Parse the expression...
            Node value = Expression();

            return new ConstantDeclarationNode
            {
                Name = nameToken,
                Value = value
            };

        }


        private Node FunctionCall()
        {
            string funcName = ExpectToken(TokenType.Identifier).Value;
            List<Node> parametersNodes = new();
            ExpectToken(TokenType.OpenParenthesis);
            while (Peek().Type != TokenType.CloseParenthesis && tokenList.HasMoreTokens())
            {
                parametersNodes.Add((Node)Expression().Clone());
                if (Peek().Type != TokenType.CloseParenthesis)
                {
                    ExpectToken(TokenType.Comma);
                }
            }
            ExpectToken(TokenType.CloseParenthesis);
            return new FunctionCallNode { Name = funcName, Arguments = parametersNodes };
        }
        private Node FunctionCall(string name)
        {

            //tokenList.NextToken();
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

        private Node ConstantCall()
        {
            return new ConstantCallNode() { Name = ExpectToken(TokenType.Identifier).Value };
        }
        private Node ConstantCall(string name, Token token)
        {
            return new ConstantCallNode() { Name = token.Value };
        }



        private Token ExpectToken(TokenType expectedType)
        {
            Token token = tokenList.NextToken();
            if (token.Type != expectedType)
            {
                throw new Exception("Expected token of type " + expectedType + ", but got " + token.Type);
            }
            return token;
        }
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


        private Node IdentifierExpressionType2()
        {


            if (PeekNext().Type == TokenType.OpenParenthesis)
            {

                return FunctionCall();
            }

            else return ConstantCall();
        }
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
                return new NumberNode { Value = double.Parse(token.Value) };
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

        private Node Import()
        {
            
            
            Token token = tokenList.NextToken();
            if (Peek().Type == TokenType.String)
            {
                
                return new ImportNode { Path = ExpectToken(TokenType.String).Value };
            }
            else throw new Exception("Expected string afther import and got: " + token.Type);
        }
        private Node LetInExpression()
        {
            //ExpectToken(TokenType.Let);

            List<ConstantDeclarationNode> declarations = new List<ConstantDeclarationNode>();
            while (Peek().Type != TokenType.In)
            {
                declarations.Add((ConstantDeclarationNode)ConstantDeclaration());
                ExpectToken(TokenType.Semicolon);
            }

            ExpectToken(TokenType.In);
            Node body = Expression();


            return new LetInNode { Declarations = declarations, Body = body };
        }

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

        private Token Peek()
        {
            return tokenList.Peek();  // Assume this method returns the next token without consuming it
        }



        private Token PeekNext()
        {
            return tokenList.PeekNext();
        }
    }
}
