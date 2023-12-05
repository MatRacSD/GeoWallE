

namespace Compiler
{
    public class Function
    {
        /// <summary>
        /// Constructor de las funciones predefinidas
        /// </summary>
        /// <param name="n"></param>
        private Function(string n)
        {
            name = n;
            IsInternal = true;
             ActiveFunctions.Add(this);
        }

        /// <summary>
        /// Constructor de las funciones por defecto
        /// </summary>
        /// <param name="vrs"></param>
        /// <param name="bdy"></param>
        /// <param name="nme"></param>
        public Function(Token vrs, List<Token> bdy, Token nme)
        {
            if(vrs.Content != "cpar")
            {
               throw new ArgumentException();
            }
            List<string> names = new List<string>();
            int a = 0;
            
            for (int i = 0; i < vrs.exp.Count; i++)
            {
                if(vrs.exp[i].Type == "iden" && a == 0)
                {
                    names.Add(vrs.exp[i].Content);
                    a = 1;
                }
                else if(vrs.exp[i].Content == ",")
                {
                    a = 0;
                }
                else throw new ArgumentException();
            }

            vars_0 = names.ToArray();
            body_0 = bdy;
            name = nme.Content;

            foreach (Function f in ActiveFunctions)
            {
                if(f.name == name)
                {
                    Error.errors.Add(new Error() { Type = "SEMANTIC ERROR: la funcion "+ name +" ya se encuentra definida" });
                return;
                }
            }

            ActiveFunctions.Add(this);
        }

