using BestPracticesCodeGenerator.Dtos;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;


namespace BestPracticesCodeGenerator
{
    /// <summary>
    /// Interaction logic for frmCodeGenerationOptionsControl.
    /// </summary>
    public partial class frmCodeGenerationOptionsControl : UserControl
    {
        public Solution Solution { get; set; }
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

        private string GetDapperCommandProviderPath(Solution solution, string originalFilePath)
        {
            var commandProviderProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Cqrs.Dapper"))
               .FirstOrDefault();

            var commandProvidersFolder = commandProviderProject?.Children.Where(n => n.Text.Equals("CommandProviders"))
                .FirstOrDefault();

            return (commandProvidersFolder is not null) ? commandProvidersFolder.FullPath : originalFilePath;
        }

        private string GetOutputBuilderPath(Solution solution, string originalFilePath)
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

        private string GetInputBuilderPath(Solution solution, string originalFilePath)
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

        private string GetInputValidatorTestsPath(Solution solution, string originalFilePath)
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

        private string GetDapperTableDefinitionPath(Solution solution, string originalFilePath)
        {
            var commandProviderProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Cqrs.Dapper"))
               .FirstOrDefault();

            var tableDefinitionsFolder = commandProviderProject?.Children.Where(n => n.Text.Equals("TableDefinitions"))
                .FirstOrDefault();

            return (tableDefinitionsFolder is not null) ? tableDefinitionsFolder.FullPath : originalFilePath;
        }

        private string GetInputValidatorPath(Solution solution, string originalFilePath)
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

        private string GetEntityTestsPath(Solution solution, string originalFilePath)
        {
            var coreTestsProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
               .FirstOrDefault();

            var domainFolder = coreTestsProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var modelsFolder = domainFolder?.Children.Where(n => n.Text.Equals("Models"))
                .FirstOrDefault();

            return (modelsFolder is not null) ? modelsFolder.FullPath : originalFilePath;
        }

        private string GetUseCasesTestsPath(Solution solution, string originalFilePath)
        {
            var coreTestsProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
                .FirstOrDefault();

            var applicationFolder = coreTestsProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var useCasesFolder = applicationFolder?.Children.Where(n => n.Text.Equals("UseCases"))
                .FirstOrDefault();

            return (useCasesFolder is not null) ? useCasesFolder.FullPath : originalFilePath;
        }

        private string GetDapperCommandPath(Solution solution, string originalFilePath)
        {
            var commandProviderProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Cqrs.Dapper"))
               .FirstOrDefault();

            var entityCommandsFolder = commandProviderProject?.Children.Where(n => n.Text.Equals("EntityCommands"))
                .FirstOrDefault();

            return (entityCommandsFolder is not null) ? entityCommandsFolder.FullPath : originalFilePath;
        }

        private string GetOutputDtoPath(Solution solution, string originalFilePath)
        {
            return GetInputDtoPath(solution, originalFilePath);
        }

        private string GetInputDtoPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                  .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            return (dtosFolder is not null) ? dtosFolder.FullPath : originalFilePath;
        }

        private string GetUseCasesPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                  .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var useCasesFolder = applicationFolder?.Children.Where(n => n.Text.Equals("UseCases"))
                .FirstOrDefault();

