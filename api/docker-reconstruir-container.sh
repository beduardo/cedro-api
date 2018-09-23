docker stop cedro-api-container
docker rm cedro-api-container
docker run -itd --name cedro-api-container --restart=always --network=rede_bistro \
-e "ConnectionStrings__DefaultConnection=User ID=bistro;Host=bd_desenv;Port=5432;Database=teste_cedro;Password=senhabd;Pooling=true;" \
-e "ASPNETCORE_ENVIRONMENT=Production" \
-e "ASPNETCORE_URLS=http://+:8080" \
-p 8080:8080 cedro-api-image