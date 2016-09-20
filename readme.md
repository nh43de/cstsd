Welcome to `cstsd`!
====

This is an (almost) complete re-write of ToTypeScriptD. The idea here was to separate
out the code generation and type scanning layers and provide a solution that is 
extensible and maintainable so that it can be adapted to multiple purposes as described 
below. As part of this rewrite I have dropped support for WinMD, events, and async
generation logic. Additionally, I have dropped Mono.Cecil in favor of System.Reflection and Roslyn.
With the rewrite it is now much easier to develop your own type scanner and leave the
TS rendering logic intact now that the rendering and type scanning logic are completely 
decoupled, and implementing a Mono-based type scanner would not be difficult.

In addition, I have added a few additional options that may be useful:

1) TypeScriptExport attribute - this lets you add the [TypeScriptExport] attribute to 
items that you want marked to be dumped to TypeScript. I am also considering codegen injection
into these attributes. TypeScanner Filtering by this attribute can be overriden by adding the -a
flag to the command lines arguments to dump all assembly types to TS.

2) Output to file. Use the -o <filepath> to output the generated TS to a file.


Note: this project is still a work in progress but I welcome any feedback/contributions.


---


## Contribute

Checkout the [Contribution](CONTRIBUTING.md) guide.

