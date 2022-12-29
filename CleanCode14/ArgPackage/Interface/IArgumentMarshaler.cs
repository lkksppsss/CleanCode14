using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCode14.Model.Interface;

public interface IArgumentMarshaler
{
    void Set(IEnumerator<string> currentArgument);
}
