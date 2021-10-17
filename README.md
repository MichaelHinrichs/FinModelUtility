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

## Supported games

- Pikmin 1 (`pikmin_1.gcm`)
- Pikmin 2 (`pikmin_2.gcm`)
- Super Mario Sunshine (`super_mario_sunshine.gcm`)

## Usage guide

Download a release via the Releases tab (for stability), or via the green "Code" button above (for latest). Extract this somewhere on your machine.

Then, follow the steps below.

1) Drop the ROM in the `cli/roms/` directory. Make sure its name matches the corresponding name above!
2) Double click the corresponding `rip_[game_name].bat` file in the `cli/` directory. This will first rip all of the files from the game, and then the currently supported models. This can take a while on the first execution, but future executions will reuse the exported files.
3) Extracted models will appear within the corresponding `cli/out/[game_name]/` directory. Both GLTF (.glb) and FBX are exported, since each format has slightly different compatibility. FBX is generally preferred due to supporting additional UV channels, but GLTF is better supported within model viewer programs such as Noesis.
4) The materials for some models are broken/incomplete due to the complexity of recreating fixed-function pipeline effects in a standalone model. These will need to be manually recreated in whichever program you import the model into.