# Nodus Backend Repository

This repository contains the backend codebase for a web application designed to store bills from work trips. It includes various projects and microservices that handle different aspects of the application.

## Projects Overview

### Nodus.API

- Description: Provides the HTTP interface for the front-end to communicate with the microservices.

### Nodus.Jamal.Service

- Description: The main gRPC microservice containing all the business logic.

### Nodus.Auth.Handler

- Description: A library used to integrate the authentication microservice into the middleware of other services.

### Nodus.NotificationService

- Description: A gRPC microservice used for sending emails.

### Nodus.Auth

- Description: A gRPC microservice responsible for user authorization and authentication.

### Nodus.Database.Models

- Description: Contains EF (Entity Framework) database models.

### Nodus.Database.Context

- Description: Contains the DbContexts for the Admin and Client databases.

### Nodus.Database.Migrator

- Description: A library with a migration service that provides all the required methods for migrating databases.

### Nodus.Database.Migrations.gRPC/WebAPI/Console

- Description: Three client projects that utilize the migration service to migrate databases.

### Nodus.API.ViewModels

- Description: Contains view models used for communication with the front-end.

### Nodus.GlobalSettings

- Description: A library that includes the GlobalSettings.json file and extension methods for IConfigurationBuilder to set up files and environment variables.

### Nodus.TgBot

- Description: A console application used for a Telegram bot.

### Nodus.gRPC.ExceptionHandler

- Description: Common exception handler settings used in gRPC microservices.

## Usage

### Docker Compose Configuration

To run the backend services using Docker Compose, you can use the provided `docker-compose.yml` file along with the `docker-compose.override.yml` file. The `docker-compose.override.yml` file is used to override or extend the configuration defined in the base `docker-compose.yml`.

The `docker-compose.override.yml` file includes additional service configurations and volume mappings for the backend services. It sets up the necessary environment variables, ports, and volume mappings for each service. Make sure to update the `docker.env` file with the required environment variables before running the services.

Here is an example of the `docker-compose.override.yml` file:

### Running the Services
To build and run the backend services using Docker Compose, execute the following commands in your terminal:

```bash
docker compose -f ./docker-compose.yml -f ./docker-compose.override.yml build
docker compose -f ./docker-compose.yml -f ./docker-compose.override.yml up
```

The first command 
```bash
docker compose -f ./docker-compose.yml -f ./docker-compose.override.yml build
```
 builds the Docker images for the services based on the provided configuration files.

The second command 
```bash
docker compose -f ./docker-compose.yml -f ./docker-compose.override.yml up
```
starts the containers and runs the backend services.

Ensure that you have Docker installed and properly configured before running these commands.

Please note that you may need to adjust the file paths in the commands based on your specific project structure.

Remember to update the docker.env file with the required environment variables for your services.
