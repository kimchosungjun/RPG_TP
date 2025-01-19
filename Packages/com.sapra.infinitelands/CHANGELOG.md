# [0.7.5] - 2024-19-12
## Added
- Website: Roadmap to the package.  https://ensapra.com/packages/infinite_lands/roadmap
	*It's been a while since I wanted to add a roadmap to the website. I just wasn't sure what was the best way to do it. This is an experimental roadmap. It contains ideas and tasks that I want to add at one point or another, but being there doesn't mean they will. It's just that they are on my radar
- Backend: Unity Terrain experimental support
	*Another feature that it's been a while that I wanted to add. This is still not feature complete since it's lacking Trees and Grass from Unity Terrain. Vegetation can be included by using the default system. We will see how this goes and what needs to be tweaked*
- Backend: Warning when pressing Regenerate on a graph without any chunk visualizer attached
- Backend: Warnings when launching a world without a terrain setup
- Backend: Warnings when launching a world that has floating origin but the chunk doesn't have floating point
## Changed
- Website: NEW DOMAIN! Yay, no longer to ensapra.github.io but now we can go to **ensapra.com!**
- Rendering: Vegetation max render distance can now be modified in runtime
- Backend: Removed deprecated methods
- Backend: Workflow of custom chunks has been modified to be more intuitive and simpler.
	*This means **redownloading samples**. Now the chunks will be a child of the terrain generator. Anything that goes with the chunk will be spawned next to it. You can also set a prefab if you prefer in the Infinite Chunk Visualizer. That should make it easier for custom systems*
## Fixed
- Vegetation: Made Vegetation displacer be disabled by default in URP and HDRP
- Samples: Disabling features that made it look wrong
- Rendering: Minimal shader looks properly
- Backend: Allows for any texture, any resolution and even missing data
- Backend: Drag and drop now works properly
- Backend: Colliders sometimes weren't properly generated because of nan values
- Backend: Scenes no longer save with meshes and materials, reducing considerably all sizes
- Backend: General fixes
## Removed
- Samples: Removed settings from the samples

# [0.7.4] - 2024-12-10
## Added
- New Feature!: Floating Point. Allows for infinite movement while generating the terrain without losing precision. It includes three components:
	- Floating Origin: Should be added on the main generator
	- Floating Point: Should be added to objects that should stay in a specific position
	- Floating Particle: Should be added to objects containing a particle system
- New Feature!: Environmental Components. Allow for objects manually placed in the terrain to take into account the world generation
	- AlignWithGround: When a new mesh is generated in it's position, it will align with the groun
	- StayOnTerrain: Ensures that the object doesn't go underthe ground when a new mesh is generated. 
- Nodes: New blending values for textures and vegetation
- Nodes: Relativity on feature extraction. You can choose in relation to what should features exist. Relative to the terrain or to vector up. (Useful for cases where the generation has some rotation)
- Testing: Test scripts for various components

## Changed
- Samples: Due to changes in the PointStore, some scripts required updating
- Samples: They make use of floating point origin and the other new environmental components
- Backend: Extra null checks on events
- Backend: Moved data into it's own interface
- Vegetation: Huge improvement on rendering of vegetation in small meshes. You can now use way smaller meshes without stutters

## Fixed
- Rendering: Displacemnt shader now works properly when disabled
- Vegetation: Objects no longer pop in and out of existence as frequently as before
- Backend: Some chunks were being rendered when they shouldn't have

## Removed
- Backend: No more references to player in PointStore

# [0.7.3] - 2024-10-23
## Added
- NEW NODE: Directional Warp. Warp a node according to their normal map, creating smaller detail
- Components: Vegetation Renderer has a new parameter called Maximum Render Distance that allows to limit the rendering of all vegetation
- Rendering: New Minimal Shader. Contains the absolut basic configuration for any shader to work with the GPU System that Infinite Lands provides.
- Vegetation System: Assets now reload the vegetation for live editing

## Changes
- Backend: IHoldVegetation and Vegetation Asset slightly decoupled

## Fixed
- Backend: Nodes that make use of an Animation Curve now reload after changes have been done to it. 
- Backend: Normal maps in shaders now are correctly set
- Backend: All LODs now have the chance to generate colliders
- Backend: Now chunks shouldn't end up with a mesh collider enabled but without an actual mesh set for it
- Vegetation System: No longer breaks without a mesh or without a material
- Rendering: Correct sampling of normals

