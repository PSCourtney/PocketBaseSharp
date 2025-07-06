
# PocketBaseSharp

Community-developed, open-source C# SDK for [PocketBase](https://pocketbase.io/) — the lightweight, real-time backend for your apps.

**Acknowledgments**
Special thanks to [**PRCV1** for creating the original PocketBase C# SDK](https://github.com/PRCV1/pocketbase-csharp-sdk) and laying the groundwork for this project. His excellent work made this continuation possible.

## Features


-  Authentication (user/admin)
-  Real-time subscriptions
-  Batch operations (create/update/delete) **(NEW)**
-  File uploads/downloads
-  Blazor & .NET 9 compatible **(NEW)**
-  Mudblazor demo Blazor WASM app **(NEW)**
-  Pocketbase v0.28.4 **(NEW)**


## Acknowledgments

Special thanks to PRCV1 for creating the original PocketBase C# SDK and laying the groundwork for this project. His excellent work made this continuation possible. The original project was archived and this project is to pick up support.

---

## Installation
[PocketBase docs](https://pocketbase.io/docs/)

 - Add PocketSharpSDK to your solution
 - Add PocketSharpSDK to your project as a reference

**Running the demo project:** 
 - Set `example.csproj` as startup project
 - Open `\PocketBase\pocketbase.exe` in terminal and run the following
   command to start the PocketBase instance: `pocketbase.exe serve`
 - Visit `http://127.0.0.1:8090/_/` to access the database directly

**Pocketbase admin login:** 
Email: `admin@admin.com`
PW: `demo123456`

**Example blazor demo login:** 
Email: `admin@admin.com`
PW: `demo1234`

`example/wwwroot/appsettings.json` to change PocketBase instance address.

## Getting Started
using PocketBaseSharp;

Create a new client which connects to your PocketBase API

    var client = new PocketBase("http://127.0.0.1:8090");

Authenticate as an Admin

    var admin = await client.Admin.AuthWithPasswordAsync("admin@admin.com", "demo123456");

Or as a User

    var user = await client.User.AuthWithPasswordAsync("admin@admin.com", "demo1234");

// Query some data (for example, some ToDo items)
// Note: Each CRUD action requires a data type which inherits from the base class 'BaseModel'.

    var restaurantList = await client.Collection("todos").GetFullListAsync<todos>();


## Development

### Requirements
- .NET 9 


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
> Continued development of the original PocketBase C# SDK by PRCV1 - I can't thank you enough for your work!





