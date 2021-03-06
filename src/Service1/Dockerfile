FROM mcr.microsoft.com/dotnet/sdk:6.0.100-preview.7-alpine3.13 AS build-prep

# Prefiltering stage using find -exec and cp --parents to copy out
# all files required for the dotnet restore step.
# This is to workaround a docker issue https://github.com/moby/moby/issues/15858
WORKDIR /repo
COPY ./ ./
RUN mkdir ../proj && \
find . -type f -a \
\( \
-iname "*.sln" -o \
-iname "*.csproj" -o \
-iname "*.props" -o \
-iname "*.targets" -o \
-iname ".editorconfig" -o \
-iname "nuget.config" -o \
-iname "stylecop.json" \
\) \
-exec cp --parents "{}" ../proj/ \;

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0.100-preview.7-alpine3.13 AS build
WORKDIR /app
# Copy only the project files with correct directory structure
# then restore packages
COPY --from=build-prep ./proj ./

RUN dotnet restore -r linux-musl-x64

# Copy, build and publish app and libraries
COPY --from=build-prep ./repo ./

# We define RID to workaround an issue with -r: https://github.com/dotnet/sdk/issues/14281
RUN dotnet build -c release --no-restore -p:RID=linux-musl-x64

# Publish stage
FROM build as publish
RUN dotnet publish ".\src\Service1\Service1.csproj" -c release -o ./bin -r linux-musl-x64 --no-restore --no-self-contained

# Final stage for runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0.0-preview.7-alpine3.13
WORKDIR /app
COPY --from=publish /app/bin ./

# Run as non-privileged service user
RUN adduser -D service
USER service

# Change ASP.NET ports so that we don't need a super user.
ENV ASPNETCORE_URLS http://+:8080
EXPOSE 8080

HEALTHCHECK CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENV LOG_LEVEL=Verbose
# Use sh to be able to expand environment variable: https://github.com/moby/moby/issues/5509
CMD [ "sh", "-c", "dotnet Genetec.Service1.dll --Serilog:MinimumLevel:Default=$LOG_LEVEL" ]
