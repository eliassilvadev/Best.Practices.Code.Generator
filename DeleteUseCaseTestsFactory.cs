using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using BestPracticesCodeGenerator.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class DeleteUseCaseTestsFactory
    {
        public static string Create(string fileContent, IList<PropertyInfo> classProperties, string filePath)
        {
            Validate(fileContent);

            if (!classProperties.Any())
                throw new ValidationException("It wasn't identified public properties to generate builder class");

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateUseCaseTestsClass(fileContent, originalClassName, classProperties, filePath);
        }

        private static string CreateUseCaseTestsClass(string fileContent, string originalClassName, IList<PropertyInfo> properties, string filePath)
        {
            var content = new StringBuilder();

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine("using Best.Practices.Core.Common;");
            content.AppendLine("using Best.Practices.Core.Extensions;");
            content.AppendLine("using Best.Practices.Core.UnitOfWork.Interfaces;");
            content.AppendLine("using FluentAssertions;");
            content.AppendLine("using Moq;");
            content.AppendLine("using Xunit;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Application.UseCases;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Domain.Repositories.Interfaces;");
            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

            content.AppendLine("{");

            var newClassName = string.Concat("Delete", originalClassName, "UseCaseTests");

            content.AppendLine(string.Concat("\tpublic class ", newClassName));

            content.AppendLine("\t{");

            GeneratePrivateVariables(content, originalClassName);

            GenerateTestConstructor(content, originalClassName, newClassName);

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

        private static void GenerateTestConstructor(StringBuilder content, string originalClassName, string newClassName)
        {
            content.AppendLine();
            content.AppendLine($"\t\tpublic {newClassName}()");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\t_unitOfWork = new Mock<IUnitOfWork>();");
            content.AppendLine($"\t\t\t_{originalClassName.GetWordWithFirstLetterDown()}Repository = new Mock<I{originalClassName}Repository>();");
            content.AppendLine($"\t\t\t_useCase = new Delete{originalClassName}UseCase(_{originalClassName.GetWordWithFirstLetterDown()}Repository.Object, _unitOfWork.Object);");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GenerateInternalExecuteMethod(StringBuilder content, string className, IList<PropertyInfo> properties)
        {
            content.AppendLine("\t\t[Fact]");
            content.AppendLine($"\t\tpublic async Task Execute_EverythingIsOk_ReturnsSuccess()");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\tvar id = Guid.NewGuid();");
            content.AppendLine("");
            content.AppendLine($"\t\t\tvar output = await _useCase.ExecuteAsync(id);");
            content.AppendLine("");
            content.AppendLine("\t\t\toutput.HasErros.Should().BeFalse();");
            content.AppendLine($"\t\t\t_{className.GetWordWithFirstLetterDown()}Repository.Verify(x => x.GetById(id), Times.Once);");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GeneratePrivateVariables(StringBuilder content, string originalClassName)
        {
            content.AppendLine($"\t\tprivate readonly Delete{originalClassName}UseCase _useCase;");
            content.AppendLine($"\t\tprivate readonly Mock<IUnitOfWork> _unitOfWork;");
            content.AppendLine($"\t\tprivate readonly Mock<I{originalClassName}Repository> _{originalClassName.GetWordWithFirstLetterDown()}Repository;");
            content.AppendLine($"");
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