# PROSE - Formative Study

Modified to perform a formative, empirical, qualitative study in the form of a cognitive walkthrough.

Use branch `formative-a` as the starting point for demos, code exploration, and initial solution attempts. Switch to `formative-b` if help is needed on the solution. Switch to `formative-c` for the final answer.

## Usage

`dotnet build` compiles ProseTutorial.

`dotnet run` (when run from within the `ProseTutorial/` subfolder) starts an interactive session with the synthesizer. This should be used for the formative study.

`ProseTutorial/synthesis/` is where the interesting files are.

This can be run from macOS once .NET core has been installed (it has been tested on macOS 10.15 with dotnet 3.0.1).

## Changes

* `master` - Links to related PROSE online documentation have been added to the tops of the grammar, semantics, witness functions, and ranking score files.
* `formative-a` - Negative indice support has been removed from `AbsPos`, corresponding to the `AbsPos` definition in part1c of the tutorial (full `RelPos` functionality has been retained).
* `formative-b` - Includes comments from part1c of the tutorial which provide guidance on the changes to makel
* `formative-c` - Includes the complete solution from the tutorial.