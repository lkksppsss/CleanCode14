using CleanCode14.Model;
using CleanCode14.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleanCode14.Model.ArgsException;

namespace CleanCode14.ArgPackage.Model;

public class DoubleArgumentMarshaler : IArgumentMarshaler
{
    private double doubleValue = 0;
    public void Set(IEnumerator<string> currentArgument)
    {
        string parameter = string.Empty;
        try 
        {
            currentArgument.MoveNext();
            parameter = currentArgument.Current;
            doubleValue = double.Parse(parameter);
        } 
        catch (InvalidOperationException e) 
        {
            throw new ArgsException(ErrorCode.MISSING_DOUBLE);
        } 
        catch (FormatException e) 
        {
            throw new ArgsException(ErrorCode.INVALID_DOUBLE, parameter);
        }
    }
    public static double GetValue(IArgumentMarshaler am)
    {
        if (am != null && am is DoubleArgumentMarshaler)
            return ((DoubleArgumentMarshaler)am).doubleValue;
        else
            return 0;
    }
}
