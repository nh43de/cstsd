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


Currently there are two code generation options:

1) Generate .d.ts files from POCO classes that contain TS interfaces (using Reflection)
2) Generate jQuery AJAX methods from controller classes (using Roslyn)

---

More documentation coming soon!
