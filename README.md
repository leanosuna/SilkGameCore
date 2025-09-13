# SilkGameCore

A framework for creating 3D games, built using Silk.NET and OpenGL

## Features so far

- [x] Game window management
- [x] Input handling
	- [x] default keyboard and mouse
	- [ ] joystick
- [x] Camera system
- [x] 3D models
	- [x] Assimp loader abstraction
	- [x] Model, Parts, Meshes hierarchy  
	- [ ] Embedded textures
- [x] texture manager 
	- [x] Handles texture loading
	- [x] Avoids duplicates
	- [x] Easily retrieve textures by name
- [x] vertex/fragment shaders
	- [x] easy selection as active
	- [x] easy uniform management
	- [x] texture as uniform management
- [x] render target manager
	- [x] framebuffers creation management
	- [x] select active target(s)
	- [x] copy from target to target 
- [x] imgui
	- [x] font management, custom font loading
	- [x] raw text rendering
	- [x] simple buttons 
	- [x] imgui interna
- [x] gizmos
	- [x] draws simple line-based geometry
	- [x] lines, spheres, cubes, cylinders, frustums, planes supported
- [x] collisions
	- [x] collision volumes and intersection logic 
	- [x] easily debuggable using gizmos 
- [ ] audio integration 
	- [x] basic audio file playback 
	- [ ] better .dll handling 
- [ ] networking integration