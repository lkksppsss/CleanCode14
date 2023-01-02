using CleanCode14.ArgPackage.Model;
using CleanCode14.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleanCode14.Model.ArgsException;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CleanCode14.Model;

public class Args
{
    private Dictionary<char, IArgumentMarshaler> marshalers;
    private ICollection<char> argsFound;
    private IEnumerator<string> currentArgument;
    internal int currentArgumentIndex = 0; 
    public Args(string schema, string[] args)
    {
        marshalers = new Dictionary<char, IArgumentMarshaler>();
        argsFound = new HashSet<char>();
        ParseSchema(schema);
        ParseArgumentStrings(args.ToList()); 
    }


    private void ParseSchema(string schema)
    {
        foreach(string element in schema.Split(','))
        {
            if (element.Length > 0)
            {
                ParseSchemaElement(element.Trim());
            }
        }
    }
    private void ParseSchemaElement(string element)
    {
        char elementId = element[0];
        string elementTail = element.Substring(1);
        ValidateSchemaElementId(elementId);
        if (elementTail.Length == 0) 
            marshalers.Add(elementId, new BooleanArgumentMarshaler());
        else if (elementTail.Equals("*")) 
            marshalers.Add(elementId, new StringArgumentMarshaler());
        else if (elementTail.Equals("#")) 
            marshalers.Add(elementId, new IntegerArgumentMarshaler());
        else if (elementTail.Equals("##")) 
            marshalers.Add(elementId, new DoubleArgumentMarshaler());
        else if (elementTail.Equals("[*]")) 
            marshalers.Add(elementId, new StringArrayArgumentMarshaler());
        else 
            throw new ArgsException(ErrorCode.INVALID_ARGUMENT_FORMAT, elementId, elementTail);
    }
    private void ValidateSchemaElementId(char elementId)
    {
        if (!Char.IsLetter(elementId)) 
            throw new ArgsException(ErrorCode.INVALID_ARGUMENT_NAME, elementId, null);
    }
    private void ParseArgumentStrings(List<string> argsList)
    {
        currentArgument = argsList.Select(x => x).GetEnumerator();
        var temp = argsList.Select(x => x).GetEnumerator();

        while(currentArgument.MoveNext())
        {
            string argString = currentArgument.Current;
            if (argString.StartsWith("-"))
            {
                ParseArgumentCharacters(argString.Substring(1));
                temp.MoveNext();
            }
            else
            {
                currentArgument = temp;
                break;
            }
        }
    }
    private void ParseArgumentCharacters(string argChars)
    {
        for (int i = 0; i < argChars.Length; i++)
        {
            ParseArgumentCharacter(argChars[i]);
        }
    }
    private void ParseArgumentCharacter(char argChar)
    {
        IArgumentMarshaler m = marshalers.GetValueOrDefault(argChar);
        if (m == null)
        {
            throw new ArgsException(ArgsException.ErrorCode.UNEXPECTED_ARGUMENT, argChar, null);
        }
        else
        {
            argsFound.Add(argChar);
            try
            {
                m.Set(currentArgument);
            }
            catch (ArgsException e)
            {
                e.setErrorArgumentId(argChar);
                throw e;
            }
        }
    }
    public bool Has(char arg)
    {
        return argsFound.Contains(arg);
    }
    public int NextArgument()
    {
        return currentArgumentIndex + 1;
    }
    public bool GetBoolean(char arg)
    {
        return BooleanArgumentMarshaler.GetValue(marshalers.GetValueOrDefault(arg));
    }
    public string GetString(char arg)
    {
        return StringArgumentMarshaler.GetValue(marshalers.GetValueOrDefault(arg));
    }
    public int GetInt(char arg)
    {
        return IntegerArgumentMarshaler.GetValue(marshalers.GetValueOrDefault(arg));
    }
    public double GetDouble(char arg)
    {
        return DoubleArgumentMarshaler.GetValue(marshalers.GetValueOrDefault(arg));
    }
    public string[] GetStringArray(char[] arg)
    {
        List<string> result = new List<string>();
        foreach(char argChar in arg)
        {
            result.Add(StringArrayArgumentMarshaler.GetValue(marshalers.GetValueOrDefault(argChar)));
        }
        return result.ToArray();
    }
}
