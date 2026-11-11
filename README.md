<div style="display: flex; align-items: center; gap: 10px; flex-wrap: wrap;">
  <img src="https://i.imgur.com/ZVBiUo5.png" alt="PBSharp" width="150"/>
  <img src="https://img.shields.io/badge/pocketbase-%23b8dbe4.svg?style=for-the-badge&logo=Pocketbase&logoColor=black" alt="PocketBase"/>
  <img src="https://img.shields.io/badge/built%20with-Blazor-purple?style=for-the-badge&logo=dotnet&logoColor=white" alt="Blazor"/>
  <img src="https://img.shields.io/badge/MudBlazor-%231e88e5.svg?style=for-the-badge&logo=mudblazor&logoColor=white" alt="MudBlazor"/>
</div>


## **PocketBaseSharp**
Community-developed, open-source C# SDK for [PocketBase](https://pocketbase.io/) — the lightweight, real-time backend for your apps.



### Features


-  Authentication (user/admin)
-  Real-time subscriptions
-  Batch operations (create/update/delete)
-  File uploads/downloads
-  **Backup Management** (create, download, restore, delete backups)
-  **SQL console, logs truncate & collection meta APIs** (superuser only) **(NEW)**
-  Blazor & .NET 11 compatible
-  Mudblazor demo Blazor WASM app with admin dashboard
-  Pocketbase v0.42.0


## Acknowledgments

Special thanks to [**PRCV1** for creating the original PocketBase C# SDK](https://github.com/PRCV1/pocketbase-csharp-sdk) and laying the groundwork for this project. His excellent work made this continuation possible. 

This fork exists with his approval. 

## Installation

**Structure:**

 - Example (Blazor demo site)
 - PocketBaseSharp (The SDK) 
 - PocketBaseSharp.Tests
 - PocketBase (Database & windows binary)

**Running the demo project:** 
 - Set `example.csproj` as startup project
 - Open `\PocketBase\pocketbase.exe` in terminal and run the following
   command to start the PocketBase instance: `pocketbase.exe serve`
 - Visit `http://127.0.0.1:8090/_/` to access the database directly

**Pocketbase admin login:** 
Email: `admin@admin.com`
PW: `demo123456`

**Example blazor demo login (User):**
Email: `admin@admin.com`
PW: `demo1234`

**Example blazor admin dashboard:**
Access the admin dashboard at `/admin/login` with:
Email: `admin@admin.com`
PW: `demo123456` (PocketBase admin password)

The admin dashboard includes:
- Admin authentication
- Database backup management (create, download, restore, delete)

`example/wwwroot/appsettings.json` to change PocketBase instance address.

**Using the SDK:**

 - Add PocketSharpSDK to your solution
 - Add PocketSharpSDK to your project as a reference
 - *Nuget package coming in the future*

## Getting Started
using PocketBaseSharp;

Create a new client which connects to your PocketBase API

    var client = new PocketBase("http://127.0.0.1:8090");

Authenticate as an Admin

    var admin = await client.Admin.AuthenticateWithPasswordAsync("admin@admin.com", "demo123456");

Or as a User

    var user = await client.User.AuthenticateWithPasswordAsync("admin@admin.com", "demo1234");

Query some data (for example, some ToDo items)
Note: Each CRUD action requires a data type which inherits from the base class 'BaseModel'.

    var restaurantList = await client.Collection("todos").GetFullListAsync<todos>();

Manage backups (Admin only)

    // Create a new backup
    var result = await client.Backup.CreateAsync("my-backup.zip");

    // List all backups
    var backups = await client.Backup.GetFullListAsync();

    // Download a backup
    var stream = await client.Backup.DownloadAsync("my-backup.zip");

    // Restore from backup (⚠️ Destructive - replaces current database)
    await client.Backup.RestoreAsync("my-backup.zip");
    (not functional when pocketbase is hosted by windows. Will work with Linux though.

    // Delete a backup
    await client.Backup.DeleteAsync("my-backup.zip");

Superuser tools (Admin only, PocketBase v0.37+)

    // Run a raw SQL query (⚠️ any statement is executed - use with care)
    var sql = await client.Sql.RunAsync("SELECT id, email FROM users LIMIT 10");
    // sql.Value.Columns, sql.Value.Rows, sql.Value.AffectedRows, sql.Value.ExecTime

    // Delete all logs without changing the retention setting
    await client.Log.TruncateAsync();

    // List all configurable OAuth2 providers (with inline SVG logos)
    var providers = await client.Collections.GetAllOAuth2ProvidersAsync();

    // Preview a view collection query without creating it
    var preview = await client.Collections.DryRunViewQueryAsync("SELECT id, title FROM todos");
    // preview.Value.Fields, preview.Value.Sample

Note: since PocketBase v0.42.0 any request with an unknown or expired `Authorization` token
is rejected with `401`, so clear the `AuthStore` and re-authenticate when you receive one.


## Development

### Requirements
- .NET 11


## Contributing

Contributions are welcome! Please open issues or pull requests.

1. Fork the repo
2. Create your feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -am 'Add new feature'`)
4. Push to the branch (`git push origin feature/YourFeature`)
5. Open a pull request

---

## License

[MIT](LICENSE)

---

This project is currently still under development. It is not recommended to use it in a production environment. Things can and will change. This also applies to PocketBase



> Built with 💘 for the PocketBase community. 
> Continued development of the original PocketBase C# SDK by PRCV1 - I can't thank you enough for your work! This project is tested with BrowserStack.





