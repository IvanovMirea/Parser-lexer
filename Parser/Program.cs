using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;

Inter.InterList("LinkedList<string> e = LinkedList<>() \n LinkedList<string> b = LinkedList<>() \n e.add(abc) \n e.add(a) \n b.add(adios) \n e.remove(abc)");
var testString = Console.ReadLine();
var list = Lex.Lexer(testString);
Parser.Parse(list.Select(x => x.Item1).ToList());

Console.WriteLine("___________________________________________________________________ \nInter:\n");
Interpreter.Inter(list);



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
            if (tokens.Count(x => x == Token.L_SQR_BR) != tokens.Count(x => x == Token.R_SQR_BR))
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

public class MyNode<T>
{
    public MyNode<T> Next { get; set; }
    public MyNode<T> Prev { get; set; }
    public T Data { get; set; }

    public MyNode(T data)
    {
        Data = data;
    }
}
public class MyLinkedList<T> : IEnumerable<T>
{
    MyNode<T> head;
    MyNode<T> tail;
    int count;

    public void Add(T data)
    {
        MyNode<T> node = new(data);

        if (head == null)
            head = node;
        else
            tail.Next = node;
        tail = node;

        count++;
    }

    public bool Remove(T data)
    {
        MyNode<T> current = head;
        MyNode<T> previous = null;

        while (current != null)
        {
            if (current.Data.Equals(data))
            {
                if (previous != null)
                {
                    previous.Next = current.Next;
                    if (current.Next == null)
                        tail = previous;
                }
                else
                {
                    head = head.Next;
                    if (head == null)
                        tail = null;
                }
                count--;
                return true;
            }

            previous = current;
            current = current.Next;
        }
        return false;
    }

    public int Count { get { return count; } }
    public bool IsEmpty { get { return count == 0; } }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)this).GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        MyNode<T> current = head;
        while (current != null)
        {
            yield return current.Data;
            current = current.Next;
        }
    }
}

public static class Inter
{
    public static void InterList(string lines)
    {
        var map = new Dictionary<string, (string, MyLinkedList<string>)>();

        var r = new Regex(@"(LinkedList<)([a-z]+?)(>) ([a-z]+?) = (LinkedList<>\(\))");
        var add = new Regex(@"(.+?)\.add\((.+?)\)");
        var rem = new Regex(@"(.+?)\.remove\((.+?)\)");

        var initMatches = r.Matches(lines);
        foreach (Match initMatch in initMatches)
        {
            var initGroups = initMatch.Groups;
            var list = new MyLinkedList<string>();
            map.Add(initGroups[4].Value, (initGroups[2].Value, list));
        }

        var s = lines.Split('\n');

        foreach (var x in s)
        {
            if (rem.IsMatch(x))
            {
                var removeMatch = rem.Match(x);
                var remGroups = removeMatch.Groups;
                if (map.TryGetValue(remGroups[1].Value.Trim(), out var listTuple))
                {
                    listTuple.Item2.Remove(remGroups[2].Value);
                    Console.WriteLine($"YOUR LIST {remGroups[1].Value} AFTER REMOVING - {remGroups[2].Value}:\n\t[{string.Join(' ', listTuple.Item2)}]");
                }
            }

            if (add.IsMatch(x))
            {
                var addMatch = add.Match(x);
                var addGroups = addMatch.Groups;
                if (map.TryGetValue(addGroups[1].Value.Trim(), out var listTuple))
                {
                    listTuple.Item2.Add(addGroups[2].Value);
                    Console.WriteLine($"YOUR LIST {addGroups[1].Value} AFTER ADDING {addGroups[2].Value}:\n\t[{string.Join(' ', listTuple.Item2)}]");
                }
            }

        }
    }
}
public static class Interpreter
{
    private readonly static Dictionary<string, Regex> _map = new()
    {
        { "WHILE_EXP", new("^WHILE_KW (L_BRACE .+? R_BRACE) (L_SQR_BR .+ R_SQR_BR)") },
        { "IF_EXP", new("^IF_KW (L_BRACE .+? R_BRACE) (L_SQR_BR .+ R_SQR_BR)") },
        { "ASSIGN_EXPR", new("^VAR ASSIGNMENT_OPERATOR (VAR|DIGIT) (OPERATOR (VAR|DIGIT))*") },
        { "EXPR_VALUE", new("^L_BRACE (VAR|DIGIT) (CONDITIONAL_OP (VAR|DIGIT))* R_BRACE") },
        { "exec_block", new("^L_SQR_BR (.+) R_SQR_BR") }
    };

    private static (Token, string)[] Blocks { get; set; }

    public static void Inter(List<(Token, string)> blocks)
    {
        Blocks = blocks.ToArray();
        //VAR ASSIGN_OP
        var stringTokens = string.Join(' ', blocks.Select(x => x.Item1.ToString()));

        SDFsdfasdf(stringTokens);
    }

    private static void asdfasdf(Token[] tokens)
    {
        for (int i = 0; i <= Blocks.Length - tokens.Length; i++)
        {
            if (Blocks.Skip(i).Take(tokens.Length).Select(x => x.Item1).SequenceEqual(tokens))
            {
                var some = string.Join(' ', Blocks.Skip(i).Take(tokens.Length).Select(x => x.Item2));
                Console.WriteLine(some);
                break;
            }
        }
    }

    private static void SDFsdfasdf(string tokens)
    {
        foreach (var map in _map)
        {
            var regex = map.Value;
            if (regex.IsMatch(tokens))
            {
                Console.WriteLine(map.Key);
                Console.WriteLine(tokens);
                asdfasdf(tokens.Split(' ').Select(x => Enum.Parse<Token>(x)).ToArray());
                var match = regex.Match(tokens);
                foreach (var g in match.Groups.Values.Skip(1))
                {
                    SDFsdfasdf(g.Value);
                }
            }
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

    public static List<(Token, string)> Lexer(string input)
    {
        var result = new List<(Token, string)>();
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
                        result.Add((currentToken.Value, curString));
                    continue;
                }
                currentToken = _map.FirstOrDefault(x => x.Value.IsMatch(curString[..^1])).Key;
                result.Add((currentToken.Value, curString));
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
                result.Add((currentToken.Value, curString));
            }
        }
        return result;
    }
}
/*