using System;
using CommandLib;

namespace TestPlugin1
{
    [PluginLoad("")]
    public class SimplePlugin : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("SimplePlugin");
        }
    }
}
