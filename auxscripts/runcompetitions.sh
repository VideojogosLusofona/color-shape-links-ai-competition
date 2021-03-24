#!/usr/bin/env bash

#
# Run a complete competition, posting results to GitHub repository branch,
# possibly configured as a GitHub pages branch. Requires a single parameter
# specifying the competition parameters script, which must define the
# following variables:
#
# SUBMISS - Folder containing submissions
# SUBMISSPROJ - Project containing submissions
# SUBMISSDLL - Submissions DLL
# GHPAGES - GitHub pages folder
# COMPCONFIG - Competition file
# INDEXFILE - Name of markdown index file
# COMMITOPTS - Git commit options
# comps - Tracks to run
#
# The file includeme_example.sh is an example of a competition parameters
# script. Run a competition with this example as follows:
#
# ./runcompetitions.sh includeme_example.sh
#
# Author: Nuno Fachada <nuno.fachada@ulusofona.pt>
# Licence: Mozilla Public License version 2 (MPLv2)
# Date: 2020
#

if [[ $# -ne 1 ]]; then
    echo "Script to include was not specified"
    exit 1
fi

if [[ ! -x $1 ]]; then
    echo "Specified script does not exist or it's not executable"
    exit 1
fi

# Leave these untouched
COMPFLDR="$(dirname "$(readlink -f "$0")")"
THEGAME="color-shape-links-ai-competition"
MASTER="${HOME}/workspace/${THEGAME}"
TODAY=$(date +%F)
OUTPUTFLDR="${COMPFLDR}/output"
CSPPROJ="${MASTER}/ConsoleApp/ColorShapeLinks/TextBased/App/App.csproj"

# Include the competition parameters script
. "$1"

# Where to place standings for the different tracks
GHPGSCOMP=${GHPAGES}/comps
# get length of an array
numcomps=${#comps[@]}

echo "[COMP] Building project..." \
&& dotnet build  ${CSPPROJ} -c Release -v q \
&& echo "[COMP] Building participants..." \
&& dotnet build  ${SUBMISSPROJ} -c Release -v q \
&& echo "[COMP] Removing previous outputs, if any..." \
&& rm -rf ${OUTPUTFLDR} \
&& echo "[COMP] Running competitions..." \
&& for (( i=0; i<${numcomps}; i+=2 )) \
do
    echo "[COMP] Running ${comps[$i]}"
    ${COMPFLDR}/runsession.sh ${comps[$i+1]} ${OUTPUTFLDR}/${comps[$i]} \
        ${SUBMISSDLL} ${COMPCONFIG}
    if [ $? -ne 3 ]
    then
        echo "[COMP] Error running session, aborting..."
        exit 1
    fi
done \
&& echo "[COMP] Fetching latest ghpages from upstream..." \
&& (cd ${GHPAGES} && exec git pull --rebase -f) \
&& echo "[COMP] Reseting ${INDEXFILE}..." \
&& echo "# Standings" > ${GHPAGES}/${INDEXFILE} \
&& echo "" >> ${GHPAGES}/${INDEXFILE} \
&& echo "Last update: $(LC_ALL=en-US.UTF-8 TZ=GMT date)" >> ${GHPAGES}/${INDEXFILE} \
&& echo "" >> ${GHPAGES}/${INDEXFILE} \
&& for (( i=0; i<${numcomps}; i+=2 ))
do
    COMPSTANDINGS="${OUTPUTFLDR}/${comps[$i]}/standings.md" \
    && echo "[COMP] Removing today's standings from gh-pages for ${comps[$i]}, if any..." \
    && rm -rf ${GHPGSCOMP}/${comps[$i]}/${TODAY} \
    && echo "[COMP] Getting previous days standings for ${comps[$i]}..." \
    && mkdir -p ${GHPGSCOMP}/${comps[$i]} \
    && ALLDAILYS=$(ls -r "${GHPGSCOMP}/${comps[$i]}") \
    && echo "[COMP] Adding previous days to ${comps[$i]} standings.md..." \
    && echo "" >> ${COMPSTANDINGS} \
    && echo "## Previous days" >>  ${COMPSTANDINGS} \
    && echo "" >>  ${COMPSTANDINGS} \
    && for CURRDAILY in ${ALLDAILYS}
    do
       echo "* [${CURRDAILY}](../${CURRDAILY}/standings.md)" >> ${COMPSTANDINGS}
    done \
    && echo "[COMP] Moving new today's standings + results to gh-pages for ${comps[$i]}..." \
    && mv ${OUTPUTFLDR}/${comps[$i]} ${GHPGSCOMP}/${comps[$i]}/${TODAY} \
    && echo "[COMP] Add link to ${comps[$i]} in root ${INDEXFILE}" \
    && echo "* [${comps[$i]} track](comps/${comps[$i]}/${TODAY}/standings.md)" >> ${GHPAGES}/${INDEXFILE}
done \
&& echo "[COMP] Adding updated standings to repo" \
&& (cd ${GHPAGES} && exec git add ${INDEXFILE} comps) \
&& echo "[COMP] Commiting with amend" \
&& (cd ${GHPAGES} && eval exec git commit ${COMMITOPTS}) \
&& echo "[COMP] Pushing..." \
&& (cd ${GHPAGES} && git push -f) \
&& echo "[COMP] Done!"