        /// <summary>
        /// Método para invocar funciones
        /// </summary>
        /// <param name="token_a"></param>
        /// <returns></returns>
        public  Token? Call(Token token_a)
        {
            if(IsInternal)
            {
                double x = 0;
                Token tk = new Token();
                
                if(name == "sin")
                {
                    Token ax = Parser.Parse(new List<Token>(){token_a},1).GetValue();
                    if(ax.Type != "number")
                    {
                        Error.errors.Add(new Error() { Type = "SEMANTIC ERROR: la funcion sin solo recibe tokens del tipo number" });
                return null;
                    }
                     x = Math.Sin(ax.ToInt());
                    

                }
                else if(name == "print")
                {
                   Token ax = Parser.Parse(new List<Token>(){token_a},1).GetValue();
                   Console.WriteLine(ax.Content);
                   return ax;
                   
                }
                else if(name == "cos")
                {
                    Token ax = Parser.Parse(new List<Token>(){token_a},1).GetValue();
                    if(ax.Type != "number")
                    {
                        Error.errors.Add(new Error() { Type = "SEMANTIC ERROR: la funcion cos solo recibe tokens del tipo number" });
                return null;
                    }
                    x = Math.Cos(ax.ToInt());
                }
                else if(name == "exp")
                {
                    Token ax = Parser.Parse(new List<Token>(){token_a},1).GetValue();
                    if(ax.Type != "number")
                    {
                        Error.errors.Add(new Error() { Type = "SEMANTIC ERROR: la funcion exp solo recibe tokens del tipo number" });
                return null;
                    }
                    x = Math.Exp(ax.ToInt());
                }

                else if(name == "sqrt")
                {
                    Token ax = Parser.Parse(new List<Token>(){token_a},1).GetValue();
                    if(ax.Type != "number")
                    {
                        Error.errors.Add(new Error() { Type = "SEMANTIC ERROR: la funcion sqrt solo recibe tokens del tipo number" });
                return null;
                    }
                    x = Math.Sqrt(ax.ToInt());
                }

                else if(name == "log")
                {
                   List<Token> l1 = new List<Token>();
                   List<Token> l2 = new List<Token>();
                   int a = 0;

                   for (int i = 0; i < token_a.exp.Count; i++)
                   {
                    if(a == 0)
                    {
                        if(token_a.exp[i].Content == ",")
                        {
                            a = 1;
                            continue;
                        }
                        else l1.Add(token_a.exp[i].Clone());
                    }
                    else if(a == 1)
                    {
                        l2.Add(token_a.exp[i].Clone());
                    }
                   }

                   x = Math.Log(Parser.Parse(l2,1).GetValue().ToInt(),Parser.Parse(l1,1).GetValue().ToInt());


                }

                tk.Type = "number";
                tk.Content = x.ToString();
                return tk;
            }
            Token token = token_a.Clone();
            Dictionary<string,Token> var_assignation = new Dictionary<string, Token>();
            List<Token> body = body_0.Clone();
            string[] vars = (string[])vars_0.Clone();

            List<Token> aux_list = new List<Token>();
            if(token.Content != "cpar")
            {
                
                        Error.errors.Add(new Error() { Type = "SEMANTIC ERROR: se esperaba expresion entre parentesis al llamara funcion" });
                return null;
                    ;
            }
            int aux = 0;
            for (int i = 0; i < token.exp.Count; i++)

            {
                if(token.exp[i].Content == ",")
                {
                    var_assignation.Add(vars[aux],Parser.Parse(aux_list,1).GetValue());
                    aux_list.Clear();
                    aux += 1;
                }
                else if(i == token.exp.Count - 1)
                {
                    aux_list.Add(token.exp[i]);
                     var_assignation.Add(vars[aux],Parser.Parse(aux_list,1).GetValue());
                    aux_list.Clear();
                    aux += 1;
                }

                else
                {
                    aux_list.Add(token.exp[i]);
                }                
            }


            for (int i = 0; i < body.Count; i++)
            {
                if(body[i].Type == "iden")
                {
                    if(var_assignation.ContainsKey(body[i].Content))
                    {
                        body[i] = var_assignation[body[i].Content];
                    }
                   
                }

                 else if(body[i].Type == "mix" || body[i].Type == "func")
                 {
                    body[i].Remplace(var_assignation);
                 }
            }

            return Parser.Parse(body,1).GetValue();

            throw new ArgumentException();
        }
        /// <summary>
        /// Método para declarar una función
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static bool GetFunction(List<Token> tokens)
        {
            if(tokens[0].Type == "iden" && tokens[0].Content == "function")
            {
                if(tokens[1].Type == "func")
                {
                     string fname = tokens[1].Content;
                     Token f = tokens[1];
                     List<Token> tks = new List<Token>();

                     if(tokens[2].Content == "=>")
                     {
                         for (int i = 3; i < tokens.Count; i++)
                         {
                            tks.Add(tokens[i]);
                         }

                         Function function = new Function(f.exp[0],tks,new Token(){Type = "iden", Content = fname});
                         return true;
                     }
                     else {
                        
                        Error.errors.Add(new Error() { Type = "SEMANTIC ERROR: se esperaba => en declaracion de funcion" });
                       return false;
                    
                     }
                }
                Error.errors.Add(new Error() { Type = "invalid token at function declaration" });
                       return false;
                
            }

            return false;
        }
        
        /// <summary>
        /// Método que inicia las funciones predefinidas
        /// </summary>
        public static void InitBasicFunctions()
        {
            string[] fs = {"sin","cos","log","exp","sqrt","print"};
            foreach (string f in fs)
            {
                Function function = new Function(f);
            }
        }

/// <summary>
/// Nombre de la funcion
/// </summary>
        public string name;

        /// <summary>
        /// parámetros de la funcion
        /// </summary>
        private string[] vars_0;

        /// <summary>
        /// cuerpo de la función
        /// </summary>
        private List<Token> body_0;

        /// <summary>
        /// Verdadero si la funcion ya viene predefinida
        /// </summary>
        
        private bool IsInternal = false;

        /// <summary>
        /// Lista que contiene las funciones que ya se han declarado
        /// </summary>
        /// <typeparam name="Function"></typeparam>
        /// <returns></returns>
        
        public static List<Function> ActiveFunctions = new List<Function>();
    }
}