#!/bin/bash
set -euo pipefail
cd "${0%/*}/"

RUNTIME_ENV_IMAGE_NAME="arthow4n/sparv-pipeline:latest"

cd ./sparv-pipeline-runtime-env
docker build -t "${RUNTIME_ENV_IMAGE_NAME}" .

cd ..
docker build -t "arthow4n/sparv-pipeline-proxy:latest" .
