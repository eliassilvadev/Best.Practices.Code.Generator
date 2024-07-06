# Best Pratices Generator

A Visual Studio extension that generates the code from clean architecture aproach based on a entity class and its properties.

Download the extension at the

or get the


## What does it do?
This extension generates code for classes based on the Best Practices framework for all Domain-Driven Design (DDD) layers and Clean Architecture layers.
Using the Clean Architecture approach can be time-consuming as it involves a substantial number of files, including classes, interfaces, and SQL scripts.
This extension simplifies the process by creating all the necessary files within the solution, saving you valuable time.

Types of files that this extension can create for you:

From Domain Layer:
	- Interface for the repository
	- Implementation class for the repository
	- Interface for the CommandProvider
From Application Layer:
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
From Infrastructore Layer:
	- Dapper command class for the entity
	- Dapper command provider class for the entity
	- Dapper query provider class for the entity
	- Dapper Table definiton class for the entity
    - Sql Script to create the table that represents the entity
From Unit Tests Layer:
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
From Presentation Layer:
 - Controller for entity operations:
	- Post entpoint
	- Put entpoint
	- Delete entpoint
	- GetById entpoint
	- GetPaginated by filter entpoint
	
Additionals:
 - Postman collection for the created endpoints that refer to:
	- Create Use Case
	- Update Use Case		
	- Delete Use Case
	- GetById Use case
	- GetPaginated use case

## Using the extension
In Visual Studio's top menu under Tools, a new command is now visible:

## License
[Apache 2.0](LICENSE) 