            return (useCasesFolder is not null) ? useCasesFolder.FullPath : originalFilePath;
        }

        private string GetRepositoryPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                 .FirstOrDefault();

            var domainFolder = coreProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var repositoryFolder = domainFolder?.Children.Where(n => n.Text.Equals("Repositories"))
                .FirstOrDefault();

            return (repositoryFolder is not null) ? repositoryFolder.FullPath : originalFilePath;
        }

        private string GetInterfaceCommandProviderPath(Solution solution, string originalFilePath)
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

        private string GetInterfaceRepositoryPath(Solution solution, string originalFilePath)
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

            newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "InputValidator.cs");
            filePath = GetInputValidatorPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputValidatorFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "InputBuilder.cs");
            filePath = GetInputBuilderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputBuilderFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "InputValidatorTests.cs");
            filePath = GetInputValidatorTestsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputValidatorTestsFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "UseCase.cs");
            filePath = GetUseCasesPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateUseCaseFactory.Create(FileContent, ClassProperties, filePath));


            newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "UseCaseTests.cs");
            filePath = GetUseCasesTestsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), CreateUseCaseTestsFactory.Create(FileContent, ClassProperties, filePath));
        }

        private void GenerateUpdateUseCaseAsync()
        {
            var filePath = GetInputDtoPath(Solution, OriginalFilePath);
            var newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "Input.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputDtoFactory.Create(FileContent, ClassProperties, filePath));


            filePath = GetInputValidatorPath(Solution, OriginalFilePath);
            newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "InputValidator.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputValidatorFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "InputValidatorTests.cs");
            filePath = GetInputValidatorTestsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputValidatorTestsFactory.Create(FileContent, ClassProperties, filePath));


            newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "InputBuilder.cs");
            filePath = GetInputBuilderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputBuilderFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "UseCase.cs");
            filePath = GetUseCasesPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateUseCaseFactory.Create(FileContent, ClassProperties, filePath));

            filePath = GetUseCasesTestsPath(Solution, OriginalFilePath);
            newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "UseCaseTests.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateUseCaseTestsFactory.Create(FileContent, ClassProperties, filePath));

        }

        private void GenerateDeleteUseCaseAsync()
        {
            var newFileName = string.Concat("Delete", FileName.Substring(0, FileName.Length - 3), "UseCase.cs");
            var filePath = GetUseCasesPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DeleteUseCaseFactory.Create(FileContent, ClassProperties, filePath));

            filePath = GetUseCasesTestsPath(Solution, OriginalFilePath);
            newFileName = string.Concat("Delete", FileName.Substring(0, FileName.Length - 3), "UseCaseTests.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), DeleteUseCaseTestsFactory.Create(FileContent, ClassProperties, filePath));
        }

        private async Task GenerateAsync()
        {
            var newFileName = "";

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Builder.cs");
            var filePath = GetEntityBuilderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), EntityBuilderFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat("I", FileName.Substring(0, FileName.Length - 3), "Repository.cs");
            filePath = GetInterfaceRepositoryPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceRepositoryFactory.Create(FileContent, ClassProperties, filePath));


            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Repository.cs");
            filePath = GetRepositoryPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), RepositoryFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Command.cs");
            filePath = GetDapperCommandPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperCommandFactory.Create(FileContent, filePath));

            newFileName = string.Concat("I", FileName.Substring(0, FileName.Length - 3), "CommandProvider.cs");
            filePath = GetInterfaceCommandProviderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceCommandProviderFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "CqrsCommandProvider.cs");
            filePath = GetDapperCommandProviderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperCommandProviderFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat("Dapper", FileName.Substring(0, FileName.Length - 3), "TableDefinition.cs");
            filePath = GetDapperTableDefinitionPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperTableDefinitionFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Tests.cs");
            filePath = GetEntityTestsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), EntityTestsFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat(GetTimeStamp(), "CreateTable", FileName.Substring(0, FileName.Length - 3), ".sql");
            filePath = GetMigratonsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), MigrationCreateTableFactory.Create(FileContent, ClassProperties, filePath));

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

                newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "OutputBuilder.cs");
                filePath = GetOutputBuilderPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), OutputBuilderFactory.Create(FileContent, ClassProperties, filePath));

                newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Controller.cs");
                filePath = GetControllerPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), ControllerFactory.Create(FileContent, ClassProperties, filePath, SEL_GenerateCreateUseCase.IsChecked.Value, SEL_GenerateUpdateUseCase.IsChecked.Value, SEL_GenerateDeleteUseCase.IsChecked.Value, SEL_GenerateGetUseCase.IsChecked.Value));
            }

            await VS.MessageBox.ShowWarningAsync("MyCommand", "Classes generated with success!");
        }

        private void GenerateGetUseCaseAsync()
        {
            var newFileName = string.Concat("Get", FileName.Substring(0, FileName.Length - 3), "ByIdUseCase.cs");
            var filePath = GetUseCasesPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), GetByIdUseCaseFactory.Create(FileContent, ClassProperties, filePath));

            filePath = GetUseCasesTestsPath(Solution, OriginalFilePath);
            newFileName = string.Concat("Get", FileName.Substring(0, FileName.Length - 3), "ByIdUseCaseTests.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), GetByIdUseCaseFactoryTestsFactory.Create(FileContent, ClassProperties, filePath));

            filePath = GetInterfaceQueryProviderPath(Solution, OriginalFilePath);
            newFileName = string.Concat("I", FileName.Substring(0, FileName.Length - 3), "CqrsQueryProvider.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceQueryProviderFactory.Create(FileContent, ClassProperties, filePath));

            filePath = GetDapperQueryProviderPath(Solution, OriginalFilePath);
            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "CqrsQueryProvider.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperQueryProviderFactory.Create(FileContent, ClassProperties, filePath));

            filePath = GetOutputBuilderPath(Solution, OriginalFilePath);
            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "ListItemOutputBuilder.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), ListItemOutputBuilderFactory.Create(FileContent, ClassProperties, filePath));

            filePath = GetOutputDtoPath(Solution, OriginalFilePath);
            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "ListItemOutput.cs");
            File.WriteAllText(Path.Combine(filePath, newFileName), ListItemOutputFactory.Create(FileContent, ClassProperties, filePath));

        }

        private string GetInterfaceQueryProviderPath(Solution solution, string originalFilePath)
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

        private string GetDapperQueryProviderPath(Solution solution, string originalFilePath)
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

        private string GetMigratonsPath(Solution solution, string originalFilePath)
        {
            var presentationProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Cqrs.Dapper"))
              .FirstOrDefault();

            var migrationsFolder = presentationProject?.Children.Where(n => n.Text.Equals("Migrations"))
                .FirstOrDefault();

            var sctructuralScriptsFolder = migrationsFolder?.Children.Where(n => n.Text.Equals("SctructuralScripts"))
                .FirstOrDefault();

            return (sctructuralScriptsFolder is not null) ? sctructuralScriptsFolder.FullPath : originalFilePath;
        }

        private string GetControllerPath(Solution solution, string originalFilePath)
        {
            var presentationProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Presentation.AspNetCoreApi"))
               .FirstOrDefault();

            var controllersFolder = presentationProject?.Children.Where(n => n.Text.Equals("Controllers"))
                .FirstOrDefault();

            return (controllersFolder is not null) ? controllersFolder.FullPath : originalFilePath;
        }

        private string GetEntityBuilderPath(Solution solution, string originalFilePath)
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

            foreach (Match item in Regex.Matches(FileContent, @"(?>public)\s+(?!class)((static|readonly)\s)?(?<Type>(\S+(?:<.+?>)?)(?=\s+\w+\s*\{\s*get))\s+(?<Name>[^\s]+)(?=\s*\{\s*get)"))
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

            this.ClassProperties = GetPropertiesInfo();
            GRD_Properties.ItemsSource = this.ClassProperties;
            BTN_Generate.IsEnabled = true;
            LBL_ClassProperties.Text = "Properties from " + FileName.Replace(":", "").Replace(".cs", "");
        }
    }
}