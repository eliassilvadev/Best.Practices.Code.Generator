# Best Pratices Generator

A Visual Studio extension that generates the code from clean architecture aproach based on an entity class and its properties.

Download the extension at the
[VS Gallery](https://visualstudiogallery.msdn.microsoft.com/XXXXXXXXXXXXX)

## What does it do?
This extension generates code for solution and classes based on [Best Practices framework](https://github.com/eliassilvadev/best-practices) for all Domain-Driven Design (DDD) layers and Clean Architecture layers.
Using the Clean Architecture approach can be time-consuming as it involves a substantial number of files, including classes, interfaces, and SQL scripts.

This extension simplifies the process by creating all the necessary files within the solution, saving you valuable time.

Types of files that this extension can do for you:

**Solution:**
	- A solution based on a template with all layers using clean architecture, DDD, and unit tests.

For each entity(model) you want you can generate necessary files to all layers.

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
		- Validate if the entity exists on repository
	- Delete Use Case class for the entity
		- Validate if the entity exists on repository
	- GetById Use case class
		- Validate if the entity exists on repository
	- GetPaginated use case
	- Interface for query provider (used in GetById UseCase)
	- Interface for list item query provider (used in GetPaginated Use Case)
	- Error messages constants creation
	- Set Dependencies mappings for use cases	
	- Set Dependencies mappings for validators
	
**From Infrastructure Layer:**
	- Dapper command class for the entity
	- Dapper command provider class for the entity
	- Dapper query provider class that implements commandProvider
	- Dapper query provider class that implements list item query provider
	- Dapper Table definiton class for the entity
	- Set Dependencies mappings for command providers
	- Set Dependencies mappings for query providers
    - Migration Sql Script to create the table that represents the entity
	
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
	- Post entpoint
	- Put entpoint
	- Delete entpoint
	- GetById entpoint
	- GetPaginated by filter endpoint
	
**Additionals:**
 - Postman collection for the created endpoints that refer to:
	- Create Use Case
	- Update Use Case		
	- Delete Use Case
	- GetById Use case
	- GetPaginated use case

## Using the extension
Create a new project and select the Best.Practices soluton template

A solution will be create with a suggested structure

After create an entity class, inherited from [BaseEntity](https://github.com/eliassilvadev/best-practices/blob/main/Best.Practices.Core/Domain/Entities/BaseEntity.cs)
Right-click on the entity file in solution explorer or in the entity class editor

A tool window with a few options will appear. Select the options according yor needs and click on 'Generate' button

The files, including classes and interfaces, migration files and Postman collection file wil be created,. You can see it on output window

with a few adjustments you will be able to run th api.


## Attention
The purpose of the Best Practices Framework Generator is to create initial code, eliminating all necessary boilerplate and allowing you to focus on what matters most: implementing your domain rules.
After generating the initial code, you'll need to get hands-on to finalize the implementations.
**Don't expect the generator to do everything for you!**

## License
[Apache 2.0](LICENSE) 