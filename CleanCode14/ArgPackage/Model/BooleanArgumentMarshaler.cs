using CleanCode14.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CleanCode14.ArgPackage.Model;

public class BooleanArgumentMarshaler : IArgumentMarshaler
{
    private bool booleanValue = false;
    public void Set(IEnumerator<string> currentArgument)
    {
        booleanValue = true;
    }
    public static bool GetValue(IArgumentMarshaler am)
    {
        if (am != null && am is BooleanArgumentMarshaler)
            return ((BooleanArgumentMarshaler)am).booleanValue;
        else
            return false;
    }
}
