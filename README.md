# SparvPipelineProxy

An API server for calling [`sparv-pipeline`](https://github.com/spraakbanken/sparv-pipeline/).

## To run the server

```sh
./build.sh

docker run -p 5002:5000 docker.io/arthow4n/sparv-pipeline-proxy:latest

curl -H 'Content-Type: application/json' -d '{"languageCode":"swe","input":"Hallå värld!"}' http://localhost:5002/annotate/sparv
```

## sparv-pipeline-runtime-env

A standalone docker image which is ready for running sparv-pipeline commands.
