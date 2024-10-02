# Project Hotline (WT)
> by _nullptr* Studios_

Team members:
- Dario: Technical Director
- Iñaki: Producer
- Xein: Lead designer

# Project conventions

## File structure conventions

Project files are structured by object rather than by type. Example of a project structure:
```
|-- Project Name
    |-- Assets
        |-- Art
        |   |-- Industrial
        |   |   |-- Ambient
        |   |   |-- Machinery
        |   |   |-- Pipes
        |   |-- Nature
        |   |   |-- Ambient
        |   |   |-- Foliage
        |   |   |-- Rocks
        |   |   |-- Trees
        |   |-- Office
        |-- Characters
        |   |-- Bob
        |   |-- Common
        |   |   |-- Animations
        |   |   |-- Audio
        |   |-- Jack
        |   |-- Steve
        |   |-- Zoe
        |-- Core
        |   |-- Characters
        |   |-- Engine
        |   |-- GameModes
        |   |-- Interactables
        |   |-- Pickups
        |   |-- Weapons
        |-- Effects
        |   |-- Electrical
        |   |-- Fire
        |   |-- Weather
        |-- Maps
        |   |-- Campaign1
        |   |-- Campaign2
        |-- MaterialLibrary
        |   |-- Debug
        |   |-- Metal
        |   |-- Paint
        |   |-- Utility
        |   |-- Weathering
        |-- Placeables
        |   |-- Pickups
        |-- Weapons
            |-- Common
            |-- Pistols
            |   |-- DesertEagle
            |   |-- RocketPistol
            |-- Rifles
```

## Coding conventions

All variables **MUST** be named using `camelCase`, putting a `_` prefix on private variables

Constant are named using `SCREAMING_CASE`

Functions and classes are named using `PascalCase` and **MUST** have proper documentation with parameters and returns (if not void)

When using `Debug.Log("message");`, a variable must be declared so it can be turn on or off. 
Variable **MUST** be declared like this: 
```csharp
#if UNITY_EDITOR
[Header("Debug")]
[SerializeField] private bool log = false;
#endif

// Example of Log
#if UNITY_EDITOR
if (log) Debug.Log("message");
#endif
```
Remember to put debug staff on compiler IFs so they aren't included on build

## Git repository

Do **NOT** commit on main without an aproved PR

Create feature branches for the group of issues you are working on and link them to the branch

When creating an issue provide a description and select a tag and a milestone for it
