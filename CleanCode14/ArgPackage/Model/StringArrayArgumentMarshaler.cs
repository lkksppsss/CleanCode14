using CleanCode14.Model.Interface;
using CleanCode14.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleanCode14.Model.ArgsException;

namespace CleanCode14.ArgPackage.Model;

public class StringArrayArgumentMarshaler : IArgumentMarshaler
{
    private string stringValue = "";
    public void Set(IEnumerator<string> currentArgument)
    {
        try
        {
            currentArgument.MoveNext();
            stringValue = currentArgument.Current;
        }
        catch (InvalidOperationException e)
        {
            throw new ArgsException(ErrorCode.MISSING_STRING);
        }
    }
    public static string GetValue(IArgumentMarshaler am)
    {
        if (am != null && am is StringArrayArgumentMarshaler)
            return ((StringArrayArgumentMarshaler)am).stringValue;
        else
            return "";
    }
}