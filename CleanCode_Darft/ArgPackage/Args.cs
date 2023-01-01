using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CleanCode_Darft.ArgPackage;
public class Args
{
    private string schema;
    private string[] args;
    private bool valid = true;
    private ICollection<char> unexpectedArguments = new List<char>();
    private Dictionary<char, Boolean> booleanArgs = new Dictionary<char, Boolean>();
    private Dictionary<char, string> stringArgs = new Dictionary<char, string>();
    private Dictionary<char, int?> intArgs = new Dictionary<char, int?>();
    private ICollection<char> argsFound = new HashSet<char>();
    private int currentArgument;
    private char errorArgumentId = '\0';
    private string errorParameter = "TILT";
    private ErrorCode errorCode = ErrorCode.OK;
    private enum ErrorCode
    {
        OK,
        MISSING_string,
        MISSING_INTEGER, INVALID_INTEGER,
        UNEXPECTED_ARGUMENT,
        INVALID_DOUBLE
    }

    public Args(string schema, string[] args)
    {
        this.schema = schema;
        this.args = args;
        valid = Parse();
    }
    private bool Parse() 
    {
        if (schema.Length == 0 && args.Length == 0) 
            return true;
        ParseSchema();

        try 
        {
            ParseArguments();
        } 
        catch (ArgsException e)
        {
            throw e;
        }
        return valid;
    }
    private bool ParseSchema() 
    {
        foreach (string element in schema.Split(",")) {
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
        {
            ParseBooleanSchemaElement(elementId);
        }
        else if (IsStringSchemaElement(elementTail)) 
        { 
            ParsestringSchemaElement(elementId); 
        }
        else if (IsIntegerSchemaElement(elementTail))
        {
            ParseIntegerSchemaElement(elementId);
        }
        else
        {
            throw new FormatException(string.Format("Argument: {0} has invalid format: {1}.", elementId, elementTail));
        }
    }
    private void ValidateSchemaElementId(char elementId)
    {
        if (!Char.IsLetter(elementId)) {
            throw new FormatException("Bad char:" + elementId + " in Args format: " + schema);
        }
    }
    private void ParseBooleanSchemaElement(char elementId)
    {
        booleanArgs.Add(elementId, false);
    }
    private void ParseIntegerSchemaElement(char elementId)
    {
        intArgs.Add(elementId, 0);
    }
    private void ParsestringSchemaElement(char elementId)
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
    private bool IsIntegerSchemaElement(string elementTail)
    {
        return elementTail.Equals("#");
    }
    private bool ParseArguments() 
    {
        for (currentArgument = 0; currentArgument < args.Length; currentArgument++) {
            string arg = args[currentArgument];
            ParseArgument(arg);
        }
        return true;
    }
    private void ParseArgument(string arg) 
    {
        if (arg.StartsWith("-")) 
            ParseElements(arg);
    }
    private void ParseElements(string arg) 
    {
        for (int i = 1; i < arg.Length; i++)
            ParseElement(arg[i]);
    }
    private void ParseElement(char argChar)
    {
        if (SetArgument(argChar)) 
        { 
            argsFound.Add(argChar); 
        }
        else
        {
            unexpectedArguments.Add(argChar);
            errorCode = ErrorCode.UNEXPECTED_ARGUMENT;
            valid = false;
        }
    }
    private bool SetArgument(char argChar)
    {
            if (IsBooleanArg(argChar)) 
                SetBooleanArg(argChar, true);
            else if (IsStringArg(argChar)) 
                SetStringArg(argChar);
            else if (IsIntArg(argChar)) 
                SetIntArg(argChar);
            else return false;

            return true;
    }
    private bool IsIntArg(char argChar)
    {
        return intArgs.ContainsKey(argChar);
    }
    private void SetIntArg(char argChar) 
    {
        currentArgument++;
        string parameter = null;
        try 
        {
            parameter = args[currentArgument];
            intArgs.Add(argChar, int.Parse(parameter));
        } 
        catch (InvalidOperationException e) 
        {
            valid = false;
            errorArgumentId = argChar;
            errorCode = ErrorCode.MISSING_INTEGER;
            throw new ArgsException(errorCode);
        } 
        catch (FormatException e) 
        {
            valid = false;
            errorArgumentId = argChar;
            errorParameter = parameter;
            errorCode = ErrorCode.INVALID_INTEGER;
            throw new ArgsException(ErrorCode.INVALID_DOUBLE, parameter);
        }
    }
    private void SetStringArg(char argChar) 
    {
        currentArgument++;
        try 
        {
            stringArgs.Add(argChar, args[currentArgument]);
        } 
        catch (InvalidOperationException e) 
        {
            valid = false;
            errorArgumentId = argChar;
            errorCode = ErrorCode.MISSING_string;
            throw new ArgsException(errorCode);
        }
    }
    private bool IsStringArg(char argChar)
    {
        return stringArgs.ContainsKey(argChar);
    }
    private void SetBooleanArg(char argChar, bool value)
    {
        booleanArgs.Add(argChar, value);
    }
    private bool IsBooleanArg(char argChar)
    {
        return booleanArgs.ContainsKey(argChar);
    }
    public int Cardinality()
    {
        return argsFound.Count;
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
        switch (errorCode)
        {
            case ErrorCode.OK:
                throw new Exception("TILT: Should not get here.");
            case ErrorCode.UNEXPECTED_ARGUMENT:
                return UnexpectedArgumentMessage();
            case ErrorCode.MISSING_string:
                return string.Format("Could not find string parameter for {0}.", errorArgumentId);
            case ErrorCode.INVALID_INTEGER:
                return string.Format("Argument {0} expects an integer but was '{1}'.", errorArgumentId, errorParameter);
            case ErrorCode.MISSING_INTEGER:
                return string.Format("Could not find integer parameter for {0}.", errorArgumentId);
        }
        return "";
    }    
    private string UnexpectedArgumentMessage()
    {
        StringBuilder message = new StringBuilder("Argument( s) -");
        foreach (char c in unexpectedArguments)
        {
            message.Append(c);
        }
        message.Append("unexpected.");
        return message.ToString();
    }
    private bool FalseIfNull(bool? b)
    {
        return b.HasValue ? false : b.Value;
    }
    private int ZeroIfNull(int? i)
    {
        return i.HasValue ? i.Value : 0;
    }
    private string BlankIfNull(string s)
    {
        return string.IsNullOrEmpty(s) ? "" : s;
    }
    public string GetString(char arg)
    {
        return BlankIfNull(stringArgs.GetValueOrDefault(arg));
    }
    public int GetInt(char arg)
    {
        return ZeroIfNull(intArgs.GetValueOrDefault(arg));
    }
    public bool GetBoolean(char arg)
    {
        return FalseIfNull(booleanArgs.GetValueOrDefault(arg));
    }
    public bool Has(char arg)
    {
        return argsFound.Contains(arg);
    }
    public bool IsValid()
    {
        return valid;
    }
    private class ArgsException : Exception
    {
        private char errorArgumentId = '\0';
        private string errorParameter = null;
        private ErrorCode errorCode = ErrorCode.OK;
        public ArgsException()
        {
        }
        public ArgsException(string message) : base(message)
        {
        }
        public ArgsException(ErrorCode errorCode)
        {
            this.errorCode = errorCode;
        }
        public ArgsException(ErrorCode errorCode, string errorParameter)
        {
            this.errorCode = errorCode;
            this.errorParameter = errorParameter;
        }
        public ArgsException(ErrorCode errorCode, char errorArgumentId, string errorParameter)
        {
            this.errorCode = errorCode;
            this.errorParameter = errorParameter;
            this.errorArgumentId = errorArgumentId;
        }
    }
}