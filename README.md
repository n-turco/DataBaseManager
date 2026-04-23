# Database Manager

## Project Overview

Database Manager is a robust application designed to transfer data from one relational database to another. It intelligently copies table schemas, validates them against the destination database, and ensures data integrity through transactional rollback on schema mismatches.

## Key Features

- Database-to-database data transfer
- Automatic schema validation and comparison
- Table creation if destination schema doesn't exist
- Transaction rollback on schema mismatch
- Support for multiple database providers (MySQL, SQL Server)

## Technical Stack

- C#
- .NET Framework
- SQL (MySQL/SQL Server)
- Entity Framework Core

## How to Run

1. Ensure both source and destination databases are accessible and connection strings are configured
2. Build the project:

   ```
   dotnet restore
   dotnet build
   ```

3. Configure your database connection strings in the application settings
4. Run the application:

   ```
   dotnet run
   ```

5. Follow the prompts to select source and destination databases and execute the transfer

## Developer

Nicholas Turco

---
