# Fin Model Utility

## Credits

- cooliscool, as their Utility of Time program was used as the basis for the OoT exporter.
- @Gericom, as their MKDS Course Modifier program was used as the basis for the Pikmin 2 exporter.

## Usage guide

For now, the only usable part is the Pikmin 2 model exporter:
- Drop a Pikmin2 rom in the "cli/roms/" directory.
- Double-click "ripper_static_verbose.bat" in the "cli/" directory. This will first rip all of the files from the game, and then the currently supported models. This can take a while on the first execution, but future executions will reuse the exported files.
- Exported models will appear within the "cli/out/" directory. Each model will contain a GLTF file and a FBX file. (The GLTF file is a middle step used when generating the FBX, one of the side effects is that its UVs are broken.)
- The materials for most models are broken due to the complexity of recreating fixed-function pipeline effects in a standalone model. These will need to be manually recreated in whichever program you import the model into. To help with this process, a mat3.txt file is generated in each directory that contains a raw JSON representation of the materials.