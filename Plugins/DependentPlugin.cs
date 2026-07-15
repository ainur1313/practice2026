using System;
using CommandLib;

namespace TestPlugin2
{
    [PluginLoad("SimplePlugin")]
    public class DependentPlugin : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("DependentPlugin");
            Console.WriteLine("Зависит от SimplePlugin");

        }
    }
}
