// See https://aka.ms/new-console-template for more information

using CleanCode14.Model;
using System.IO;
using System;
class Program
{
    static void Main(string[] args)
    {
        try
        {
            Args arg = new Args("l, p#, d*, x##", new string[] {"-x","42.3","-l","-p","50","-d","ABC"});
            bool logging = arg.GetBoolean('l');
            int port = arg.GetInt('p');
            string directory = arg.GetString('d');
            double number = arg.GetDouble('x');
            Console.WriteLine("logging:" + logging);
            Console.WriteLine("port:" + port);
            Console.WriteLine("directory:" + directory);
            Console.WriteLine("Double:" + number);

            Console.ReadKey();
        }
        catch (ArgsException e)
        {
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLine("Argumenterror:{0}", e.errorMessage());
        }
    }
}