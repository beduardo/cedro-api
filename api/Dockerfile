FROM microsoft/dotnet:2.1-aspnetcore-runtime

ENV TZ=America/Sao_Paulo
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

WORKDIR /app

COPY bin/Release/netcoreapp2.1/publish .

ENV ASPNETCORE_URLS http://+:8080

ENTRYPOINT ["dotnet", "api.dll"]

