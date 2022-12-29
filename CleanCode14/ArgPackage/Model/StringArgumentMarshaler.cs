using CleanCode14.Model;
using CleanCode14.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleanCode14.Model.ArgsException;

namespace CleanCode14.ArgPackage.Model;

public class StringArgumentMarshaler : IArgumentMarshaler
{
    private string stringValue = "";
    public void Set(IEnumerator<string> currentArgument)
    {
        try
        {
            stringValue = currentArgument.MoveNext() ? currentArgument.Current : throw new Exception("IEnumerator move next fail");
        }
        catch (InvalidOperationException e)
        {
            throw new ArgsException(ErrorCode.MISSING_STRING);
        }
    }
    public static string GetValue(IArgumentMarshaler am)
    {
        if (am != null && am is StringArgumentMarshaler)
            return ((StringArgumentMarshaler)am).stringValue;
        else
            return "";
    }
}
