using CleanCode_Darft.ArgPackage;

static void main(string[] args)
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
