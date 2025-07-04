PocketBase C# SDK - NEXT edition. 
======================================================================

Community-developed C# SDK (Multiplatform) for interacting with the [PocketBase API](https://pocketbase.io/docs)

Acknowledgments
Special thanks to PRCV1 for creating the original PocketBase C# SDK and laying the groundwork for this project. His excellent work made this continuation possible.
The original project was archived and this project is to pick up support.

Latest Update:
- Forked from original project
- Upgraded to .NET 9 - Latest framework support
- Dependancies updated
- Batch Operations - batch create/update/delete operations


- [PocketBase C# SDK](#pocketbase-c-sdk)
- [Installation](#installation)
  - [Nuget](#nuget)
- [Usage](#usage)
- [Development](#development)
  - [Requirements](#requirements)
  - [Steps](#steps)

# Installation

# Usage
```c#
//create a new Client which connects to your PocketBase-API
var client = new PocketBase("http://127.0.0.1:8090");

//authenticate as a Admin
var admin = await client.Admin.AuthenticateWithPassword("test@test.de", "0123456789");

//or as a User
var user = await client.User.AuthenticateWithPassword("kekw@kekw.com", "0123456789");

//query some data (for example, some restaurants)
//note that each CRUD action requires a data type which inherits from the base class 'ItemBaseModel'.
var restaurantList = await client.Records.ListAsync<Restaurant>("restaurants");

//like this one
class Restaurant : ItemBaseModel
{
    public string? Name { get; set; }
}
```

# Development

## Requirements
- .NET 9 SDK

## Steps
1. Clone this repository
```cmd
git clone https://github.com/PSCourtney/pocketbase-csharp-sdk-next
```
2. Open the [pocketbase-csharp-sdk.sln](pocketbase-csharp-sdk.sln) with Visual Studio (Community Edition should work just fine)
