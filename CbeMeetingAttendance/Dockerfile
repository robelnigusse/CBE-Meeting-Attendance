
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["CbeMeetingAttendance.csproj", "./"]
RUN dotnet restore "CbeMeetingAttendance.csproj"


COPY . .


RUN dotnet publish "CbeMeetingAttendance.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

EXPOSE 8080


COPY --from=build /app/publish .


ENTRYPOINT ["dotnet", "CbeMeetingAttendance.dll"]