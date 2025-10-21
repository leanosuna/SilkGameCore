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
    - [x] Bone Animation
    - [x] Animation blending 
	- [ ] Embedded textures
- [x] Texture manager 
	- [x] Handles texture loading
	- [x] Avoids duplicates
	- [x] Easily retrieve textures by name
- [x] Vertex/fragment shaders
	- [x] Easy selection as active
	- [x] Easy uniform management
	- [x] Texture as uniform management
- [x] Render target manager
	- [x] Framebuffers creation management
	- [x] Select active target(s)
	- [x] Copy from target to target 
- [x] ImGui
	- [x] Font management, custom font loading
	- [x] Raw text rendering
	- [x] Simple buttons 
- [x] Gizmos
	- [x] Draws simple line-based geometry
	- [x] Lines, spheres, cubes, cylinders, frustums, planes supported
- [x] Collisions
	- [x] Collision volumes and intersection logic 
	- [x] Easily debuggable using gizmos 
- [ ] Audio integration 
	- [x] Basic audio file playback 
	- [ ] Better .dll handling 
- [ ] Networking integration