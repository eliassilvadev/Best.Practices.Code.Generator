﻿using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using BestPracticesCodeGenerator.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class GetByIdUseCaseFactory
    {
        public static string Create(string fileContent, IList<PropertyInfo> classProperties, string filePath)
        {
            Validate(fileContent);

            if (!classProperties.Any())
                throw new ValidationException("It wasn't identified public properties to generate builder class");

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateUseCaseClass(fileContent, originalClassName, classProperties, filePath);
        }

        private static string CreateUseCaseClass(string fileContent, string originalClassName, IList<PropertyInfo> properties, string filePath)
        {
            var content = new StringBuilder();

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine("using Best.Practices.Core.Extensions;");
            content.AppendLine("using Best.Practices.Core.Application.UseCases;");
            content.AppendLine("using Best.Practices.Core.Exceptions;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Application.Cqrs.QueryProviders;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Common;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Application.Dtos;");
            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

            content.AppendLine("{");

            var newClassName = string.Concat("Get", originalClassName, "ByIdUseCase");

            content.AppendLine(string.Concat("\tpublic class ", newClassName, $" : BaseUseCase<Guid, {originalClassName}Output>"));

            content.AppendLine("\t{");

            GeneratePrivateVariables(content, originalClassName);

            GenerateRepositoryConstructor(content, originalClassName, newClassName);

            GenerateInternalExecuteMethod(content, originalClassName, properties);

            content.AppendLine("\t}");

            content.AppendLine("}");

            return content.ToString();
        }

        private static void Validate(string fileContent)
        {
            if (fileContent.IndexOf("namespace ") < 0)
                throw new ValidationException("The file selected is not valid.");
        }

        private static void GenerateRepositoryConstructor(StringBuilder content, string originalClassName, string newClassName)
        {
            content.AppendLine();
            content.AppendLine($"\t\tpublic {newClassName}(I{originalClassName}CqrsQueryProvider {originalClassName.GetWordWithFirstLetterDown()}CqrsQueryProvider)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\t_{originalClassName.GetWordWithFirstLetterDown()}CqrsQueryProvider = {originalClassName.GetWordWithFirstLetterDown()}CqrsQueryProvider;");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GenerateInternalExecuteMethod(StringBuilder content, string className, IList<PropertyInfo> properties)
        {
            content.AppendLine($"\t\tpublic override async Task<UseCaseOutput<{className}Output>> InternalExecuteAsync(Guid {className.GetWordWithFirstLetterDown()}Id)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\tvar {className.GetWordWithFirstLetterDown()}Output = await _{className.GetWordWithFirstLetterDown()}CqrsQueryProvider.GetById({className.GetWordWithFirstLetterDown()}Id) ??");
            content.AppendLine($"\t\t\t\tthrow new ResourceNotFoundException(Constants.ErrorMessages.{className}WithIdDoesNotExists.Format({className.GetWordWithFirstLetterDown()}Id));");
            content.AppendLine("");
            content.AppendLine($"\t\t\t return CreateSuccessOutput({className.GetWordWithFirstLetterDown()}Output);");
            content.AppendLine("\t\t}");
        }

        private static void GeneratePrivateVariables(StringBuilder content, string originalClassName)
        {
            content.AppendLine($"\t\tprivate readonly I{originalClassName}CqrsQueryProvider _{originalClassName.GetWordWithFirstLetterDown()}CqrsQueryProvider;");
        }

        private static string GetNameSpace(string filePath)
        {
            var solution = VS.Solutions.GetCurrentSolutionAsync().Result;

            var solutionPath = Path.GetDirectoryName(solution.FullPath);

            var namespacePath = filePath.Replace(solutionPath, "").Replace("\\", ".");

            namespacePath = namespacePath.Substring(1, namespacePath.Length - 2);

            return "namespace " + namespacePath;
        }

        private static string GetNameRootProjectName()
        {
            var solution = VS.Solutions.GetCurrentSolutionAsync().Result;

            return solution.Name.Replace(".sln", "");
        }

        private static string GetUsings(string fileContent)
        {
            return fileContent.Substring(0, fileContent.IndexOf("namespace"));
        }

        private static string GetOriginalClassName(string fileContent)
        {
            var regex = Regex.Match(fileContent, @"\s+(class)\s+(?<Name>[^\s]+)");

            return regex.Groups["Name"].Value.Replace(":", "");
        }

        private static IList<PropertyInfo> GetPropertiesInfo(string fileContent)
        {
            var propertyes = new List<PropertyInfo>();

            foreach (Match item in Regex.Matches(fileContent, @"(?>public)\s+(?!class)((static|readonly)\s)?(?<Type>(\S+(?:<.+?>)?)(?=\s+\w+\s*\{\s*get))\s+(?<Name>[^\s]+)(?=\s*\{\s*get)"))
            {
                propertyes.Add(new PropertyInfo(item.Groups["Type"].Value, item.Groups["Name"].Value));
            }

            return propertyes;
        }
    }
}