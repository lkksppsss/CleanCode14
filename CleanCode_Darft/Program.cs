using CleanCode_Darft.ArgPackage;

class Program
{
    static void Main(string[] args)
    {
        Args arg = new Args("l, p#, d*", new string[] {"-l", "-p", "50", "-d", "ABC" });
        bool logging = arg.GetBoolean('l');
        int port = arg.GetInt('p');
        string directory = arg.GetString('d');
        Console.WriteLine("logging:" + logging);
        Console.WriteLine("port:" + port);
        Console.WriteLine("directory:" + directory);

        Console.ReadKey();
    }
}
