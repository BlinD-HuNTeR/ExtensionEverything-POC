# ExtensionEverything - POC
This package demonstrates a way to implement extension members in C# (methods, properties, and even operators), with no modifications to the language.

The concept is to use mono.cecil to modify the assemblies referenced by your projects, *at compile-only* (no modifications are made at runtime).
For this I am using the Blur package, which allows an assembly to instrument itself at build time (but this package is super buggy, and it's not an absolute requirement to make this thing work, just used it cause it made things easier).
