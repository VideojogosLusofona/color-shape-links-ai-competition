# Auxiliary scripts

This folder contains the following auxiliary scripts:

* [`gendocs.sh`](gendocs.sh) - Generates ColorShapeLinks documentation using
  Doxygen.
* [`testthinker.sh`](testthinker.sh) - Tests a thinker agent for rule
  compliance.
* [`runcompetitions.sh`](runcompetitions.sh) - Runs a complete competition,
  making use of [`runsession.sh`](runsession.sh) to run individual sessions
  (e.g., competition tracks) and an additional include file which specifies
  the competition configuration, including the location of the participating
  thinkers. The [`includeme_example.sh`](includeme_example.sh) is an example
  of such an include file.

Note that these scripts assume that the ColorShapeLinks local repository is
located in the `${HOME}/workspace` folder under its default name, i.e.,
`color-shape-links-ai-competition`. However, this can be easily changed by
updating one or two variables in the scripts.
