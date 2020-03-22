# Mrt Station
> Find your route in Singapore MRT station

## Installation

OS X & Linux:

Install Dotnet core 2.1
https://dotnet.microsoft.com/download/dotnet-core/2.1

If you encounter `dotnet command not found!` error. Link your path with **dotnet**
`ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/`

## Development setup
Go to root folder

**Build and Run**
```sh
dotnet build
dotnet run --project MRT
```

**Run Unit Test**
```sh
dotnet test
```

**Or**

Install Visual Studio Community for Mac and run the solution

## Usage example
Use postman or `curl` command.

**POST** http://localhost:3000/api/searchStation
**Request Body**
| Key  | Description |
| ------------- | ------------- |
| SourceStationCode  | Source Station Code E.G CC16  |
| DestStationCode  | Destination Station E.G NS1  |
| AtDate  | Commencement Date of the station. In YYYY-MM-DD format.  |

**Sample**
```sh
curl --location --request POST 'localhost:3000/api/searchStation' \
--header 'Content-Type: application/json' \
--data-raw '{
    "SourceStationCode" : "CC16",
    "DestStationCode": "NS1",
    "AtDate": "2020-01-01"
}'
```