# [0.7.2] - 2024-10-7
## Added
- As per request of unit577 and stor314. Backend: Support for any rotated type of terrain
- Backend: Support for simple grid like generation
- Backend: Moved Mesh Creation into a separate interface and MonoBehaviour
## Fixed
- Backend: Fixed a problem in code where building the project would break
- Backend: Added missing events in Texture Painter for when textures are generated or removed
- Backend: Export popup not correctly having the generator and therefore not working
- Backend: Creating a simple world no longers throws an errors and creates two output height Generators
- Backend: Texturing doesn't break when there are no textures available
- Backend: Removed deprecated node from Simple World asset generator
- Backend: Resolved the issue of constantly checking for new data in vegetation system
- Backend: Resolved an issue where the vegetation would fall with a wrong offset
- Backend: Reduced vegetation complexity of the diferent scripts
- Backend: Biome generation now works
- And more bugs that I found along the way
## Changed
- Backend: Removed unnecessary fields in CPU Instancing
- Backend: Renamed Size to Minimum Scale and Maximum Scale in CPU Instancing and GPU Instancing assets.
		It wasn't super clear that Size was actually a range of values, so this new name should better reflect that

# [0.7.1] - 2024-09-30
## Changed
- Rendering: Changed impostor ambient occlusion so light reacts accordingly

## Fixed
- Samples: Created a Shared Content folder for all the assets of the samples
- Samples: Samples have been fixed so that they correctly load up when importing them


# [0.7.0] - 2024-09-29
## Added
- Editor: Multi-window support
- Editor: Save and restoring of graph positions
- Editor: Copy pasting of group
- Editor: Creation of sticky nodes, with copy-pasting functionality
- Editor: Custom icon in the windows
- Editor: New options in the context menu
	- Copy Properties: Allows copying properties of a node to be pasted into another
	- View option to graph view so that it focuses on the right part
		- Expand/Collapse Nodes.
		- Expand/Collapse Preview.
		- Fit: Allows the graph to focus on all the nodes.
- Editor: New Export Window.
	- Export Mode: Allows selection of the exporting method. Check out the different exporters and the documentation to create your exporter. 
	- Quality: Allows generation exporting to a higher quality resolution than it was previously. 
- Editor: New Drag and Drop functionality for assets. You can drag an asset into the Editor Window to automatically create the required node. 
- Nodes: Added Minimum, Maximum, To Zero to Multiply Node (now named Apply Mask).
		This should make it more clear to differentiate between the Normalized Multiply option from Combine and the functionality that this node does. 
- Nodes: A fixed seed option was added for the Voronoi Generator.
		This option should allow users to use the variants of the Voronoi Generator with the correct context. 
- Components: A new component called Texture Painter. This new component manages the texturing of the terrain and should be used when requiring color data of the generated chunk.
- Vegetation: New Vegetation Type, CPU Instancing.
		This new type of vegetation will use the native Unity Instantiate method to create new game objects and pool them accordingly. This method is slower than GPU Instancing but might be interesting for anyone looking at persistence.
- Samples: Added new Samples
	-  World Generation (Built-in)
	-  World Generation (URP)
	-  World Generation (HDRP)
	- Grass Displacement (Built-in)
- Backend: A Mesh Collider will be automatically added when Entering Play Mode in Single Chunk Visualizer
- Backend: New Drag and Drop from Inspector View to Scene View to generate the necessary components
- Experimental: New option in Mesh Settings to export masks in a custom texture Resolution.
		This new feature allows you to set any resolution during the creation of Texture Masks and Vegetation Masks. However, when enabled, generation times could be doubled or more.
	
## Deprecations
- Nodes: Noise Generator node has been deprecated. You will see it in red. Please change it to the corresponding node before the next major release.
- Renderer: **All previous shaders have been removed!** Please make sure to select the new shaders accordingly.

## Changes
- Editor: Resolution and mesh scale in the editor will now be clamped after imputing a value
- Editor: Fully reworked the node creation menu. Now it simulates the Shader Graph search menu. 
- Nodes: Separated Noise Nodes into different nodes.
		The reason for this change is to accommodate for future nodes. I'm planning on adding more variations of noise and I would like a more robust and clear way to have each type defined. 
