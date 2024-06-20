using BestPracticesCodeGenerator.Dtos;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace BestPracticesCodeGenerator
{
    /// <summary>
    /// Interaction logic for frmCodeGenerationOptionsControl.
    /// </summary>
    public partial class frmCodeGenerationOptionsControl : UserControl
    {
        private DTE2 _dte;

        public Community.VisualStudio.Toolkit.Solution Solution { get; set; }
        public string OriginalFileName { get; set; }
        public string FileContent { get; set; }
        public string OriginalFilePath { get; set; }
        public string FileName { get; set; }
        public IList<PropertyInfo> ClassProperties { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="frmCodeGenerationOptionsControl"/> class.
        /// </summary>
        public frmCodeGenerationOptionsControl()
        {
            this.InitializeComponent();
            ThreadHelper.ThrowIfNotOnUIThread();
            _dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;
        }

        public void WriteFileNewCreatedFileMessageToOutPutWindow(string message, bool blankLine = false)
        {
            WriteToOutputWindow(message, blankLine);
        }

        public void WriteToOutputWindow(string message, bool blankLine = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            EnvDTE.OutputWindow outputWindow = _dte.ToolWindows.OutputWindow;

            EnvDTE.OutputWindowPane pane = null;
            try
            {
                foreach (EnvDTE.OutputWindowPane p in outputWindow.OutputWindowPanes)
                {
                    if (p.Name == "Best.Practices generator")
                    {
                        pane = p;
                        break;
                    }
                }

                if (pane == null)
                {
                    pane = outputWindow.OutputWindowPanes.Add("Best.Practices generator");
                }
            }
            catch (COMException)
            {

            }

            pane?.Activate();
            pane?.OutputString(message + "\n");
        }


        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void BTN_Generate_Click(object sender, RoutedEventArgs e)
        {
            GenerateAsync().Wait();
        }

        private string GetDapperCommandProviderPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var commandProviderProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Cqrs.Dapper"))
               .FirstOrDefault();

            var commandProvidersFolder = commandProviderProject?.Children.Where(n => n.Text.Equals("CommandProviders"))
                .FirstOrDefault();

            return (commandProvidersFolder is not null) ? commandProvidersFolder.FullPath : originalFilePath;
        }

        private string GetOutputBuilderPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
                .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            var buildersFolder = dtosFolder?.Children.Where(n => n.Text.Equals("Builders"))
               .FirstOrDefault();

            return (buildersFolder is not null) ? buildersFolder.FullPath : originalFilePath;
        }

        private string GetInputBuilderPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
                 .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            var buildersFolder = dtosFolder?.Children.Where(n => n.Text.Equals("Builders"))
               .FirstOrDefault();

            return (buildersFolder is not null) ? buildersFolder.FullPath : originalFilePath;
        }

        private string GetInputValidatorTestsPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreTestsProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
              .FirstOrDefault();

            var applicationFolder = coreTestsProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            var validatorsFolder = dtosFolder?.Children.Where(n => n.Text.Equals("Validators"))
               .FirstOrDefault();

            return (validatorsFolder is not null) ? validatorsFolder.FullPath : originalFilePath;
        }

        private string GetDapperTableDefinitionPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var commandProviderProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Cqrs.Dapper"))
               .FirstOrDefault();

            var tableDefinitionsFolder = commandProviderProject?.Children.Where(n => n.Text.Equals("TableDefinitions"))
                .FirstOrDefault();

            return (tableDefinitionsFolder is not null) ? tableDefinitionsFolder.FullPath : originalFilePath;
        }

        private string GetInputValidatorPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                 .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            var validatorsFolder = dtosFolder?.Children.Where(n => n.Text.Equals("Validators"))
               .FirstOrDefault();

            return (validatorsFolder is not null) ? validatorsFolder.FullPath : originalFilePath;
        }

        private string GetEntityTestsPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreTestsProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
               .FirstOrDefault();

            var domainFolder = coreTestsProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var modelsFolder = domainFolder?.Children.Where(n => n.Text.Equals("Models"))
                .FirstOrDefault();

            return (modelsFolder is not null) ? modelsFolder.FullPath : originalFilePath;
        }

        private string GetUseCasesTestsPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreTestsProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
                .FirstOrDefault();

            var applicationFolder = coreTestsProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var useCasesFolder = applicationFolder?.Children.Where(n => n.Text.Equals("UseCases"))
                .FirstOrDefault();

            return (useCasesFolder is not null) ? useCasesFolder.FullPath : originalFilePath;
        }

        private string GetDapperCommandPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var commandProviderProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Cqrs.Dapper"))
               .FirstOrDefault();

            var entityCommandsFolder = commandProviderProject?.Children.Where(n => n.Text.Equals("EntityCommands"))
                .FirstOrDefault();

            return (entityCommandsFolder is not null) ? entityCommandsFolder.FullPath : originalFilePath;
        }

        private string GetOutputDtoPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            return GetInputDtoPath(Solution, OriginalFilePath);
        }

        private string GetInputDtoPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                  .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            return (dtosFolder is not null) ? dtosFolder.FullPath : originalFilePath;
        }

        private string GetUseCasesPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                  .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var useCasesFolder = applicationFolder?.Children.Where(n => n.Text.Equals("UseCases"))
                .FirstOrDefault();

            return (useCasesFolder is not null) ? useCasesFolder.FullPath : originalFilePath;
        }

        private string GetRepositoryPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                 .FirstOrDefault();

            var domainFolder = coreProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var repositoryFolder = domainFolder?.Children.Where(n => n.Text.Equals("Repositories"))
                .FirstOrDefault();

            return (repositoryFolder is not null) ? repositoryFolder.FullPath : originalFilePath;
        }

        private string GetInterfaceCommandProviderPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
               .FirstOrDefault();

            var domainFolder = coreProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var cqrsFolder = domainFolder?.Children.Where(n => n.Text.Equals("Cqrs"))
                .FirstOrDefault();

            var commandProvidersFolder = cqrsFolder?.Children.Where(n => n.Text.Equals("CommandProviders"))
                .FirstOrDefault();

            return (commandProvidersFolder is not null) ? commandProvidersFolder.FullPath : originalFilePath;
        }

        private string GetInterfaceRepositoryPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                .FirstOrDefault();

            var domainFolder = coreProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var repositoryFolder = domainFolder?.Children.Where(n => n.Text.Equals("Repositories"))
                .FirstOrDefault();

            var repositoryInterfaceFolder = repositoryFolder?.Children.Where(n => n.Text.Equals("Interfaces"))
                .FirstOrDefault();

            return (repositoryInterfaceFolder is not null) ? repositoryInterfaceFolder.FullPath : originalFilePath;
        }

        private void GenerateCreateUseCaseAsync()
        {
            var newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "Input.cs");
            var filePath = GetInputDtoPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputDtoFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "InputValidator.cs");
            filePath = GetInputValidatorPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputValidatorFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "InputBuilder.cs");
            filePath = GetInputBuilderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputBuilderFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "InputValidatorTests.cs");
            filePath = GetInputValidatorTestsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputValidatorTestsFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "UseCase.cs");
            filePath = GetUseCasesPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateUseCaseFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "UseCaseTests.cs");
            filePath = GetUseCasesTestsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateUseCaseTestsFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));
        }

        private void GenerateUpdateUseCaseAsync()
        {
            var filePath = GetInputDtoPath(Solution, OriginalFilePath);
            var newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "Input.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputDtoFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));


            filePath = GetInputValidatorPath(Solution, OriginalFilePath);
            newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "InputValidator.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputValidatorFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "InputValidatorTests.cs");
            filePath = GetInputValidatorTestsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputValidatorTestsFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));


            newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "InputBuilder.cs");
            filePath = GetInputBuilderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputBuilderFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "UseCase.cs");
            filePath = GetUseCasesPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateUseCaseFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            filePath = GetUseCasesTestsPath(Solution, OriginalFilePath);
            newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "UseCaseTests.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateUseCaseTestsFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

        }

        private void GenerateDeleteUseCaseAsync()
        {
            var newFileName = string.Concat("Delete", FileName.Substring(0, FileName.Length - 3), "UseCase.cs");
            var filePath = GetUseCasesPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DeleteUseCaseFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            filePath = GetUseCasesTestsPath(Solution, OriginalFilePath);
            newFileName = string.Concat("Delete", FileName.Substring(0, FileName.Length - 3), "UseCaseTests.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), DeleteUseCaseTestsFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));
        }

        private async Task GenerateAsync()
        {
            var newFileName = "";

            WriteFileNewCreatedFileMessageToOutPutWindow("Files below were automatically generated...");
            WriteFileNewCreatedFileMessageToOutPutWindow("", true);

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Builder.cs");
            var filePath = GetEntityBuilderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), EntityBuilderFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat("I", FileName.Substring(0, FileName.Length - 3), "Repository.cs");
            filePath = GetInterfaceRepositoryPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceRepositoryFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Repository.cs");
            filePath = GetRepositoryPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), RepositoryFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Command.cs");
            filePath = GetDapperCommandPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperCommandFactory.Create(FileContent, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat("I", FileName.Substring(0, FileName.Length - 3), "CommandProvider.cs");
            filePath = GetInterfaceCommandProviderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceCommandProviderFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "CqrsCommandProvider.cs");
            filePath = GetDapperCommandProviderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperCommandProviderFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat("Dapper", FileName.Substring(0, FileName.Length - 3), "TableDefinition.cs");
            filePath = GetDapperTableDefinitionPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperTableDefinitionFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Tests.cs");
            filePath = GetEntityTestsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), EntityTestsFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            newFileName = string.Concat(GetTimeStamp(), "CreateTable", FileName.Substring(0, FileName.Length - 3), ".sql");
            filePath = GetMigratonsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), MigrationCreateTableFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            if (SEL_GenerateCreateUseCase.IsChecked.Value)
            {
                GenerateCreateUseCaseAsync();
            }

            if (SEL_GenerateUpdateUseCase.IsChecked.Value)
            {
                GenerateUpdateUseCaseAsync();
            }

            if (SEL_GenerateDeleteUseCase.IsChecked.Value)
            {
                GenerateDeleteUseCaseAsync();
            }

            if (SEL_GenerateGetUseCase.IsChecked.Value)
            {
                GenerateGetUseCaseAsync();
            }

            if ((SEL_GenerateCreateUseCase.IsChecked.Value) || SEL_GenerateUpdateUseCase.IsChecked.Value || SEL_GenerateDeleteUseCase.IsChecked.Value || SEL_GenerateGetUseCase.IsChecked.Value)
            {
                newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Output.cs");
                filePath = GetOutputDtoPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), OutputDtoFactory.Create(FileContent, ClassProperties, filePath));
                WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

                newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "OutputBuilder.cs");
                filePath = GetOutputBuilderPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), OutputBuilderFactory.Create(FileContent, ClassProperties, filePath));
                WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

                newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Controller.cs");
                filePath = GetControllerPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), ControllerFactory.Create(FileContent, ClassProperties, filePath, SEL_GenerateCreateUseCase.IsChecked.Value, SEL_GenerateUpdateUseCase.IsChecked.Value, SEL_GenerateDeleteUseCase.IsChecked.Value, SEL_GenerateGetUseCase.IsChecked.Value));
                WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));
            }

            WriteFileNewCreatedFileMessageToOutPutWindow("", true);
            WriteFileNewCreatedFileMessageToOutPutWindow("Double click on file path to navigate.");
            WriteFileNewCreatedFileMessageToOutPutWindow("Please, remember to finish coding with your specific needs...");

            await VS.MessageBox.ShowWarningAsync("Best.Practices generator", "Files generated with success.\nCheck out output window for details.");
        }

        private void GenerateGetUseCaseAsync()
        {
            var newFileName = string.Concat("Get", FileName.Substring(0, FileName.Length - 3), "ByIdUseCase.cs");
            var filePath = GetUseCasesPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), GetByIdUseCaseFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            filePath = GetUseCasesTestsPath(Solution, OriginalFilePath);
            newFileName = string.Concat("Get", FileName.Substring(0, FileName.Length - 3), "ByIdUseCaseTests.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), GetByIdUseCaseFactoryTestsFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            filePath = GetInterfaceQueryProviderPath(Solution, OriginalFilePath);
            newFileName = string.Concat("I", FileName.Substring(0, FileName.Length - 3), "CqrsQueryProvider.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceQueryProviderFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            filePath = GetDapperQueryProviderPath(Solution, OriginalFilePath);
            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "CqrsQueryProvider.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperQueryProviderFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            filePath = GetOutputBuilderPath(Solution, OriginalFilePath);
            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "ListItemOutputBuilder.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), ListItemOutputBuilderFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

            filePath = GetOutputDtoPath(Solution, OriginalFilePath);
            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "ListItemOutput.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), ListItemOutputFactory.Create(FileContent, ClassProperties, filePath));
            WriteFileNewCreatedFileMessageToOutPutWindow(Path.Combine(filePath, newFileName));

        }

        private string GetInterfaceQueryProviderPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var presentationProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                .FirstOrDefault();

            var applicationFolder = presentationProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var cqrsFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Cqrs"))
                .FirstOrDefault();

            var queryProvidersFolder = cqrsFolder?.Children.Where(n => n.Text.Equals("QueryProviders"))
                .FirstOrDefault();

            return (queryProvidersFolder is not null) ? queryProvidersFolder.FullPath : originalFilePath;
        }

        private string GetDapperQueryProviderPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var presentationProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Cqrs.Dapper"))
             .FirstOrDefault();

            var queryProvidersFolder = presentationProject?.Children.Where(n => n.Text.Equals("QueryProviders"))
                .FirstOrDefault();

            return (queryProvidersFolder is not null) ? queryProvidersFolder.FullPath : originalFilePath;
        }

        private string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmss");
        }

        private string GetMigratonsPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var presentationProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Cqrs.Dapper"))
              .FirstOrDefault();

            var migrationsFolder = presentationProject?.Children.Where(n => n.Text.Equals("Migrations"))
                .FirstOrDefault();

            var sctructuralScriptsFolder = migrationsFolder?.Children.Where(n => n.Text.Equals("SctructuralScripts"))
                .FirstOrDefault();

            return (sctructuralScriptsFolder is not null) ? sctructuralScriptsFolder.FullPath : originalFilePath;
        }

        private string GetControllerPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var presentationProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Presentation.AspNetCoreApi"))
               .FirstOrDefault();

            var controllersFolder = presentationProject?.Children.Where(n => n.Text.Equals("Controllers"))
                .FirstOrDefault();

            return (controllersFolder is not null) ? controllersFolder.FullPath : originalFilePath;
        }

        private string GetEntityBuilderPath(Community.VisualStudio.Toolkit.Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
                .FirstOrDefault();

            var domainFolder = coreProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var modelsFolder = domainFolder?.Children.Where(n => n.Text.Equals("Models"))
                .FirstOrDefault();

            var buildersFolder = modelsFolder?.Children.Where(n => n.Text.Equals("Builders"))
               .FirstOrDefault();

            return (buildersFolder is not null) ? buildersFolder.FullPath : originalFilePath;
        }

        public IList<PropertyInfo> GetPropertiesInfo()
        {
            var propertyes = new List<PropertyInfo>();

            var matches = Regex.Matches(FileContent, @"(?>public|protected|internal|private)\s+(static\s+|readonly\s+|virtual\s+|override\s+)*?(?<Type>\S+(?:<.+?>)?)\s+(?<Name>[^\s]+)(?=\s*\{\s*get)");

            foreach (Match item in matches)
            {
                propertyes.Add(new PropertyInfo(item.Groups["Type"].Value, item.Groups["Name"].Value));
            }

            return propertyes;
        }

        private void BTN_Reload_Click(object sender, RoutedEventArgs e)
        {
            this.FileContent = File.ReadAllText(OriginalFileName);

            this.OriginalFilePath = Path.GetDirectoryName(OriginalFileName);
            this.FileName = Path.GetFileName(OriginalFileName);

            this.ClassProperties = new ObservableCollection<PropertyInfo>(GetPropertiesInfo());
            GRD_Properties.ItemsSource = this.ClassProperties;
            BTN_Generate.IsEnabled = true;
            LBL_ClassProperties.Text = "Properties from " + FileName.Replace(":", "").Replace(".cs", "");
        }

        private void SEL_GenerateAllUseCase_Checked(object sender, RoutedEventArgs e)
        {
            SEL_GenerateCreateUseCase.IsChecked = true;
            SEL_GenerateUpdateUseCase.IsChecked = true;
            SEL_GenerateDeleteUseCase.IsChecked = true;
            SEL_GenerateGetUseCase.IsChecked = true;
        }

        private void SEL_GenerateAllUseCase_Unchecked(object sender, RoutedEventArgs e)
        {
            SEL_GenerateCreateUseCase.IsChecked = false;
            SEL_GenerateUpdateUseCase.IsChecked = false;
            SEL_GenerateDeleteUseCase.IsChecked = false;
            SEL_GenerateGetUseCase.IsChecked = false;
        }

        private void BTN_GoToEntity_Click(object sender, RoutedEventArgs e)
        {
            VS.Documents.OpenAsync(OriginalFileName).Wait();
        }

        private void SimulateKeyPress(Key key)
        {
            // Cria uma simulação do KeyEvent
            KeyEventArgs keyEventArgs = new KeyEventArgs(
                Keyboard.PrimaryDevice,
                PresentationSource.FromVisual(this),
                0,
                key)
            {
                RoutedEvent = Keyboard.KeyDownEvent
            };

            // Dispara o evento
            InputManager.Current.ProcessInput(keyEventArgs);
        }

        private void GRD_Properties_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
                return;

            if (e.Delta > 0)
            {
                SimulateKeyPress(Key.Up);
            }
            // Scroll down
            else if (e.Delta < 0)
            {
                SimulateKeyPress(Key.Down);
            }

            // Marcar o evento como manipulado para evitar processamento adicional
            e.Handled = true;
        }
    }
}