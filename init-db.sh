#!binbash
echo Czekam na uruchomienie SQL Server...
sleep 20

echo Tworzę bazę danych i strukturę...
dotnet ef database update --startup-project .FinansoApp --project .FinansoData --context ApplicationDbContext --verbose

echo Dodaję dane początkowe...
dotnet run --project .FinansoAppFinansoApp.csproj seeddata

echo Baza danych gotowa.
