

using System.Buffers;
using System.Collections;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;

namespace Compiler
{

    
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
    }
    public enum TokenType
    {
        Number,
        Plus,
        Minus,
        Multiply,
        Divide,
        Identifier,
        Import,
        DivideRest,
        Color,
        Draw,
        Equals,
        Point,
        Line,
        Arc,
        Segment,
        Ray,
        GuionBajo,
        Circle,
        PointSequence,
        OpenParenthesis,
        CloseParenthesis,
        GreaterThan,
        LessThan,
        Comma,
        Semicolon,
        LessThanEqual,
        GreaterThanEqual,
        EqualEqual,
        OpenBrace,
        CloseBrace,
        String,
        Let,
        In,
        If,
        Else,
        Then,
        End,

    }


    public static class Lexer
    {


        public class Tokenizer
        {
            private string input;
            private int position;

            public Tokenizer(string input)
            {
                this.input = input;
                this.position = 0;
            }

            public IEnumerable<Token> Tokenize()
            {
                while (position < input.Length)
                {
                    if (char.IsWhiteSpace(input[position]))
                    {
                        position++;
                    }
                    else if (char.IsLetter(input[position]))
                    {
                        yield return ReadIdentifier();
                    }
                    else if (char.IsDigit(input[position]))
                    {
                        yield return ReadNumber();
                    }
                    else
                    {
                        switch (input[position])
                        {
                            case '+':
                                yield return new Token { Type = TokenType.Plus, Value = "+" };
                                position++;
                                break;
                            case '%':
                                yield return new Token { Type = TokenType.DivideRest, Value = "%" };
                                position++;
                                break;
                            case '-':
                                yield return new Token { Type = TokenType.Minus, Value = "-" };
                                position++;
                                break;
                            case '_':
                                yield return new Token { Type = TokenType.GuionBajo, Value = "_" };
                                position++;
                                break;
                            case '*':
                                yield return new Token { Type = TokenType.Multiply, Value = "*" };
                                position++;
                                break;
                            case '/':
                                yield return new Token { Type = TokenType.Divide, Value = "/" };
                                position++;
                                break;

                            case '(':
                                yield return new Token { Type = TokenType.OpenParenthesis, Value = "(" };
                                position++;
                                break;
                            case ')':
                                yield return new Token { Type = TokenType.CloseParenthesis, Value = ")" };
                                position++;
                                break;
                            case ',':
                                yield return new Token { Type = TokenType.Comma, Value = "," };
                                position++;
                                break;
                            case ';':
                                yield return new Token { Type = TokenType.Semicolon, Value = ";" };
                                position++;
                                break;
                            case '{':
                                yield return new Token { Type = TokenType.OpenBrace, Value = "{" };
                                position++;
                                break;
                            case '}':
                                yield return new Token { Type = TokenType.CloseBrace, Value = "}" };
                                position++;
                                break;
                            case '"':
                                yield return ReadString();
                                break;
                            case '<':
                                if (input[position + 1] == '=')
                                {
                                    yield return new Token { Type = TokenType.LessThanEqual, Value = "<=" };
                                    position += 2;
                                }
                                else
                                {
                                    yield return new Token { Type = TokenType.LessThan, Value = "<" };
                                    position++;
                                }

                                break;
                            case '>':
                                if (input[position + 1] == '=')
                                {
                                    yield return new Token { Type = TokenType.GreaterThanEqual, Value = ">=" };
                                    position += 2;
                                }
                                else
                                {
                                    yield return new Token { Type = TokenType.GreaterThan, Value = ">" };
                                    position++;
                                }


                                break;
                            case '=':
                                if (input[position + 1] == '=')
                                {
                                    yield return new Token { Type = TokenType.EqualEqual, Value = "==" };
                                    position += 2;
                                }
                                else
                                {
                                    yield return new Token { Type = TokenType.Equals, Value = "=" };
                                    position++;
                                }
                                break;

                            default:
                                throw new Exception("Unexpected character: " + input[position]);
                        }
                    }
                }
            }

            private Token ReadIdentifier()
            {
                int start = position;
                while (position < input.Length && char.IsLetterOrDigit(input[position]))
                {
                    position++;
                }

                string value = input.Substring(start, position - start);
                TokenType type;
                switch (value)
                {
                    case "let":
                        type = TokenType.Let;
                        break;
                    case "import":
                        type = TokenType.Import;
                        break;
                    case "in":
                        type = TokenType.In;
                        break;
                    case "if":
                        type = TokenType.If;
                        break;
                    case "then":
                        type = TokenType.Then;
                        break;
                    case "arc":
                        type = TokenType.Arc;
                        break;
                    case "segment":
                        type = TokenType.Segment;
                        break;
                    case "ray":
                        type = TokenType.Ray;
                        break;
                    case "else":
                        type = TokenType.Else;
                        break;
                    case "circle":
                        type = TokenType.Circle;
                        break;
                    case "point":
                        type = TokenType.Point;
                        break;
                    case "line":
                        type = TokenType.Line;
                        break;
                    case "draw":
                        type = TokenType.Draw;
                        break;
                    default:
                        type = TokenType.Identifier;
                        break;
                }

                return new Token { Type = type, Value = value };
            }

            private Token ReadNumber()
            {
                int start = position;
                while (position < input.Length && (char.IsDigit(input[position]) || input[position] == '.'))
                {
                    position++;
                }

                string value = input.Substring(start, position - start);
                return new Token { Type = TokenType.Number, Value = value };
            }
            private Token ReadString()
            {
                int start = ++position; 
                while (position < input.Length && input[position] != '"')
                {
                    position++;
                }

                if (position >= input.Length)
                {
                    throw new Exception("Unterminated string");
                }

                string value = input.Substring(start, position - start);
                position++; 
                return new Token { Type = TokenType.String, Value = value };

            }
        }
    }
    public class TokenList
    {
        private List<Token> tokens;
        private int index;

        public TokenList(List<Token> tokens)
        {
            this.tokens = tokens;
            this.tokens.Add(new Token { Type = TokenType.End, Value = "end" });
            this.tokens.Add(new Token { Type = TokenType.Semicolon, Value = ";" });
            this.index = 0;
        }

        public Token NextToken()
        {
            return tokens[index++];
        }
       

        public Token Peek()
        {
            return tokens[index];
        }

        public bool IsThereAnEqual()
        {

            int p = 0;
            for (int i = index; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.OpenParenthesis)
                {
                    p += 1;
                    continue;
                }
                else if (tokens[i].Type == TokenType.CloseParenthesis)
                {
                    p -= 1;
                    continue;
                }
                if (p == 0 && i != index)
                {
                    
                    if (tokens[i].Type == TokenType.Equals)
                    {
                        return true;
                    }
                    else return false;
                }
            }
            return false;
        }

        public Token PeekNext()
        {
            return tokens[index + 1];
        }

        public bool HasMoreTokens()
        {
            return index < tokens.Count;
        }
    }
}