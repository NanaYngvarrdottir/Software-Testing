DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd ${DIR}
mkdir -p VirtualRealityDocuments/doxygen
rm -fr VirtualRealityDocuments/doxygen/*
doxygen doxygen.conf
