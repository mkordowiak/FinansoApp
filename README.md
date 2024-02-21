```
dotnet ef migrations add init --startup-project ./FinansoApp --project ./FinansoData --context ApplicationDbContext --verbose
```


```
dotnet ef database update --startup-project ./FinansoApp --project ./FinansoData --context ApplicationDbContext --verbose
```

```
dotnet run --project ./FinansoApp/FinansoApp.csproj seeddata
```