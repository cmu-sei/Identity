#
#multi-stage target: dev
#
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS dev

ENV ASPNETCORE_URLS=http://*:5000 \
    ASPNETCORE_ENVIRONMENT=DEVELOPMENT

COPY . /app
WORKDIR /app/src/IdentityServer
RUN bash pullcdn.sh && \
    dotnet publish -o /app/dist
CMD [ "dotnet", "run" ]

#
#multi-stage target: prod
#
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS prod
ARG commit
ENV COMMIT=$commit
COPY --from=dev /app/dist /app
COPY --from=dev /app/LICENSE.md /app/LICENSE.md
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
CMD [ "dotnet", "IdentityServer.dll" ]
