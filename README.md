# Fin Model Utility

Command-line tools for extracting models from games en-masse. Separate batch scripts are provided for each supported game in order to simplify the process.

## Credits

- cooliscool, as their [Utility of Time](http://wiki.maco64.com/Tools/Utility_of_Time) program was used as the basis for the OoT exporter.
- [@Asia81](https://github.com/Asia81) as their [HackingToolkit9DS](https://github.com/Asia81/HackingToolkit9DS-Deprecated-) is used to extract the contents of 3DS CIAs.
- TTEMMA, as their [Gar/Zar UnPacker v0.2](https://gbatemp.net/threads/release-gar-zar-unpacker-v0-1.385264/) tool is used to extract the contents of Ocarina of Time 3D files.
- [@nico](https://github.com/nico), AKA thakis, as their [szstools](http://amnoid.de/gc/) CLI is used to extract the contents of GameCube ISOs.
- [@Gericom](https://github.com/Gericom), as their [MKDS Course Modifier](https://www.romhacking.net/utilities/1285/) program was used as the basis for the Pikmin 2 exporter.
- [@intns](https://github.com/intns), as their [MODConv](https://github.com/intns/MODConv) tool was ported to add general support for Pikmin 1.
- [@NerduMiner](https://github.com/NerduMiner), as their [Pikmin1Toolset](https://github.com/NerduMiner/Pikmin1Toolset) was ported to add texture support for Pikmin 1.

## Usage guide

Download a release via the Releases tab (for stability), or via the green "Code" button above (for latest). Extract this somewhere on your machine.

Then, follow the steps below.

### Pikmin 1

- Drop a Pikmin 1 rom in the "cli/roms/" directory. Make sure it's named "pikmin_1.gcm"!
- Double-click "rip_pikmin_1.bat" in the "cli/" directory. This will first rip all of the files from the game, and then the currently supported models. This can take a while on the first execution, but future executions will reuse the exported files.
- Extracted models will appear within the "cli/out/pikmin_1/" directory. Both GLTF (.glb) and FBX are exported, since each format has slightly different compatibility. FBX is generally preferred due to supporting additional UV channels.
- The materials for some models are broken due to the complexity of recreating fixed-function pipeline effects in a standalone model. These will need to be manually recreated in whichever program you import the model into.

### Pikmin 2

- Drop a Pikmin 2 rom in the "cli/roms/" directory. Make sure it's named "pikmin_2.gcm"!
- Double-click "rip_pikmin_2.bat" in the "cli/" directory. This will first rip all of the files from the game, and then the currently supported models. This can take a while on the first execution, but future executions will reuse the exported files.
- Extracted models will appear within the "cli/out/pikmin_2/" directory. Both GLTF (.glb) and FBX are exported, since each format has slightly different compatibility. FBX is generally preferred due to supporting additional UV channels.
- The materials for some models are broken due to the complexity of recreating fixed-function pipeline effects in a standalone model. These will need to be manually recreated in whichever program you import the model into. To help with this process, a mat3.txt file is generated in each directory that contains a raw JSON representation of the materials.