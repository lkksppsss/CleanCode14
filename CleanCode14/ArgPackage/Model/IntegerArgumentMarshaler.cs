using CleanCode14.Model;
using CleanCode14.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCode14.ArgPackage.Model;

public class IntegerArgumentMarshaler : IArgumentMarshaler
{
    private int intValue = 0;
    public void Set(IEnumerator<string> currentArgument)
    {
        string parameter = null;
        try 
        {
            parameter = currentArgument.MoveNext() ? currentArgument.Current : throw new Exception("IEnumerator move next fail");
            intValue = int.Parse(parameter);
        } 
        catch (InvalidOperationException e) 
        {
            throw new ArgsException(ArgsException.ErrorCode.MISSING_INTEGER);
        } 
        catch (FormatException e) 
        {
            throw new ArgsException(ArgsException.ErrorCode.INVALID_INTEGER, parameter);
        }
    }
    public static int GetValue(IArgumentMarshaler am)
    {
        if (am != null && am is IntegerArgumentMarshaler) 
            return ((IntegerArgumentMarshaler)am).intValue;
        else 
            return 0;
    }
}