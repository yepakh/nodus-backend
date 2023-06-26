# Database Migrations

Currently application has two context:

- ClientContext
- AdminContext

## Entity Framework CLI migrations

### Configuration

CLI migrations use `appsettings.json` for getting connection strings. Configure it before executing any command.

`appsettings.json` file structure

```json
{
  "ConnectionStrings": {
    "ClientDatabase": "Data Source=localhost;Initial Catalog=SRAdminDatabase;User ID=sa;Password=Qwerty1!"
  }
}
```

### Usage

You can add new migrations with a following command:

```bash
dotnet ef migrations add AddTable --context Nodus.Database.Context.AdminContext --output-dir Migrations\Admin
```

To remove latest migration file generated, use command:

```bash
dotnet ef migrations remove --context Nodus.Database.Context.AdminContext
```

> [See more](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli) about EF comands.
