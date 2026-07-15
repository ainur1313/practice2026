using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace task11
{
    public static class ClassGenerator
    {
        public static ICalculator CreateCalculator(string userCode)
        {
            if (string.IsNullOrWhiteSpace(userCode))
                throw new ArgumentNullException(nameof(userCode), "Исходный код не может быть пустым");

            string modifiedCode = InjectInterfaceInheritance(userCode);

            SyntaxTree parsedTree = CSharpSyntaxTree.ParseText(modifiedCode);

            MetadataReference[] dependencies = ResolveDependencies();

            CSharpCompilation compiler = BuildCompilation(parsedTree, dependencies);

            return EmitAndInstantiate(compiler);
        }

        private static string InjectInterfaceInheritance(string originalCode)
        {
            const string classDeclaration = "public class Calculator";
            const string replacement = "public class Calculator : task11.ICalculator";

            return originalCode.Replace(classDeclaration, replacement);
        }

        private static MetadataReference[] ResolveDependencies()
        {
            var references = new List<MetadataReference>();

            references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location));
            references.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location));
            references.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.CoreLib")).Location));

            return references.ToArray();
        }

        private static CSharpCompilation BuildCompilation(SyntaxTree tree, MetadataReference[] references)
        {
            string dynamicName = $"GeneratedAssembly_{Guid.NewGuid():N}";

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            return CSharpCompilation.Create(
                dynamicName,
                new[] { tree },
                references,
                options
            );
        }

        private static ICalculator EmitAndInstantiate(CSharpCompilation compilation)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    string errors = FormatCompilationErrors(result);
                    throw new InvalidOperationException($"Ошибка компиляции: {errors}");
                }

                ms.Seek(0, SeekOrigin.Begin);
                byte[] rawAssembly = ms.ToArray();
                Assembly loadedAssembly = Assembly.Load(rawAssembly);

                return ActivateCalculator(loadedAssembly);
            }
        }

        private static string FormatCompilationErrors(EmitResult result)
        {
            var errorMessages = result.Diagnostics
                .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .Select(diagnostic => diagnostic.GetMessage())
                .ToList();

            return errorMessages.Count > 0
                ? string.Join("; ", errorMessages)
                : "Неизвестная ошибка";
        }

        private static ICalculator ActivateCalculator(Assembly assembly)
        {
            Type calcType = assembly.GetType("Calculator");

            if (calcType == null)
                throw new InvalidOperationException("Класс 'Calculator' не найден в сборке");

            object calcInstance = Activator.CreateInstance(calcType);

            if (calcInstance == null)
                throw new InvalidOperationException("Не удалось создать экземпляр класса 'Calculator'");

            if (calcInstance is ICalculator calculatorInstance)
                return calculatorInstance;

            throw new InvalidOperationException("Класс 'Calculator' не реализует интерфейс ICalculator");
        }
    }
}
