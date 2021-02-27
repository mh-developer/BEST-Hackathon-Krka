# Hackathon izziv Krka

Kazalo vsebine
=================
- [Predpogoji](#predpogoji)
- [Inštalacija in poganjanje](#inštalacija-in-poganjanje)
- [Uporabljene tehnologije](#uporabljene-tehnologije)
- [Uporabljen pristop](#uporabljen-pristop)
- [Struktura aplikacije](#struktura-aplikacije)


## Predpogoji

Pravilno poganjanje aplikacije preizkušeno s sledečimi razvijalskimi orodji:
- [Visual Studio 2019](https://visualstudio.microsoft.com/vs/)
- [SSMS 19.x](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)


## Inštalacija in poganjanje

Ko imate pripravljena razvijalska okolja, zaženite sledeče ukaze:

1. Odprite projekt v Visual Studiu
    - `Open project or solution -> poiščite preneseno mapo ter v podmapi Hackathon/ odprite .sln datoteko`
    - Po potrebi izvedite `Restore NuGet Packages`

2. Nastavite povezavo do podatkovne baze v Visual Studiu
    - `Hackathon.MVC ali .API -> appsettings.json -> "ConnectionStrings" -> "DefaultConnection"`
    
3. Zaženite migracije podatkovne baze
    - `dotnet ef database update` -> če uporabljate .NET Core CLI
    - `Update-Database -Project Hackathon.Infrastructure` -> če uporabljate Packege Manager Console
    
4. Nato lahko zaženete aplikacijo


## Uporabljene tehnologije

- ASP.NET Core MVC 5.0
- ASP.NET Core API 5.0
- SendGrid - Za pošiljanje e-mailov
- Bootstrap 4 - Mobile first
- Docker
- Azure Cloud

## Uporabljen pristop

- "Code first"
- "Soft delete"

## Objavljeno na Azure Cloud

- http://hackathon-web.westeurope.azurecontainer.io/
- http://hackathon-api.westeurope.azurecontainer.io/

## Struktura aplikacije in razslojevanje aplikacije

- Hackathon.Domain -> organizacija podatkovnih modelov aplikacije
- Hackathon.Infrastructure -> organizacija migracij in komunikaje z podatkovno bazo
- Hackathon.MVC -> implementacija spletnega vmesnika
- Hackathon.API -> implementacija spletne storitve
- Hackathon.Application -> implementacija storitvene logike aplikacije
