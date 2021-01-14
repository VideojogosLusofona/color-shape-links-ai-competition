#
# This is an example of a competition parameters script to be included by the
# runcompetitions.sh script. It assumes the following:

# - A C# project named example-competition containing all the participating
#   thinkers exists in the ${HOME}/workspace/example-competition folder.
# - A session configuration file competition.txt exists in that same folder.
# - A GitHub pages repository exists in the
#   ${HOME}/workspace/example-competition-ghpages folder.
#
# Author: Nuno Fachada <nuno.fachada@ulusofona.pt>
# Licence: Mozilla Public License version 2 (MPLv2)
# Date: 2020
#

# Folder containing submissions
SUBMISS="${HOME}/workspace/example-competition"
# Project containing submissions
SUBMISSPROJ="${SUBMISS}/example-competition.csproj"
# Submissions DLL
SUBMISSDLL="${SUBMISS}/bin/Release/netstandard2.0/example-competition.dll"
# GitHub pages folder
GHPAGES="${SUBMISS}-ghpages"
# Competition configuration file
COMPCONFIG="${SUBMISS}/competition.txt"
# Name of markdown index file
INDEXFILE="standings.md"
# Git commit options
COMMITOPTS="-m \"Results for $(LC_ALL=en-US.UTF-8 TZ=GMT date)\""
# Competitions to run
declare -a comps=(
    "Base"         " 6  7  4  10  11  200 1"
    "Test12x12x6"  "12 12  6  35  37  300 0"
    "Test18x22x9"  "18 22  9  99  99  550 0")
