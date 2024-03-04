[0.1.0-pre-alpha.8] - 2024-03-04
Added
* Added ability to edit following UdonBehaviour symbols via UdonMonitor: Boolean, Int32, Single, String, Vector2, Vector3 and Color

Changes
* TransformEditor doesn't keep writing field values anymore if the editor is collapsed or the values haven't changed since the previous frame in order to save performance
* Applied consistent field naming to TransformEditor
* SceneViewCamera now updates on LateUpdate instead of Update
* UdonMonitor symbols and methods are now sorted alphabetically

Fixes
* Hierarchy will not crash anymore when it's empty

[0.1.0-pre-alpha.7] - 2023-11-05
Fixes
* Missing built-in icon ('d_SceneViewSnap-On@2x') in Unity 2022.3
* Disabled compression on loaded built-in editor icons (Unity 2022.3 warned about each icon)
* Replaced About window's banner's TMP sprite icon with individual UI Image (TPM with sprite refused to show up in 2022.3)
* Added UnityEditor.Experimental.SceneManagement under preprocessor define to prevent automatic API update prompt showing up in Unity 2022.3

[0.1.0-pre-alpha.6] - 2023-10-12
Added
* Toggle to constrain Transform scale proportions
* GameObject icon to hierarchy elements

Fixed
* Attempted fix for corrupted editors on builds
* UdonSharp dependency is now defined by com.vrchat.worlds >=3.4.0 <4.0.0
* Position handle planes from being able to be grabbed from further away from the center than intended

[0.1.0-pre-alpha.5] - 2023-06-13
Added
* Reconstructed UdonSharp proxy behaviour for UdonSharpBehaviours
* '(Script)' label on UdonBehaviour editor element header for consistency
* 'Send Custom Event' button and an event name field to UdonBehaviour editor
* Option to override the type name label on ObjectField (Useful for editor-only types)
* Basic HDR support to ColorField
* Scroll input to scene view
* Header to editor descriptor window settings panel
* Warning to editor descriptor when user attempts to scale the editor descriptor instead of canvas
* MeshCollider editor and visualizer
* AudioSource editor

Changes
* Udonity Editor Descriptor's placeholder canvas is now automatically hidden in hierarchy
* Removed obsolete debug logging
* Disabled UdonityLink toolbar menu item by default
* Scene view camera now defaults to Pan/Tilt instead of Orbit
* Disabled VRC InputField Keyboard Override on hierarchy search field
* Adjusted ranged float field slider style to match the original editor slider
* Set ranged float field size to fixed width of 50 pixels to match the original editor field
* Disabled toolbar 'File' dropdown menu due to none if the items being functional
* Scene view now renders Environment, PickupNoEnvironment and Walkthrough
* Welcome window now displays a prompt to load builtin editor icons after import if any new icons are missing

Fixes
* Scene view camera's pitch axis shouldn't get misaligned anymore if the editor is rotated
* Text-based objects in ObjectField (Displays name of asset instead of raw text contents)
* ObjectField won't attempt to ping the object anymore if the value is null
* Attempting to set ObjectField type to null defaults to typeof(Object)
* Presumably fixed an error with build postprocessor attempting to access destroyed scene roots by removing caching
* ColorFields now get properly initialized during build
* Color dialog now displays the original color properly on the comparison preview
* Scene view pointer coordinates are now translated to local space, preventing the inverted input when editor window is rotated
* Color dialog sliders now correctly display HSV and RGB based on the current mode
* Presumably fixed scene view alpha artifacts
* ObjectField label will now get refreshed on type change

Removed
* 'Open In UdonMonitor' menu item on UdonBehaviour editor context menu dropdown. This is now only available on UdonSharp proxy behaviour editor

[0.1.0-pre-alpha.4] - 2023-06-04
Added
* Instructions to editor descriptor about adding UdonAssetDatabase
* Basic EnumFlagsField class
* DropdownToggleMenu abstract class and default implementation
* Basic rotation navigation to scene viewport

Changes
* Udonity's version is now displayed in editor window's title, about window and crash dialog
* Current scene's name is now displayed in editor window's title and hierarchy scene header
* UdonMonitor now informs the user if a program couldn't be loaded
* All scene view toggles are now located in dropdowns
* Build postprocessor will now attempt to set UdonAssetDatabase's default logger to Udonity's console window
* Editor window application icons are now using PNG-based icons instead of SVG

Fixes
* Added missing error dialog icon
* Added missing window background
* Undo now detects RectTransforms as valid Transform undo targets
* Dragging and dropping scene roots to editor descriptors root list header now properly sets the object dirty
* AnnotationUtility should now be able to find annotations when setting Udonity scene icons disabled

[0.1.0-pre-alpha.3] - 2023-05-29
Fixes
* Improved hierarchy stability with added and removed objects
* Improved crash prevention of hierarchy search
* Prevented editor descriptor window scale from being smaller than 0.0001 or larger than 10
* Inspector now gets cleared if the inspected object gets deleted by user in order to prevent editor element crashes

[0.1.0-pre-alpha.2] - 2023-05-28
Added
* Custom editor for UdonityEditorDescriptor
* "Add Udonity Editor To Scene" button to the welcome window
* Undo support to utility for adding Udonity to scene
* Custom icon for UdonityRootInclusionDescriptor

Changes
* UdonityRootInclusionDescriptor's add component path is now "VUdon/Udonity/Udonity Root Inclusion Descriptor" instead of "VUdon/Udonity/Root Inclusion Descriptor", allowing it to be searched with "Udonity"
* Udonity's unnecessary scene view icons are now automatically disabled

Fixes
* Resolution adjustment on UdonityEditorDescriptor shouldn't break window layout anymore
* Disallowed multiple components of anything on same object
* (Presumably) fixed UdonSharp failing during build postprocessing, removing the requirement to modify UdonSharp in order to keep Udonity functional
* Included project window's generic file and folder icons in the package

[0.1.0-pre-alpha.1] - 2023-05-26
This is the first stable package for developer evaluation