- Nodes: Renamed Normal Mask to Select Mask
- Nodes: Renamed Multiply Node to Apply Mask
- Nodes: Variables in Nodes and Generator are now Read Only (can be disabled by going into Debug Mode of Unity)
- Nodes: Added preview options to Texture Nodes and Vegetation Nodes
- Rendering: Improved and reduced memory consumption.
- Rendering: Moved all shaders into Shader Graph, making them compatible with URP and HDRP
- Rendering: Fully reworked the rendering system to be more performant and compatible with URP and HDRP
- Backend: Vegetation will now be rendered in all the cameras. 
- Backend: Instead of selecting the cameras via a List, now you can set a Culling Mask that will be used to select the cameras that render that channel.
- Backend: Caching normal map generation for faster generation times
- Backend: General renaming of classes, variables, and namespaces
- Backend: Added more interfaces to allow users for self-implementations of assets and nodes (Check Documentation)
- Backend: Reworked Point Store to be more efficient and consistent. 
- Backend: Deleting a generator will now also close the window where it was opened

## Fixes
- Editor: When deleting a group it no longer deletes all the nodes
- Editor: Groups will always stay grouped
- Editor: Height Output Node should always be generated now
- Rendering: Shadows of GPU Instanced objects should no longer disappear when the object is out of frame. 
- Rendering: LOD Transitions should always work
- Backend: General precision issues have been fixed. Now Generation between Burst compiled enabled or disabled shouldn't be too different.
- Backend: Undo/Redo no longer breaks the graph editor.
- Backend: Auto-Update no longer breaks other generations. 
- Backend: Copy-pasting is now more responsive, and faster, takes more objects into account, and can be done between graphs. 
- Backend: When saving an asset, the generation window will no longer be black and it will keep the data consistently
- Backend: Pressing play in a scene with an Infinite Generator and the Graph Editor open should no longer break the graph/scene. 
- Backend: Null Texture or Null Vegetation Assets will no longer crash the graph or Unity
- Backend: Improved validation of Nodes so that it is more robust.
- Backed: General fixes in the Decimated Mesh mode to always work.
- Backend: General bug fixes
- Backend: General stability fixes

And many many more that I might not have been able to track...


# [0.6.3] - 2024-07-19
## Fix
- Texture and Vegetation Node: Fix where if empty or no connection would crash unity
- Backend: Reworked the validation of nodes to be more robust
- Backend: General bug fixes
- Backend: General stability fixes
# [0.6.2] - 2024-06-03
## Fix
- Backend: General bug fixing to allow non-breaking generation

# [0.6.1] - 2024-05-28
## Fixed
- Cavity Node: Fix where the border wasn't being generated correctly
- Samples: Corrected the sample to look slightly better and have textures in all the places
- Backend: Removed GetIndices method
- Backend: Finally fixed the generation of images

# [0.6.0] - 2024-05-26
## Added
- Added a Mask into the Warp Node
- Added Copy/Paste/Duplicate funcionality
- Added GetSlope node
- Added GetCavity node
- Added Blur Node
- Added the documentation button into each node
- Modifying a biome will also modify the terrain if both auto update are setup
- Support for Unity 2021.3
- Added export to image into the node editor
- Added About Window inside InfiniteLands/About

## Changed
- Renamed HeightMask into RangeMask
- Renamed CurveMask into Gate
- Renamed SlopeMask into NormalizedMask, no longer extracts the slope from the map
- Remap node separated into three nodes
    - Normalize 
    - New Range 
    - Curve 
- Warp node only uses one warp channel
- Added a range into lacunarity field in noise node
- Backend rework allowing for more modularity and many bug fixes
- Toggle the icon of visualize texture

## Fixed
- Step filter missing reference fix
- Fix octaves in noise to handle any amount of octaves without breaking
- Fixed Interpolate Node having the wrong minmax values (outputing wrong results 

# [0.5.1] - 2024-05-21
- Fix on Terrain Chunk. Null reference exception removed
- Samples are now enabled by default

# [0.5.0] - 2024-05-15
- First upload!