using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCode_Draft_1.ArgPackage;

public class Args_Add_String
{
    private string schema;
    private string[] args;
    private bool valid = true;
    private ICollection<char> unExpectedArguments = new List<char>();
    private Dictionary<char, Boolean> booleanArgs = new Dictionary<char, Boolean>();
    private Dictionary<char, string> stringArgs = new Dictionary<char, string>();
    private ICollection<char> argsFound = new HashSet<char>();
    private int currentArgument;
    private char errorArgument = '\0';
    enum ErrorCode { OK, MISSING_STRING }
    private ErrorCode errorCode = ErrorCode.OK;
    public Args_Add_String(string schema, string[] args)
    {
        this.schema = schema;
        this.args = args;
        valid = Parse();
    }
    private bool Parse() 
    {
        if (schema.Length == 0 && args.Length == 0) return true;
        ParseSchema();
        ParseArguments();
        return valid;
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
        char elementId = element[0];
        string elementTail = element.Substring(1);
        ValidateSchemaElementId(elementId);
        if (IsBooleanSchemaElement(elementTail)) 
            ParseBooleanSchemaElement(elementId);
        else if (IsStringSchemaElement(elementTail))
            ParseStringSchemaElement(elementId);
    }
    private void ValidateSchemaElementId(char elementId)
    {
        if (!char.IsLetter(elementId))
        {
            throw new FormatException("Bad char:" + elementId + " in Args format: " + schema);
        }
    }
    private void ParseStringSchemaElement(char elementId)
    {
        stringArgs.Add(elementId, "");
    }
    private bool IsStringSchemaElement(string elementTail)
    {
        return elementTail.Equals("*");
    }
    private bool IsBooleanSchemaElement(string elementTail)
    {
        return elementTail.Length == 0;
    }
    private void ParseBooleanSchemaElement(char elementId)
    {
        booleanArgs.Add(elementId, false);
    }
    private bool ParseArguments()
    {
        for (currentArgument = 0; currentArgument < args.Length; currentArgument++)
        {
            string arg = args[currentArgument];
            ParseArgument(arg);
        }
        return true;
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
        if (SetArgument(argChar)) argsFound.Add(argChar);
        else
        {
            unExpectedArguments.Add(argChar);
            valid = false;
        }
    }
    private bool SetArgument(char argChar)
    {
        bool set = true;
        if (IsBoolean(argChar)) SetBooleanArg(argChar, true);
        else if (IsString(argChar)) SetStringArg(argChar, "");
        else set = false;
        return set;
    }
    private void SetStringArg(char argChar, string s)
    {
        currentArgument++;
        try
        {
            stringArgs.Add(argChar, args[currentArgument]);
        }
        catch (Exception e)
        {
            valid = false;
            errorArgument = argChar;
            errorCode = ErrorCode.MISSING_STRING;
        }
    }
    private bool IsString(char argChar)
    {
        return stringArgs.ContainsKey(argChar);
    }
    private void SetBooleanArg(char argChar, bool value)
    {
        booleanArgs.Add(argChar, value);
    }
    private bool IsBoolean(char argChar)
    {
        return booleanArgs.ContainsKey(argChar);
    }
    public int Cardinality()
    {
        return argsFound.Count();
    }
    public string Usage()
    {
        if (schema.Length > 0) 
            return "-[" + schema + "]";
        else 
            return "";
    }
    public string ErrorMessage() 
    {
        if (unExpectedArguments.Count() > 0) 
        {
            return UnexpectedArgumentMessage();
        } 
        else 
        {
            switch (errorCode)
            {
                case ErrorCode.MISSING_STRING:
                    return string.Format("Could not find string parameter for -%c.", errorArgument);
                case ErrorCode.OK:
                    throw new Exception("TILT: Should not get here.");
            }
        }
            return "";
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
        return FalseIfNull(booleanArgs.GetValueOrDefault(arg));
    }
    private bool FalseIfNull(bool? b)
    {
        return b.HasValue ? false : b.Value;
    }
    public string GetString(char arg)
    {
        return BlankIfNull(stringArgs.GetValueOrDefault(arg));
    }
    private string BlankIfNull(string s)
    {
        return s == null ? "" : s;
    }
    public bool Has(char arg)
    {
        return argsFound.Contains(arg);
    }
    public bool IsValid()
    {
        return valid;
    }
}
