using BestPracticesCodeGenerator.Exceptions;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class DapperCommandFactory
    {
        public static string Create(string fileContent, string filePath)
        {
            Validate(fileContent);

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateRepositoryClass(fileContent, originalClassName, filePath);
        }

        private static string CreateRepositoryClass(string fileContent, string originalClassName, string filePath)
        {
            var content = new StringBuilder();

            content.Append(GetUsings(fileContent));

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine("using Dapper;");
            content.AppendLine("using MySql.Data.MySqlClient;");
            content.AppendLine("using System.Data;");
            content.AppendLine("using Best.Practices.Core.CommandProvider.Dapper.EntityCommands;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Domain.Models;");
            content.AppendLine("");

            content.AppendLine(GetNameSpace(filePath));

            content.AppendLine("{");

            var newClassName = string.Concat(originalClassName, "Command");

            content.AppendLine(string.Concat("\tpublic class ", newClassName, $" : DapperCommand<{originalClassName}>"));

            content.AppendLine("\t{");

            GenerateRepositoryConstructor(content, originalClassName, newClassName);

            GenerateCreateCommandDefinitionsMethod(content, originalClassName);

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
            content.AppendLine($"\t\tpublic {newClassName}(IDbConnection connection, {originalClassName} affectedEntity) : base(connection, affectedEntity)");
            content.AppendLine("\t\t{");
            content.AppendLine($"");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GenerateCreateCommandDefinitionsMethod(StringBuilder content, string className)
        {
            content.AppendLine($"\t\tpublic override IList<CommandDefinition> CreateCommandDefinitions({className} entity)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\t CreateCommandDefinitionByState(entity);");
            content.AppendLine($"");
            content.AppendLine($"\t\t\t return CommandDefinitions;");
            content.AppendLine("\t\t}");
            content.AppendLine();
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
    }
}