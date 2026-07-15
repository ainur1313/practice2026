using CommandLib;
using System.Reflection;

namespace PluginApp
{
    public class PluginLoader
    {
        private readonly List<Type> _discoveredTypes = new();
        private readonly List<Type> _orderedPlugins = new();
        private readonly HashSet<string> _processingStack = new();
        private readonly HashSet<string> _processedNodes = new();

        public IReadOnlyList<Type> DiscoveredTypes => _discoveredTypes;
        public IReadOnlyList<Type> OrderedPlugins => _orderedPlugins;

        public List<Type> LoadPlugins(string pluginDirectory)
        {
            _discoveredTypes.Clear();
            _orderedPlugins.Clear();
            _processingStack.Clear();
            _processedNodes.Clear();

            if (!Directory.Exists(pluginDirectory))
            {
                Directory.CreateDirectory(pluginDirectory);
                Console.WriteLine($"Создана директория плагинов: {pluginDirectory}");
                return _discoveredTypes;
            }

            string[] assemblies = Directory.GetFiles(pluginDirectory, "*.dll");
            Console.WriteLine($"Найдено DLL-файлов: {assemblies.Length}");

            foreach (string assemblyPath in assemblies)
            {
                try
                {
                    Assembly loadedAssembly = Assembly.LoadFrom(assemblyPath);
                    int typesAdded = 0;

                    foreach (var candidateType in loadedAssembly.GetTypes())
                    {
                        if (candidateType.IsClass &&
                            !candidateType.IsAbstract &&
                            typeof(ICommand).IsAssignableFrom(candidateType))
                        {
                            _discoveredTypes.Add(candidateType);
                            typesAdded++;
                        }
                    }

                    Console.WriteLine($"  > {Path.GetFileName(assemblyPath)}: загружено {typesAdded} команд");
                }
                catch (Exception error)
                {
                    Console.WriteLine($"Не удалось загрузить {Path.GetFileName(assemblyPath)}: {error.Message}");
                }
            }

            return _discoveredTypes;
        }

        public List<Type> SortPlugins()
        {
            _orderedPlugins.Clear();
            _processingStack.Clear();
            _processedNodes.Clear();

            Console.WriteLine("Топологическая сортировка плагинов");

            foreach (var pluginType in _discoveredTypes)
            {
                TopologicalSort(pluginType);
            }

            Console.WriteLine($"Порядок выполнения определен ({_orderedPlugins.Count} плагинов)");
            return _orderedPlugins;
        }

        private void TopologicalSort(Type pluginType)
        {
            if (_processedNodes.Contains(pluginType.Name))
                return;

            if (_processingStack.Contains(pluginType.Name))
            {
                throw new InvalidOperationException(
                    $"Обнаружена циклическая зависимость: {pluginType.Name}!");
            }

            _processingStack.Add(pluginType.Name);

            var dependencyAttr = pluginType.GetCustomAttribute<PluginLoadAttribute>();
            if (dependencyAttr != null && !string.IsNullOrEmpty(dependencyAttr.PluginLoadPastNode))
            {
                Type requiredPlugin = null;

                foreach (var candidate in _discoveredTypes)
                {
                    if (candidate.Name == dependencyAttr.PluginLoadPastNode)
                    {
                        requiredPlugin = candidate;
                        break;
                    }
                }

                if (requiredPlugin != null)
                {
                    Console.WriteLine($"  - {pluginType.Name} зависит от {requiredPlugin.Name}");
                    TopologicalSort(requiredPlugin);
                }
                else
                {
                    Console.WriteLine($"Зависимость {pluginType.Name} -> {dependencyAttr.PluginLoadPastNode} не найдена");
                }
            }

            _processingStack.Remove(pluginType.Name);
            _processedNodes.Add(pluginType.Name);
            _orderedPlugins.Add(pluginType);
        }

        public void ExecutePlugins()
        {
            Console.WriteLine("\nЗапуск плагинов");

            int executedCount = 0;
            int failedCount = 0;

            foreach (var pluginType in _orderedPlugins)
            {
                try
                {
                    Console.Write($"  {pluginType.Name}");

                    if (Activator.CreateInstance(pluginType) is ICommand commandInstance)
                    {
                        commandInstance.Execute();
                        Console.WriteLine("Успешно");
                        executedCount++;
                    }
                    else
                    {
                        Console.WriteLine("Не удалось создать экземпляр");
                        failedCount++;
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine($"Ошибка: {error.Message}");
                    failedCount++;
                }
            }

            Console.WriteLine(new string('=', 40));
            Console.WriteLine($"Успешно: {executedCount}, с ошибками: {failedCount}");
        }
    }

    class Program
    {
        public static void Main()
        {
            string pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

            Console.WriteLine("PluginApp");
            Console.WriteLine($"Директория плагинов: {pluginsDirectory}");

            var pluginManager = new PluginLoader();
            var discoveredPlugins = pluginManager.LoadPlugins(pluginsDirectory);

            Console.WriteLine($"\nОбнаружено плагинов: {discoveredPlugins.Count}");

            if (discoveredPlugins.Count == 0)
            {
                Console.WriteLine("Нет доступных плагинов для выполнения.");
                return;
            }

            try
            {
                pluginManager.SortPlugins();
                pluginManager.ExecutePlugins();
            }
            catch (Exception criticalError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nОШИБКА {criticalError.Message}");
                Console.ResetColor();
            }
        }
    }
}
