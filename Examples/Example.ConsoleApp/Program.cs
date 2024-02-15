using Example.ConsoleApp;
using System;
using System.Threading.Tasks;


namespace ConsoleApp;

class Program
{
    static async Task Main()
    {
        await SimpleExample.Run();
        await AdvancedExample.Run();
    }
}
