using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCode_Draft_1.ArgPackage;

public class Args_bool
{
    private string schema;
    private string[] args;
    private bool valid;
    private ICollection<char> unExpectedArguments = new List<char>();
    private Dictionary<char, Boolean> booleanArgs_bool = new Dictionary<char, Boolean>();
    private int numberOfArguments = 0;
    public Args_bool(string schema, string[] args)
    {
        this.schema = schema;
        this.args = args;
        valid = Parse();
    }
    public bool IsValid()
    {
        return valid;
    }
    private bool Parse()
    {
        if (schema.Length == 0 && args.Length == 0) return true;
        ParseSchema();
        ParseArgument();
        return unExpectedArguments.Count() == 0;
    }
    private bool ParseSchema()
    {
        foreach (string element in schema.Split(","))
        {
            if (element.Length > 0)
            {
                string trimmedElement = element.Trim();
                ParseSchemaElement(trimmedElement);
            }
        }
        return true;
    }
    private void ParseSchemaElement(string element)
    {
        if (element.Length == 1)
        {
            ParseBooleanSchemaElement(element);
        }
    }
    private void ParseBooleanSchemaElement(string element)
    {
        char c = element[0];
        if (char.IsLetter(c))
        {
            booleanArgs_bool.Add(c, false);
        }
    }
    private void ParseArgument(string arg)
    {
        if (arg.StartsWith("-")) ParseElements(arg);
    }
    private void ParseElements(string arg)
    {
        for (int i = 1; i < arg.Length; i++) ParseElement(arg[i]);
    }
    private void ParseElement(char argChar)
    {
        if (IsBoolean(argChar))
        {
            numberOfArguments++;
            SetBooleanArg(argChar, true);
        }
        else unExpectedArguments.Add(argChar);
    }
    private void SetBooleanArg(char argChar, bool value)
    {
        booleanArgs_bool.Add(argChar, value);
    }
    private bool IsBoolean(char argChar)
    {
        return booleanArgs_bool.ContainsKey(argChar);
    }
    public int Cardinality()
    {
        return numberOfArguments;
    }
    public string Usage()
    {
        if (schema.Length > 0)
            return "-[" + schema + "]";
        else return "";
    }
    public string ErrorMessage()
    {
        if (unExpectedArguments.Count() > 0)
        {
            return UnexpectedArgumentMessage();
        }
        else return "";
    }
    private string UnexpectedArgumentMessage()
    {
        StringBuilder message = new StringBuilder("Argument( s) -");
        foreach (char c in unExpectedArguments)
        {
            message.Append(c);
        }
        message.Append("unexpected.");
        return message.ToString();
    }
    public bool GetBoolean(char arg)
    {
        booleanArgs_bool.TryGetValue(arg, out bool result);
        return result;
    }
}
