using Compiler;

public static class CompilerUtils
{
    /// <summary>
    /// Método que compila código G# 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns> <summary>
        public static string Run(string input)
    {
        List<string> stringTokens = new List<string>();

        List<Token> tokens = Lexer.TokensInit(input);
        
        string output = ">>";

        for (int i = 0; i < tokens.Count; i++)
        {
            if(tokens[i] != null)
            {
                output += tokens[i].ToString();
            }
        }

        return output;
        
        try
        {Function.InitBasicFunctions();
        

            //List<Token> tokens = new List<Token>(){new Token(){Type="number",Content="4"},new Token(){Type = "Operator",Content = "+"},new Token(){Type="iden",Content ="x"}};
            //Function fa = new Function(new Token(){Type = "mix",Content = "cpar",exp = new List<Token>(){new Token(){Type="iden",Content="x"}}},tokens,new Token(){Type = "iden",Content="test"} );



             
            var list = Lexer.TokensInit(input);

            if (Error.errors.Count > 0)
            {
                foreach (Error e in Error.errors)
                {
                    return (e.ToString());
                }
                Error.errors.Clear();

                
            }
            var l2 = Lexer.GetToken2(list);

            if (Error.errors.Count > 0)
            {
                foreach (Error e in Error.errors)
                {
                    return (e.ToString());
                }
                Error.errors.Clear();

                
            }

            if (l2.Count == 0) return " ";



            if (Function.GetFunction(l2)) return "asd";

            if (Error.errors.Count > 0)
            {
                foreach (Error e in Error.errors)
                {
                    return (e.ToString());
                }
                Error.errors.Clear();

                
            }



            Node node = Parser.Parse(l2, 1);

            if (Error.errors.Count > 0)
            {
                foreach (Error e in Error.errors)
                {
                    return (e.ToString());
                }
                Error.errors.Clear();

                
            }
            Token? token = node.GetValue();

            if (Error.errors.Count > 0)
            {
                foreach (Error e in Error.errors)
                {
                    return (e.ToString());
                }
                Error.errors.Clear();

                
            }

            string? value = token.Content.ToString();
            if (Error.errors.Count > 0)
            {
                foreach (Error e in Error.errors)
                {
                   return (e.ToString());
                }
                Error.errors.Clear();

                
            }
            return value;}
            catch (System.Exception ex)
        {
            
            return "An exception ocurred: " + ex.ToString();
        }
            


        

    }
}