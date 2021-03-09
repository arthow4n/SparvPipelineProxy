FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM arthow4n/sparv-pipeline:latest

COPY --from=build-env --chown=sparv-pipeline:sparv-pipeline /app/out/SparvPipelineProxy /home/sparv-pipeline/.local/bin/

WORKDIR /tmp/sparv-pipeline
COPY ./config.yaml ./

ENTRYPOINT [ "/bin/bash", "-l", "-c", "sparv preload --socket /tmp/sparv_preload.sock --processes `nproc --all` & until ss -l | grep -q /tmp/sparv_preload.sock; do echo Waiting for sparv preload...; sleep 3; done; ASPNETCORE_URLS=http://0.0.0.0:5000/ /home/sparv-pipeline/.local/bin/SparvPipelineProxy" ]
HEALTHCHECK --interval=60s --timeout=10s --start-period=60s --retries=2 CMD [ "/bin/bash", "-c", "curl -sfo /dev/null http://127.0.0.1:5000/healthcheck" ]
