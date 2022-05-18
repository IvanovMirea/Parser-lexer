using System.Text.RegularExpressions;

Console.WriteLine("Input your expression");
var testString = Console.ReadLine();
var list = Lex.Lexer(testString);
Parser.Parse(list);



if (list.Count < 3)
{
    Console.WriteLine("Wrong expression!!");
    return;
}

Console.WriteLine("___________________________________________________________________ \nTokens:\n");
foreach (var l in list)
    Console.WriteLine(l);
public static class Parser
{
    public static void Parse(List<Token> tokens)
    {
        Console.WriteLine("\n___________________________________________________________________\nErrors:\n ");

        if (tokens[0] != Token.VAR && tokens[0] != Token.WHILE_KW && tokens[0] != Token.IF_KW)
        {
            Console.WriteLine("Wrong expression! You must write VAR, WHILE or IF first !");
        }
        if (tokens[0] == Token.WHILE_KW || tokens[0] == Token.IF_KW)
        {
            if (tokens[1] != Token.L_BRACE)
            {
                Console.WriteLine("You must write a brace after keywords !");
            }
            if (tokens.Count(x => x == Token.L_BRACE) != tokens.Count(x => x == Token.R_BRACE))
                Console.WriteLine("Wrong expression! You must close the braces !");
            if (tokens.Count(x => x == Token.L_SQR_BR)!= tokens.Count(x => x == Token.R_SQR_BR))
                Console.WriteLine("Wrong expression! You must close the braces !");
            if (tokens[^2] == Token.VAR && tokens[^3] == Token.L_SQR_BR)
            {
                Console.WriteLine(" You must input a correct expression in a curly braces!");
            }
            if (tokens[^2] == Token.DIGIT && tokens[^3] == Token.L_SQR_BR)
            {
                Console.WriteLine(" You must input a correct expression in a curly braces!");
            }
            if (tokens[^1] != Token.R_SQR_BR)
            {
                Console.WriteLine("The keyword expression must end with curly braces!");
            }
            if (tokens[^2] != Token.VAR && tokens[^2] != Token.DIGIT && tokens[^2] != Token.R_BRACE && tokens[^2] != Token.R_SQR_BR)
            {
                Console.WriteLine("You must enter a correct expression");
            }

        }
        if (tokens[0] == Token.VAR)
        {
            if (tokens[1] != Token.ASSIGNMENT_OPERATOR)
                Console.WriteLine("ASSIGNMENT OPERATOR must be second!");
            if (tokens[2] != Token.DIGIT && tokens[2] != Token.VAR)
                Console.WriteLine("You must write a DIGIT or VARIABLE after ASSIGNMENT OPERATOR !");
            if (tokens[^1] == Token.OPERATOR)
                Console.WriteLine("You must add another one DIGIT after OPERATOR");
        }
        

    }
}

public enum Token
{
    VAR,
    DIGIT,
    OPERATOR,
    ASSIGNMENT_OPERATOR,
    L_BRACE,
    R_BRACE,
    L_SQR_BR,
    R_SQR_BR,
    CONDITIONAL_OP,
    IF_KW,
    WHILE_KW,
    ELSE_KW
}

public static class Lex
{
    private readonly static Dictionary<Token, Regex> _map = new()
    {
        { Token.IF_KW, new("^if$", RegexOptions.Compiled) },
        { Token.ELSE_KW, new("^else$", RegexOptions.Compiled) },
        { Token.WHILE_KW, new("^while$", RegexOptions.Compiled) },
        { Token.VAR, new("^[A-Za-z]+$", RegexOptions.Compiled) },
        { Token.DIGIT, new("^0|[1-9][0-9]*$", RegexOptions.Compiled) },
        { Token.OPERATOR, new(@"^[-|+|/|\*]$", RegexOptions.Compiled) },
        { Token.ASSIGNMENT_OPERATOR, new("^=$", RegexOptions.Compiled) },
        { Token.L_BRACE, new(@"^\($", RegexOptions.Compiled) },
        { Token.R_BRACE, new(@"^\)$", RegexOptions.Compiled) },
        { Token.L_SQR_BR, new("^{$", RegexOptions.Compiled) },
        { Token.R_SQR_BR, new("^}$", RegexOptions.Compiled) },
        { Token.CONDITIONAL_OP, new(@"^[>|<]$", RegexOptions.Compiled) },
       

    };

    public static List<Token> Lexer(string input)
    {
        var result = new List<Token>();
        Token? currentToken = null;
        var skip = 0;
        for (var i = 1; i <= input.Length; i++)
        {
            var curString = input[skip..i];

            if (currentToken != null)
            {
                if (_map[currentToken.Value].IsMatch(curString))
                {
                    if (input.Length == i)
                        result.Add(currentToken.Value);
                    continue;
                }                
                currentToken = _map.FirstOrDefault(x => x.Value.IsMatch(curString[..^1])).Key;
                result.Add(currentToken.Value);
                currentToken = null;
                skip = --i;
                continue;
            }

            if (!_map.Any(x => x.Value.IsMatch(curString)))
            {
                skip = i;
                continue;
            }

            currentToken = _map.FirstOrDefault(x => x.Value.IsMatch(curString)).Key;
            if (input.Length == i)
            {
                result.Add(currentToken.Value);
            }
        }
        return result;
    }
}       