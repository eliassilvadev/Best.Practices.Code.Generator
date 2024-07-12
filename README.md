# Best Pratices Generator

A Visual Studio extension that generates the code from clean architecture approach based on an entity class and its properties.

Download the extension at the
[VS Gallery](https://visualstudiogallery.msdn.microsoft.com/XXXXXXXXXXXXX)

## What does it do?
This extension generates code for the solution and classes based on [Best Practices framework](https://github.com/eliassilvadev/best-practices) for all Domain-Driven Design (DDD) layers and Clean Architecture layers.
Using the Clean Architecture approach can be time-consuming as it involves many files, including classes, interfaces, and SQL scripts.

This extension simplifies the process by creating all the necessary files within the solution, saving you valuable time.

Types of files that this extension can do for you:

**Solution:**
 - A solution based on a template with all layers using clean architecture, DDD, and unit tests.

You can generate necessary files to all layers for each entity(model) you want.

**From Domain Layer:**
 - Interface for the repository
 - Implementation class for the repository
 - Interface for the command provider
	
**From Application Layer:**
 - Dto record for create use case input
 - Dto record for update use case input
 - Dto record for use case output
 - Dto record for get by filter paginated use case output
 - Validation class for the dto create use case input
 - Validation class for the dto update use case input
 - Create Use Case class for the entity
 	- Preventing register duplication if you need
 - Update Use Case class for the entity		
 	- Preventing register duplication if you need
 	- Validate if the entity exists on the repository
 - Delete Use Case class for the entity
 	- Validate if the entity exists on the repository
 - GetById Use case class
 	- Validate if the entity exists on the repository
 - GetPaginated use case
 - Interface for query provider (used in GetById UseCase)
 - Interface for list item query provider (used in GetPaginated Use Case)
 - Error messages constants creation
 - Set Dependencies mappings for use cases	
 - Set Dependencies mappings for validators
	
**From Infrastructure Layer:**
 - Dapper command class for the entity
 - Dapper command provider class for the entity
 - Dapper query provider class that implements command provider
 - Dapper query provider class that implements list item query provider
 - Dapper Table definition class for the entity
 - Set Dependencies mappings for command providers
 - Set Dependencies mappings for query providers
 - Migration SQL Script to create the table that represents the entity
	
**From Unit Tests Layer:**
 - Builder class for the create use case input dto
 - Builder class for the update use case input dto
 - Builder class for the use case output dto
 - Builder class for the list use case output dto
 - Builder class for the entity
 - Unit tests class for the entity
 	- Test for each method
 - Unit tests class for create use case input dto validation  
 - Unit tests class for update use case input dto validation
 - Unit tests class for create use case
 - Unit tests class for update use case
 - Unit tests class for delete use case
 - Unit tests class for get by id use case

**From Presentation Layer:**
 - Controller for entity operations:
	- Post endpoint
	- Put endpoint
	- Delete endpoint
	- GetById endpoint
	- GetPaginated by filter endpoint
	
**Additionals:**
 - Postman collection for the created endpoints that refer to:
	- Create Use Case
	- Update Use Case		
	- Delete Use Case
	- GetById Use case
	- GetPaginated use case

## Using the extension
Create a new project and select the Best.Practices solution template
(https://github.com/eliassilvadev/BestPracticesCodeGenerator/blob/main/Resources/1%20-%20NewProject.jpg)

A solution will be created with a suggested structure
(https://github.com/eliassilvadev/BestPracticesCodeGenerator/blob/main/Resources/2%20-%20SolutionTemplate.jpg)

After creating an entity class, inherited from [BaseEntity](https://github.com/eliassilvadev/best-practices/blob/main/Best.Practices.Core/Domain/Entities/BaseEntity.cs)
Right-click on the entity file in Solution Explorer or the Entity class editor
(https://github.com/eliassilvadev/BestPracticesCodeGenerator/blob/main/Resources/4%20-%20RightClickMenu.jpg)

A tool window with a few options will appear. Select the options according to your needs and click on the 'Generate' button
(https://github.com/eliassilvadev/BestPracticesCodeGenerator/blob/main/Resources/5%20-%20ToolWindowOptio%20ns.jpg)

The files, including classes and interfaces, migration files, and Postman collection file will be created. You can see it on the output window
(https://github.com/eliassilvadev/BestPracticesCodeGenerator/blob/main/Resources/6%20-%20GeneratedFiles.jpg)
(https://github.com/eliassilvadev/BestPracticesCodeGenerator/blob/main/Resources/7%20-%20PostmanCollection.jpg)

with a few adjustments, you will be able to run the API.
(https://github.com/eliassilvadev/BestPracticesCodeGenerator/blob/main/Resources/8%20-%20RunningApi.jpg)

## Attention
The purpose of the Best Practices Framework Generator is to create initial code, eliminating all necessary boilerplate and allowing you to focus on what matters most: implementing your domain rules.
After generating the initial code, you'll need to get hands-on to finalize the implementations.
**Don't expect the generator to do everything for you!**

## License
[Apache 2.0](LICENSE) 
