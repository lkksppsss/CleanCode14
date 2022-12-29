// See https://aka.ms/new-console-template for more information

using CleanCode14.Model;
using System.IO;
using System;
static void main(string[] args)
{
    try
    {
        Args arg = new Args("l, p#, d*", args);
        bool logging = arg.GetBoolean('l');
        int port = arg.GetInt('p');
        string directory = arg.GetString('d');
        Console.WriteLine("logging:" + logging);
        Console.WriteLine("port:" + port);
        Console.WriteLine("directory:" + directory);

        Console.ReadKey();
    }
    catch (ArgsException e)
    {
        TextWriter errorWriter = Console.Error;
        errorWriter.WriteLine("Argumenterror:% s\n", e.errorMessage());
    }
}