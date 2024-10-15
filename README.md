# WYD_back

creare migrazioni sql:
dotnet ef migrations add {Commento}

applicare le migrazioni sul db:
dotnet ef datebase update --connection {connectionString}

per eseguire: 

install func: https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local
posizionarsi in /src
func host start

