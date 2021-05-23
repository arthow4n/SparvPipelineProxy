# SparvPipelineProxy

An API server for calling [`sparv-pipeline`](https://github.com/spraakbanken/sparv-pipeline/).

## To run the server

```sh
./build.sh


docker run --rm -p 5002:5000 arthow4n/sparv-pipeline-proxy:latest
# or alternatively limit the CPU cores used by this container.
# The default entrypoint of this container runs `sparv preload` with `--processes` equal to available CPU cores.
docker run --rm --cpuset-cpus=1 -p 5002:5000 arthow4n/sparv-pipeline-proxy:latest

# Beware `curl -d` actually strips å into a and ä into a so the output could be "wrong" because the input is already wrong.
# This command is just for presenting how to call the API.
curl -H 'Content-Type: application/json' -d '{"languageCode":"swe","input":"Hallå värld!"}' http://localhost:5002/annotate/sparv
```

## sparv-pipeline-runtime-env

A standalone docker image which is ready for running sparv-pipeline commands.
