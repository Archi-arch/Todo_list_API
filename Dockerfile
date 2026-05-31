
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app


COPY *.csproj ./
RUN dotnet restore


COPY . ./


RUN dotnet publish My_todo_api.csproj -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080


ENTRYPOINT ["dotnet", "My_todo_api.dll"]