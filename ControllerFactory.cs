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
    public static class ControllerFactory
    {
        public static string Create(string fileContent, IList<PropertyInfo> classProperties, string filePath,
            bool generateCreateUseCase,
            bool generateUpdateUseCase,
            bool generateDeleteUseCase,
            bool generateGetByIdUseCase)
        {
            Validate(fileContent);

            if (!classProperties.Any())
                throw new ValidationException("It wasn't identified public properties to generate builder class");

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateUseCaseClass(fileContent, originalClassName, classProperties, generateCreateUseCase, generateUpdateUseCase, generateDeleteUseCase, generateGetByIdUseCase, filePath);
        }

        private static string CreateUseCaseClass(
            string fileContent,
            string originalClassName,
            IList<PropertyInfo> properties,
            bool generateCreateUseCase,
            bool generateUpdateUseCase,
            bool generateDeleteUseCase,
            bool generateGetByIdUseCase,
            string filePath)
        {
            var content = new StringBuilder();

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine($"using Microsoft.AspNetCore.Mvc;");
            content.AppendLine($"using Best.Practices.Core.Presentation.AspNetCoreApi.Controllers;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Application.Dtos;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Application.UseCases;");
            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

            content.AppendLine("{");

            var newClassName = string.Concat(originalClassName, "Controller");

            content.AppendLine($"\t[Route(\"api/{originalClassName.GetWordWithFirstLetterDown()}s\")]");
            content.AppendLine("\t[ApiController]");

            content.AppendLine(string.Concat("\tpublic class ", originalClassName, "Controller : BaseController"));

            content.AppendLine("\t{");

            GeneratePrivateVariables(content, originalClassName, generateCreateUseCase, generateUpdateUseCase, generateDeleteUseCase, generateGetByIdUseCase);

            GenerateControllerConstructor(content, originalClassName, newClassName, generateCreateUseCase, generateUpdateUseCase, generateDeleteUseCase, generateGetByIdUseCase);

            GenerateApiMethods(content, originalClassName, properties, generateCreateUseCase, generateUpdateUseCase, generateDeleteUseCase, generateGetByIdUseCase);

            content.AppendLine("\t}");

            content.AppendLine("}");

            return content.ToString();
        }

        private static void Validate(string fileContent)
        {
            if (fileContent.IndexOf("namespace ") < 0)
                throw new ValidationException("The file selected is not valid.");
        }

        private static void GenerateControllerConstructor(StringBuilder content, string originalClassName, string newClassName, bool generateCreateUseCase, bool generateUpdateUseCase, bool generateDeleteUseCase, bool generateGetByIdUseCase)
        {
            content.AppendLine();
            content.AppendLine($"\t\tpublic {newClassName}(");
            content.AppendLine($"\t\t\tIHttpContextAccessor httpContextAccessor,");

            if (generateCreateUseCase)
            {
                var parentesis = (!generateUpdateUseCase && !generateDeleteUseCase && !generateGetByIdUseCase) ? ")" : "";

                content.AppendLine($"\t\t\tCreate{originalClassName}UseCase create{originalClassName}UseCase" + ((generateUpdateUseCase || generateDeleteUseCase || generateGetByIdUseCase) ? "," : "") + parentesis);
            }

            if (generateUpdateUseCase)
            {
                var parentesis = (!generateDeleteUseCase && !generateGetByIdUseCase) ? ")" : "";

                content.AppendLine($"\t\t\tUpdate{originalClassName}UseCase update{originalClassName}UseCase" + (generateDeleteUseCase || generateGetByIdUseCase ? "," : "") + parentesis);
            }

            if (generateDeleteUseCase)
            {
                var parentesis = (!generateGetByIdUseCase) ? ")" : "";

                content.AppendLine($"\t\t\tDelete{originalClassName}UseCase delete{originalClassName}UseCase" + (generateGetByIdUseCase ? "," : "") + parentesis);
            }

            if (generateGetByIdUseCase)
            {
                content.AppendLine($"\t\t\tGet{originalClassName}ByIdUseCase get{originalClassName}ByIdUseCase)");
            }

            content.AppendLine("\t\t\t: base(httpContextAccessor)");
            content.AppendLine("\t\t\t{");
            if (generateCreateUseCase)
                content.AppendLine($"\t\t\t\t_create{originalClassName}UseCase = create{originalClassName}UseCase;");

            if (generateUpdateUseCase)
                content.AppendLine($"\t\t\t\t_update{originalClassName}UseCase = update{originalClassName}UseCase;");

            if (generateDeleteUseCase)
                content.AppendLine($"\t\t\t\t_delete{originalClassName}UseCase = delete{originalClassName}UseCase;");

            if (generateGetByIdUseCase)
                content.AppendLine($"\t\t\t\t_get{originalClassName}ByIdUseCase = get{originalClassName}ByIdUseCase;");

            content.AppendLine("\t\t\t}");
            content.AppendLine("");
        }

        private static void GenerateApiMethods(
            StringBuilder content,
            string originalClassName,
            IList<PropertyInfo> properties,
            bool generateCreateUseCase,
            bool generateUpdateUseCase,
            bool generateDeleteUseCase,
            bool generateGetByIdUseCase)
        {
            if (generateCreateUseCase)
            {
                content.AppendLine($"\t\t\t[HttpPost]");
                content.AppendLine($"\t\t\t[ProducesResponseType(typeof({originalClassName}Output), StatusCodes.Status201Created)]");
                content.AppendLine($"\t\t\tpublic async Task<IActionResult> Create{originalClassName}(Create{originalClassName}Input input)");
                content.AppendLine("\t\t\t{");
                content.AppendLine($"\t\t\t\treturn OutputConverter(await _create{originalClassName}UseCase.ExecuteAsync(input));");
                content.AppendLine("\t\t\t}");
                content.AppendLine("");
            }

            if (generateUpdateUseCase)
            {

                content.AppendLine($"\t\t\t[HttpPut(\"{{id}}\")]");
                content.AppendLine($"\t\t\t[ProducesResponseType(typeof({originalClassName}Output), StatusCodes.Status201Created)]");
                content.AppendLine($"\t\t\tpublic async Task<IActionResult> Update{originalClassName}(Guid id, Update{originalClassName}Input input)");
                content.AppendLine("\t\t\t{");
                content.AppendLine("\t\t\t\tinput.Id = id;");
                content.AppendLine("");
                content.AppendLine($"\t\t\t\treturn OutputConverter(await _update{originalClassName}UseCase.ExecuteAsync(input));");
                content.AppendLine("\t\t\t}");
                content.AppendLine("");
            }

            if (generateDeleteUseCase)
            {
                content.AppendLine($"\t\t\t[HttpDelete(\"{{id}}\")]");
                content.AppendLine($"\t\t\t[ProducesResponseType(typeof({originalClassName}Output), StatusCodes.Status200OK)]");
                content.AppendLine($"\t\t\tpublic async Task<IActionResult> Delete{originalClassName}(Guid id)");
                content.AppendLine("\t\t\t{");
                content.AppendLine($"\t\t\t\treturn OutputConverter(await _delete{originalClassName}UseCase.ExecuteAsync(id));");
                content.AppendLine("\t\t\t}");
                content.AppendLine();
            }

            if (generateGetByIdUseCase)
            {
                content.AppendLine($"\t\t\t[HttpGet(\"{{id}}\")]");
                content.AppendLine($"\t\t\t[ProducesResponseType(typeof({originalClassName}Output), StatusCodes.Status200OK)]");
                content.AppendLine($"\t\t\tpublic async Task<IActionResult> Get{originalClassName}ById(Guid id)");
                content.AppendLine("\t\t\t{");
                content.AppendLine($"\t\t\t\treturn OutputConverter(await _get{originalClassName}ByIdUseCase.ExecuteAsync(id));");
                content.AppendLine("\t\t\t}");
                content.AppendLine();
            }
        }

        private static void GeneratePrivateVariables(StringBuilder content, string originalClassName, bool generateCreateUseCase, bool generateUpdateUseCase, bool generateDeleteUseCase, bool generateGetByIdUseCase)
        {
            if (generateCreateUseCase)
                content.AppendLine($"\t\tprivate readonly Create{originalClassName}UseCase _create{originalClassName}UseCase;");

            if (generateUpdateUseCase)
                content.AppendLine($"\t\tprivate readonly Update{originalClassName}UseCase _update{originalClassName}UseCase;");

            if (generateDeleteUseCase)
                content.AppendLine($"\t\tprivate readonly Delete{originalClassName}UseCase _delete{originalClassName}UseCase;");

            if (generateGetByIdUseCase)
                content.AppendLine($"\t\tprivate readonly Get{originalClassName}ByIdUseCase _get{originalClassName}ByIdUseCase;");

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