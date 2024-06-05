using System.IO;
using System.Linq;

namespace BestPracticesCodeGenerator
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var doc = await VS.Documents.GetActiveDocumentViewAsync();
            var span = doc.TextView.Selection.SelectedSpans.FirstOrDefault();
            var originalFileName = doc.FilePath;

            var fileContent = File.ReadAllText(originalFileName);

            var filePath = Path.GetDirectoryName(originalFileName);
            var fileName = Path.GetFileName(originalFileName);

            var newFileName = string.Concat("I", fileName.Substring(0, fileName.Length - 3), "Repository.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceRepositoryFactory.Create(fileContent));

            newFileName = string.Concat("I", fileName.Substring(0, fileName.Length - 3), "CommandProvider.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceCommandProviderFactory.Create(fileContent));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "Repository.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), RepositoryFactory.Create(fileContent));

            newFileName = string.Concat("Create", fileName.Substring(0, fileName.Length - 3), "UseCase.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), CreateUseCaseFactory.Create(fileContent));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "UseCase.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateUseCaseFactory.Create(fileContent));

            newFileName = string.Concat("Create", fileName.Substring(0, fileName.Length - 3), "Input.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputDtoFactory.Create(fileContent));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "Input.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputDtoFactory.Create(fileContent));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "Output.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), OutputDtoFactory.Create(fileContent));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "Command.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), DapperCommandFactory.Create(fileContent));

            newFileName = string.Concat("Create", fileName.Substring(0, fileName.Length - 3), "UseCaseTests.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), CreateUseCaseTestsFactory.Create(fileContent));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "UseCaseTests.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateUseCaseTestsFactory.Create(fileContent));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "InputValidator.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), InputValidatorFactory.Create(fileContent));

            newFileName = string.Concat("Dapper", fileName.Substring(0, fileName.Length - 3), "TableDefinition.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), DapperTableDefinitionFactory.Create(fileContent));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "InputValidatorTests.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), InputValidatorTestsFactory.Create(fileContent));

            newFileName = string.Concat("Create", fileName.Substring(0, fileName.Length - 3), "InputBuilder.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputBuilderFactory.Create(fileContent));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "InputBuilder.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputBuilderFactory.Create(fileContent));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "InputBuilder.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), OutputBuilderFactory.Create(fileContent));

            await VS.MessageBox.ShowWarningAsync("MyCommand", doc.WindowFrame.Caption);
        }
    }
}
