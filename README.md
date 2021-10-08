# Fin Model Utility

## Credits

- cooliscool, as their [Utility of Time](http://wiki.maco64.com/Tools/Utility_of_Time) program was used as the basis for the OoT exporter.
- @nico, AKA thakis, as their [szstools/yaz0dec](http://amnoid.de/gc/) CLI is used to extract the contents of GameCube ISOs.
- @Gericom, as their [MKDS Course Modifier](https://www.romhacking.net/utilities/1285/) program was used as the basis for the Pikmin 2 exporter.
- @intns, as their [MODConv](https://github.com/intns/MODConv) tool was ported to add general support for Pikmin 1.
- @NerduMiner, as their [Pikmin1Toolset](https://github.com/NerduMiner/Pikmin1Toolset) was ported to add texture support for Pikmin 1.

## Usage guide

Download this repository via the green "Code" button above and extract it to a directory on your machine. Then, follow the steps below.

### Pikmin 1

- Drop a Pikmin 1 rom in the "cli/roms/" directory. Make sure it's named "pkmn1.gcm"!
- Double-click "rip_pkmn1.bat" in the "cli/" directory. This will first rip all of the files from the game, and then the currently supported models. This can take a while on the first execution, but future executions will reuse the exported files.
- Exported models will appear within the "cli/out/pkmn1/" directory. Both GLTF (.glb) and FBX are exported, since each format has slightly different compatibility. FBX is generally preferred due to supporting additional UV channels.
- The materials for most models are broken due to the complexity of recreating fixed-function pipeline effects in a standalone model. These will need to be manually recreated in whichever program you import the model into.

### Pikmin 2

- Drop a Pikmin 2 rom in the "cli/roms/" directory. Make sure it's named "pkmn2.gcm"!
- Double-click "rip_pkmn2.bat" in the "cli/" directory. This will first rip all of the files from the game, and then the currently supported models. This can take a while on the first execution, but future executions will reuse the exported files.
- Exported models will appear within the "cli/out/pkmn2/" directory. Both GLTF (.glb) and FBX are exported, since each format has slightly different compatibility. FBX is generally preferred due to supporting additional UV channels.
- The materials for most models are broken due to the complexity of recreating fixed-function pipeline effects in a standalone model. These will need to be manually recreated in whichever program you import the model into. To help with this process, a mat3.txt file is generated in each directory that contains a raw JSON representation of the materials.