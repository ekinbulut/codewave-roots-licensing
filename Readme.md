# Codewave Roots Licence Server

## Overview

This project is a C\# application that implements a licence server with JWT-based authentication, rate limiting, and a
unit of work pattern using an SQLite database. The server provides endpoints for generating and validating licenses.

## Features

- **JWT Authentication:** Secures endpoints using JSON Web Tokens.
- **Rate Limiting:** Prevents abuse by limiting the number of requests per time window.
- **Unit of Work Pattern:** Manages database operations with an encapsulated repository pattern.
- **SQLite Integration:** Uses SQLite for data persistence.
- **OpenAPI & Scalar API Reference:** For API documentation and easy testing in development environments.

## Project Structure

- `codewave-roots-licence-server/Program.cs`  
  Main application entry point, configuring services, middleware, and endpoints.

- `codewave-root-licence-server-infrastructure/UnitOfWork/UnitOfWork.cs`  
  Implementation of the UnitOfWork pattern to coordinate repository work with the database context.

- `codewave_root_licence_server_infrastructure/Data/LicenseDbContext.cs`  
  Contains the SQLite database context definitions.

- `codewave_root_licence_server_infrastructure/Repositories/LicenseRepository.cs`  
  Implements data access logic for licenses.

- `codewave_root_licence_server_core/Interfaces/`  
  Contains interfaces defining contracts for repositories and unit of work.

## Setup

1. **Install Dependencies:**
    - Ensure you have the .NET SDK installed.
    - Navigate to the project folder and restore dependencies with:
      ```bash
      dotnet restore
      ```

2. **Database:**
    - The project uses SQLite. The database file (`licenses.db`) will be created automatically if it does not exist.

3. **Configuration:**
    - JWT settings are defined in the `Program.cs` file. Replace the example key ("YourSuperSecretKeyHere") with your
      own secure key.
    - Check `launchSettings.json` under `codewave-roots-licence-server/Properties` for development environment settings.

4. **Running the Application:**
    - In development mode, run the application with:
      ```bash
      dotnet run
      ```

## Endpoints

- **POST** `/generate`  
  Generates a new license. Requires authentication.

- **GET** `/validate/{key}`  
  Validates the provided license key. Requires authentication.

## Testing

- Use tools like [Postman](https://www.postman.com) or [curl](https://curl.se) to test endpoints.
- In development, use the OpenAPI endpoint provided to explore the API.

## License

This project is licensed under the MIT license.