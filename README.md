DataBaseManager

Description: A simple application that transfers data from one relational database to another, it copies the table schema and checks it against the destination schema.
             If the schema's do not match, the transaction rollsback. If the table does not exist, it creates a new table based on the source table schema and transfers the data.

Technologies: C#, SQL (MySQL/SQL Server), .NET
