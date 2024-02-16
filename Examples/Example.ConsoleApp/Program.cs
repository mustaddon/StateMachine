﻿using Example.ConsoleApp;
using System.Threading.Tasks;


namespace ConsoleApp;

class Program
{
    static async Task Main()
    {
        ReadmeExample.Run();

        await SimpleExample.Run();

        await WorkflowExample.Run();

        await AdvancedExample.Run();

        await MediatorExample.Run();
    }
}
