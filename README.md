# ProductManagementSystemSol
Overview
Product Management System is a REST API service for managing an e-shop’s products. The service is built on the .NET Core platform (using the latest or latest LTS version) and follows Clean Architecture and SOLID principles. It provides the following functionality:

List Products: Retrieve a list of all available products

Create Product: Create a new product (only the product name and image URL are required).

Get Product by ID: Retrieve a single product using its unique identifier.

Update Product Stock: Partially update a product’s stock quantity.
*Note: The stock update is processed asynchronously via a message queue (Kafka). The Kafka Producer is implemented purely as an example, since in a production scenario, the orders service would send stock update messages, and the consumer in this service would update its database.*

Version 2 adds pagination support and kafka integration and was developed in a branch named V2 Kafka integration and later merged into the master branch.

Architecture
The solution is divided into several layers/projects:

ProductManagementSystem.API: The Web API project containing the controllers

ProductManagementSystem.Application: Contains business logic, service interfaces, and implementations.

ProductManagementSystem.Contacts: Contains DTOs

ProductManagementSystem.Domain: Contains the core domain entities and repository interfaces.

ProductManagementSystem.Infrastructure: Contains implementations for data access (using EF Core), repositories, and Kafka integration (both Producer and Consumer).

ProductManagementSystem.Tests: The unit testing project for controllers, services, and repositories.


Installation and Running
  1. Clone the Repository
  2. Configure the Application - Database and Kafka in appsettings.Development
  3. Run Docker Containers for Kafka and ZooKeeper - docker-compose up -d
  4. Apply any necessary database migrations:
  5. Ensure Proper DNS Resolution for Kafka -  127.0.0.1    kafka1 in hosts
