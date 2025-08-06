// See https://aka.ms/new-console-template for more information



using AA.Modules.EcEniBuilderAcontisModule;
using AA.Modules.EcEniBuilderAcontisModule.Tests;

namespace EcMasterAcontisApp;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting  Test...");

        EcEniBuilderAcontisTests.RunAll();



        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

    }
}