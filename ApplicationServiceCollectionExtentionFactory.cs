using BestPracticesCodeGenerator.Dtos;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class ApplicationServiceCollectionExtentionFactory
    {
        public static string Create(
            string fileContent,
            string filePath,
            IList<PropertyInfo> classProperties,
            IList<MethodInfo> methods,
            FileContentGenerationOptions options)
        {
            var originalClassName = GetOriginalClassName(fileContent);

            return CreateDependencyServiceMappings(fileContent, originalClassName, classProperties, methods, filePath, options);
        }

        private static string CreateDependencyServiceMappings(string fileContent, string originalClassName, IList<PropertyInfo> properties, IList<MethodInfo> methods, string filePath, FileContentGenerationOptions options)
        {
            var serviceCollectionFile = Path.Combine(filePath, "ServiceCollectionExtentions.cs");

            if (!File.Exists(serviceCollectionFile))
                return string.Empty;

            var serviceCollectionFileContent = File.ReadAllText(serviceCollectionFile);

            var useCasesDependencyMappings = new StringBuilder();
            var validationsDependencyMappings = new StringBuilder();
            var usingDeclarations = new StringBuilder();

            if (options.GenerateCreateUseCase)
            {
                var dependencyMapping = $"service.AddSingleton<Create{originalClassName}UseCase>();";

                if (!serviceCollectionFileContent.Contains(dependencyMapping) && !useCasesDependencyMappings.ToString().Contains(dependencyMapping))
                    useCasesDependencyMappings.AppendLine($"\t\t\t{dependencyMapping}");

                var validationMapping = $"service.AddSingleton<IValidator<Create{originalClassName}Input>, Create{originalClassName}InputValidator>();";

                if (!serviceCollectionFileContent.Contains(validationMapping) && !validationsDependencyMappings.ToString().Contains(validationMapping))
                    validationsDependencyMappings.AppendLine($"\t\t\t{validationMapping}");
            }

            if (options.GenerateUpdateUseCase)
            {
                var dependencyMapping = $"service.AddSingleton<Update{originalClassName}UseCase>();";

                if (!serviceCollectionFileContent.Contains(dependencyMapping) && !useCasesDependencyMappings.ToString().Contains(dependencyMapping))
                    useCasesDependencyMappings.AppendLine($"\t\t\t{dependencyMapping}");

                var validationMapping = $"service.AddSingleton<IValidator<Update{originalClassName}Input>, Update{originalClassName}InputValidator>();";

                if (!serviceCollectionFileContent.Contains(validationMapping) && !validationsDependencyMappings.ToString().Contains(validationMapping))
                    validationsDependencyMappings.AppendLine($"\t\t\t{validationMapping}");
            }

            if (options.GenerateDeleteUseCase)
            {
                var dependencyMapping = $"service.AddSingleton<Delete{originalClassName}UseCase>();";

                if (!serviceCollectionFileContent.Contains(dependencyMapping) && !useCasesDependencyMappings.ToString().Contains(dependencyMapping))
                    useCasesDependencyMappings.AppendLine($"\t\t\t{dependencyMapping}");
            }

            if (options.GenerateGetUseCase)
            {
                var dependencyMapping = $"service.AddSingleton<GetPaginatedResultsUseCase<ICqrsQueryProvider<{originalClassName}ListItemOutput>, {originalClassName}ListItemOutput>>();";

                if (!serviceCollectionFileContent.Contains(dependencyMapping) && !useCasesDependencyMappings.ToString().Contains(dependencyMapping))
                    useCasesDependencyMappings.AppendLine($"\t\t\t{dependencyMapping}");
            }

            var mapUseCasesMethod = "public static void MapUseCases(this IServiceCollection service)";

            int insertIndex = serviceCollectionFileContent.IndexOf(mapUseCasesMethod) + mapUseCasesMethod.Length;
            insertIndex = serviceCollectionFileContent.IndexOf('{', insertIndex);
            var newFileContent = serviceCollectionFileContent;

            if ((insertIndex != -1) && useCasesDependencyMappings.Length > 0)
            {
                newFileContent = serviceCollectionFileContent.Insert(insertIndex + 1, "\n" + useCasesDependencyMappings.ToString());
            }

            var mapValidationsMethod = "public static void MapValidations(this IServiceCollection service)";

            insertIndex = newFileContent.IndexOf(mapValidationsMethod) + mapValidationsMethod.Length;
            insertIndex = newFileContent.IndexOf('{', insertIndex);

            if ((insertIndex != -1) && validationsDependencyMappings.Length > 0)
            {
                newFileContent = newFileContent.Insert(insertIndex + 1, "\n" + validationsDependencyMappings.ToString());
            }

            if (options.GenerateCreateUseCase || options.GenerateUpdateUseCase || options.GenerateDeleteUseCase)
            {
                var useCasesUsingDeclaration = $"using {GetNameRootProjectName()}.Core.Application.UseCases;";

                if (!newFileContent.Contains(useCasesUsingDeclaration) && !usingDeclarations.ToString().Contains(useCasesUsingDeclaration))
                    usingDeclarations.AppendLine($"{useCasesUsingDeclaration}");

                var usingDtosDeclaration = $"using {GetNameRootProjectName()}.Core.Application.Dtos;";

                if (!newFileContent.Contains(usingDtosDeclaration) && !usingDeclarations.ToString().Contains(usingDtosDeclaration))
                    usingDeclarations.AppendLine($"{usingDtosDeclaration}");
            }

            if (options.GenerateCreateUseCase || options.GenerateUpdateUseCase)
            {
                var usingValidatorsDeclaration = $"using {GetNameRootProjectName()}.Core.Application.Dtos.Validators;";

                if (!newFileContent.Contains(usingValidatorsDeclaration) && !usingDeclarations.ToString().Contains(usingValidatorsDeclaration))
                    usingDeclarations.AppendLine($"{usingValidatorsDeclaration}");

                var usingCoreUseCasesDeclaration = $"using Best.Practices.Core.Application.UseCases;";

                if (!newFileContent.Contains(usingCoreUseCasesDeclaration) && !usingDeclarations.ToString().Contains(usingCoreUseCasesDeclaration))
                    usingDeclarations.AppendLine($"{usingCoreUseCasesDeclaration}");

                var usingCoreQueryProvidersDeclaration = $"using Best.Practices.Core.Application.Cqrs.QueryProviders;";

                if (!newFileContent.Contains(usingCoreQueryProvidersDeclaration) && !usingDeclarations.ToString().Contains(usingCoreQueryProvidersDeclaration))
                    usingDeclarations.AppendLine($"{usingCoreQueryProvidersDeclaration}");

                var usingFluentValidationDeclaration = $"using FluentValidation;";

                if (!newFileContent.Contains(usingFluentValidationDeclaration) && !usingDeclarations.ToString().Contains(usingFluentValidationDeclaration))
                    usingDeclarations.AppendLine($"{usingFluentValidationDeclaration}");
            }

            var classNamespace = $"namespace {GetNameRootProjectName()}.Core.Configurations";

            insertIndex = newFileContent.IndexOf(classNamespace) - 1;

            if ((insertIndex != -1) && usingDeclarations.Length > 0)
            {
                newFileContent = newFileContent.Insert(insertIndex, "\n" + usingDeclarations.ToString());
            }

            return newFileContent;
        }

        private static string GetNameRootProjectName()
        {
            var solution = VS.Solutions.GetCurrentSolutionAsync().Result;

            return solution.Name.Replace(".sln", "");
        }

        private static string GetOriginalClassName(string fileContent)
        {
            var regex = Regex.Match(fileContent, @"\s+(class)\s+(?<Name>[^\s]+)");

            return regex.Groups["Name"].Value.Replace(":", "");
        }
    }
}