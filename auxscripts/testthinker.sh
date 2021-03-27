#!/usr/bin/env bash

#
# Test a thinker for rule compliance. Requires the following parameters:
#
# $1 - Folder containing thinker source code files (no end slash)
# $2 - Fully qualified name of thinker to test
# $3 - Full path to required assembly
# $4 - Options for console app, namely thinker options (e.g. --red-params lala)
#
# Usage example:
# ./testthinker.sh ../ConsoleApp/ColorShapeLinks/Common/AI/Examples \
#    ColorShapeLinks.Common.AI.Examples.RandomAIThinker \
#    ${HOME}/workspace/color-shape-links-ai-competition/ConsoleApp/ColorShapeLinks/Common/bin/Release/netstandard2.0/ColorShapeLinks.Common.dll \
#    --white-params=2 --red-params=3
#
# Requires: grep dotnet /usr/bin/time
#
# Author: Nuno Fachada <nuno.fachada@ulusofona.pt>
# Licence: Mozilla Public License version 2 (MPLv2)
# Date: 2020, 2021
#

# Need at least three parameters
if [[ $# -lt 3 ]]; then
    echo "Usage: $0 <SourceFolder> <ThinkerFQN> <AbsPathToAssembly> [thinker options and/or assemblies]"
    exit 1
fi

# Check if folder specified in $1 exists
if [[ ! -d $1 ]]; then
    echo "'$1' is not a known folder, aborting test..."
    exit 1
fi

# Check if assembly specified in $3 exists
ASSEMBL=$(realpath $3)
if [[ $? -ne 0 || ! -e ${ASSEMBL} ]]; then
    echo "Unknown assembly '$3', aborting test..."
    exit 1
fi

# Check for invalid code
declare -a codechecks=(
    "Console.Write" "System.Console" "System.IO" "System.Net")

# Number of checks to perform
numcodechecks=${#codechecks[@]}
badcode=0

# Perform tests
for (( i=0; i<${numcodechecks}; i++ )) \
do
    grep -q ${codechecks[$i]} $1/*.cs
    if [[ $? -eq 0 ]]; then
        echo -e "\e[31mCode contains '${codechecks[$i]}\e[0m'"
        badcode=1
    fi
done

if [[ ${badcode} -eq 1 ]]; then
    echo -e -n "\e[1;31mInvalid code detected, keep going? (y/n) \e[0m"
    read -n 1 badcodeaction
    echo ""
    if [ ${badcodeaction} != "y" ]; then
        exit -1
    fi
fi

# Path variables: edit these according to your setup
THEGAME="color-shape-links-ai-competition"
NETCOREV="netcoreapp3.1"
MASTER="${HOME}/workspace/${THEGAME}"
CONSOLEAPP="${MASTER}/ConsoleApp/ColorShapeLinks/TextBased/App"
CONSOLEAPPEXE="${CONSOLEAPP}/bin/Release/${NETCOREV}/ColorShapeLinks.TextBased.App"
OUTPUTFLDR="${MASTER}/temp-output"

# Maximum allowed memory usage in kilobytes
MAXMEM=4400000

# Tests to perform
declare -a tests=(
    "Base"         "-r  6 -c  7 -w  4 -o  10 -s  11 -t   200"
    "Test12x12x8"  "-r 12 -c 12 -w  6 -o  35 -s  37 -t   300"
    "Test18x22x9"  "-r 18 -c 22 -w  9 -o  99 -s  99 -t   550"
    "Test27x35x14" "-r 27 -c 35 -w 14 -o 236 -s 237 -t  1875"
    "Test33x42x20" "-r 33 -c 42 -w 20 -o 400 -s 500 -t  3000")

# Number of tests to perform x 2
numtests=${#tests[@]}

# Build console app
dotnet build ${CONSOLEAPP} -c Release -v q > ${OUTPUTFLDR}/app-build.log 2>&1
if [[ $? -ne 0 ]]; then
    echo "\e[31mUnable to build console app, aborting test...\e[0m"
    exit 1
fi

# Check if thinker exists
${CONSOLEAPPEXE} info -a ${ASSEMBL} | grep -q "$2"
if [[ $? -ne 0 ]]; then
    echo "Thinker '$2' not found, aborting test..."
    exit 1
fi

# Capture last error
set -o pipefail

# Create output folder
mkdir -p ${OUTPUTFLDR}

# Perform tests
for (( i=0; i<${numtests}; i+=2 )) \
do
    echo -e "\e[1m[ Running test '${tests[$i]}']\e[0m"

    # Run match against itself
    /usr/bin/time -f "%M" ${CONSOLEAPPEXE} \
        match -W $2 -R $2 -a ${ASSEMBL} ${tests[$i+1]} ${@:4} \
        2>${OUTPUTFLDR}/${tests[$i]}-errors.log \
        1>${OUTPUTFLDR}/${tests[$i]}.log

    # Capture exit status (must be between 0 and 2, inclusive)
    EXITSTATUS=$(echo $?)

    # Get memory usage in kb
    MEMUSAGE=$(cat ${OUTPUTFLDR}/${tests[$i]}-errors.log | tail -n 1)

    # Check memory usage
    if [[ $MEMUSAGE -lt $MAXMEM ]]; then
        MEMUSAGETEST="\e[32mOK\e[0m"
    else
        MEMUSAGETEST="\e[31mFAIL\e[0m"
    fi
    echo -e "    Memory usage... ${MEMUSAGETEST} (${MEMUSAGE}Kb < ${MAXMEM}Kb)"

    # Check exit status
    if [[ $EXITSTATUS -ge 0 && $EXITSTATUS -le 2 ]]; then
        EXITSTATUSTEST="\e[32mOK\e[0m"
    else
        EXITSTATUSTEST="\e[31mFAIL\e[0m"
    fi
    echo -e "    Exit status... ${EXITSTATUSTEST} (exit status was ${EXITSTATUS})"
done
