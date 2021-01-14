#!/usr/bin/env bash

#
# Run a ColorShapeLinks session with the parameters below and save detailed
# results in Markdown in the output folder:
#
# $1 - Rows
# $2 - Cols
# $3 - Win sequence
# $4 - Init round pieces
# $5 - Init square pieces
# $6 - Time limit
# $7 - Limit to one core? (1 - yes, 0 - no)
# $8 - Output folder
# $9 - Submissions DLL
# ${10} - Competition configuration file
#
# Although this script can be used directly, its main purpose is to be used
# by the runcompetitions.sh script.
#
# Author: Nuno Fachada <nuno.fachada@ulusofona.pt>
# Licence: Mozilla Public License version 2 (MPLv2)
# Date: 2020
#

# Check number of parameters
if [[ $# -ne 10 ]]; then
    echo "Illegal number of parameters"
    exit 1
fi

# Limit to one CPU or not?
if [[ $7 -eq 1 ]]
then
    LIMITER="taskset --cpu-list 0"
else
    LIMITER=""
fi

# Edit these according to your setup
NETCOREV="netcoreapp3.1"
THEGAME="color-shape-links-ai-competition"
MASTER="${HOME}/workspace/${THEGAME}"
CSPPROJ="${MASTER}/ConsoleApp/ColorShapeLinks/TextBased/App/App.csproj"
CSPAPP="${MASTER}/ConsoleApp/ColorShapeLinks/TextBased/App/bin/Release/${NETCOREV}/ColorShapeLinks.TextBased.App"

# Clearer variable names
OUTPUTFOLDER=$8
SUBMISSDLL=$9
COMPCONFIG=${10}

# Run session
echo "[SESSION] Running session and saving to ${OUTPUTFOLDER}..." \
&& mkdir -p ${OUTPUTFOLDER} \
&& (cd ${OUTPUTFOLDER} && \
    ${LIMITER} ${CSPAPP} session -r $1 -c $2 -w $3 -o $4 -s $5 -t $6 \
    -g ${COMPCONFIG} \
    --session-listeners ColorShapeLinks.TextBased.Lib.CompSessionListener \
    --thinker-listeners ColorShapeLinks.TextBased.App.SimpleRenderingListener \
    --match-listeners ColorShapeLinks.TextBased.App.SimpleRenderingListener \
    --assemblies ${SUBMISSDLL})
exit $?
