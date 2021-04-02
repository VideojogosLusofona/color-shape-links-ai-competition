#!/usr/bin/env bash

#
# Generate documentation with Doxygen and push it to the GitHub pages branch.
#
# Requires: git doxygen
#
# Author: Nuno Fachada <nuno.fachada@ulusofona.pt>
# Licence: Mozilla Public License version 2 (MPLv2)
# Date: 2020, 2021
#

### Change these according to your setup

# Path to the ColorShapeLinks repo, master branch
MASTER="${HOME}/workspace/color-shape-links-ai-competition"
# Path to the ColorShapeLinks repo, gh-pages branch
GHPAGES="${HOME}/workspace/color-shape-links-ai-competition-ghpages"
# Path to Doxygen binary
DOXYBIN="${HOME}/bin/doxygen.1.9.1"

### Don't change these unless you really know what you're doing

echo "[CSL] Removing docs from ${MASTER}" \
&& rm -rf ${MASTER}/docs \
&& echo "[CSL] Fetching latest ghpages from upstream..." \
&& (cd ${GHPAGES} && exec git pull --rebase -f) \
&& echo "[CSL] Removing docs from ${GHPAGES}" \
&& rm -rf ${GHPAGES}/docs \
&& echo "[CSL] Running doxygen on ${MASTER}" \
&& (cd ${MASTER} && exec ${DOXYBIN}) \
&& echo "[CSL] Moving docs from ${MASTER} to ${GHPAGES}" \
&& mv ${MASTER}/docs ${GHPAGES}/docs \
&& echo "[CSL] Copy README.md to ghpages folder" \
&& cp ${MASTER}/README.md ${GHPAGES}/README.md \
&& echo "[CSL] Adding markdown files and updated docs" \
&& (cd ${GHPAGES} && exec git add *.md docs) \
&& echo "[CSL] Commiting..." \
&& (cd ${GHPAGES} && exec git commit -m "Documentation updates") \
&& echo "[CSL] Pushing..." \
&& (cd ${GHPAGES} && git push) \
#&& echo "[CSL] Commiting with amend" \
#&& (cd ${GHPAGES} && exec git commit --amend --no-edit) \
#&& echo "[CSL] Pushing..." \
#&& (cd ${GHPAGES} && git push -f) \
