FROM python:3.9
SHELL ["/bin/bash", "-l", "-c"]

RUN apt update && \
    apt install -y --no-install-recommends \
    # JRE is required to run Swedish models with sprav-pipeline. 
    openjdk-11-jre \
    # 32-bit libraries are required by Hunpos
    lib32z1

RUN adduser --disabled-password --gecos '' sparv-pipeline

USER sparv-pipeline
WORKDIR /home/sparv-pipeline

RUN python3 -m pip install --user pipx && \
    python3 -m pipx ensurepath

# TODO: Change this to stable release.
# https://github.com/spraakbanken/sparv-pipeline/issues/96#issuecomment-792732238
# https://github.com/spraakbanken/sparv-pipeline/issues/95#issuecomment-792727917

ENV BUILD_INFO_SPARV_SOURCE="git+https://github.com/spraakbanken/sparv-pipeline.git@32e618e34d2afcd301b5aa683be70fc2282d984b"
# License: MIT License
RUN pipx install "${BUILD_INFO_SPARV_SOURCE}"

ENV SPARV_DATADIR="/home/sparv-pipeline/.local/share/sparv"

WORKDIR /home/sparv-pipeline/.local/share/sparv/bin/wsd
RUN set -o pipefail && \
    sparv setup -d "${SPARV_DATADIR}" && \
    # License: MIT License
    wget "https://github.com/spraakbanken/sparv-wsd/raw/master/bin/saldowsd.jar" -P "/home/sparv-pipeline/.local/share/sparv/bin/wsd" && \
    wget "https://github.com/spraakbanken/sparv-wsd/raw/master/LICENSE" -P "/home/sparv-pipeline/.local/share/sparv/bin/wsd" && \
    # License: The 3-Clause BSD License included in .tar.gz file
    (curl "http://maltparser.org/dist/maltparser-1.7.2.tar.gz" | tar xz -C  "/home/sparv-pipeline/.local/share/sparv/bin") && \
    wget "http://maltparser.org/mco/swedish_parser/swemalt-1.7.2.mco" -P "/home/sparv-pipeline/.local/share/sparv/models" && \
    # License: The 3-Clause BSD License but couldn't find the file: https://code.google.com/archive/p/hunpos/
    (curl "https://storage.googleapis.com/google-code-archive-downloads/v2/code.google.com/hunpos/hunpos-1.0-linux.tgz" | tar xz --strip-components=1 -C  "/home/sparv-pipeline/.local/share/sparv/bin")

WORKDIR /home/sparv-pipeline
RUN sparv build-models --language swe --all

ENTRYPOINT [ "/bin/bash" ]
