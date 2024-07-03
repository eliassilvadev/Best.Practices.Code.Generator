using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class DapperServiceCollectionExtentionsFactory
    {
        public static string Create(
            string fileContent,
            string filePath,
            IList<PropertyInfo> classProperties,
            IList<MethodInfo> methods,
            FileContentGenerationOptions options)
        {
            if (!classProperties.Any())
                throw new ValidationException("It wasn't identified public properties to generate builder class");

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateConstants(fileContent, originalClassName, classProperties, methods, filePath, options);
        }

        private static string CreateConstants(string fileContent, string originalClassName, IList<PropertyInfo> properties, IList<MethodInfo> methods, string filePath, FileContentGenerationOptions options)
        {
            var serviceCollectionFile = Path.Combine(filePath, "ServiceCollectionExtentions.cs");

            if (!File.Exists(serviceCollectionFile))
                return string.Empty;

            var serviceCollectionFileContent = File.ReadAllText(serviceCollectionFile);

            var commandProvidersDependencyMappings = new StringBuilder();
            var repositoriesDependencyMappings = new StringBuilder();

            var commandProviderMapping = $"service.AddScoped<I{originalClassName}CqrsCommandProvider, {originalClassName}CqrsCommandProvider>();";

            if (!serviceCollectionFileContent.Contains(commandProviderMapping) && !commandProvidersDependencyMappings.ToString().Contains(commandProviderMapping))
                commandProvidersDependencyMappings.AppendLine($"\t\t\t{commandProviderMapping}");

            var repositoryMapping = $"service.AddScoped<I{originalClassName}Repository, {originalClassName}Repository>();";

            if (!serviceCollectionFileContent.Contains(repositoryMapping) && !repositoriesDependencyMappings.ToString().Contains(repositoryMapping))
                repositoriesDependencyMappings.AppendLine($"\t\t\t{repositoryMapping}");

            var mapCommandProvidersMethod = "public static void MapCommandProviders(this IServiceCollection service)";

            int insertIndex = serviceCollectionFileContent.IndexOf(mapCommandProvidersMethod) + mapCommandProvidersMethod.Length;
            insertIndex = serviceCollectionFileContent.IndexOf('{', insertIndex);
            var newFileContent = serviceCollectionFileContent;

            if ((insertIndex != -1) && commandProvidersDependencyMappings.Length > 0)
            {
                newFileContent = serviceCollectionFileContent.Insert(insertIndex + 1, "\n" + commandProvidersDependencyMappings.ToString());
            }

            var mapRepositoriesMethod = "public static void MapRepositories(this IServiceCollection service)";

            insertIndex = serviceCollectionFileContent.IndexOf(mapRepositoriesMethod) + mapRepositoriesMethod.Length;
            insertIndex = serviceCollectionFileContent.IndexOf('{', insertIndex);

            if ((insertIndex != -1) && repositoriesDependencyMappings.Length > 0)
            {
                newFileContent = serviceCollectionFileContent.Insert(insertIndex + 1, "\n" + repositoriesDependencyMappings.ToString());
            }

            return newFileContent;
        }

        private static string GetOriginalClassName(string fileContent)
        {
            var regex = Regex.Match(fileContent, @"\s+(class)\s+(?<Name>[^\s]+)");

            return regex.Groups["Name"].Value.Replace(":", "");
        }
    }
}