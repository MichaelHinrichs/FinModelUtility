# Fin Model Utility

Command-line tools for extracting models from games en-masse. Separate batch scripts are provided for each supported game in order to simplify the process.

## Credits

- [@Asia81](https://github.com/Asia81), as their [HackingToolkit9DS](https://github.com/Asia81/HackingToolkit9DS-Deprecated-) is used to extract the contents of 3DS .cias.
- [@Chadderz121](https://github.com/Chadderz121), AKA Chadderz, as their [CTools](https://www.chadsoft.co.uk/wiicoder/) suite was used to read .bmd texture formats.
- cooliscool, as their [Utility of Time](http://wiki.maco64.com/Tools/Utility_of_Time) program was used as the basis for the F3DZEX2/F3DEX2 importer.
- [@Cuyler36](https://github.com/Cuyler36), aka CulyerAC, as their [RELDumper](https://github.com/Cuyler36/RELDumper) is used to extract the contents of .rel/.map files.
- [@EstevanBR](https://github.com/EstevanBR), whose [DATReaderC](https://github.com/EstevanBR/DATReaderC) was referenced as the starting point for the .dat importer.
- [@Gericom](https://github.com/Gericom), as their [MKDS Course Modifier](https://www.romhacking.net/utilities/1285/) program was used as the basis for the .bmd importer.
- [@intns](https://github.com/intns), as their [MODConv](https://github.com/intns/MODConv) tool was used as the basis for the .mod importer.
- [@jefffhaynes](https://github.com/jefffhaynes), as their [BinarySerializer](https://github.com/jefffhaynes/BinarySerializer) attribute library inspired the schema attributes for configuring how binary data is read.
- [@KillzXGaming](https://github.com/KillzXGaming), whose [Switch-Toolbox](https://github.com/KillzXGaming/Switch-Toolbox) was referenced to add support for LZSS decompression.
- [@kornman00](https://github.com/kornman00), AKA [@KornnerStudios](https://github.com/KornnerStudios), for documenting the Halo Wars formats in [HaloWarsDocs](https://github.com/HaloMods/HaloWarsDocs) and providing the [KSoft suite](https://github.com/KornnerStudios/KSoft) to extract the contents of the game.
- [@LordNed](https://github.com/LordNed), whose [J3D-Model-Viewer](https://github.com/LordNed/J3D-Model-Viewer) tool and [JStudio](https://github.com/LordNed/JStudio) library were referenced to fix bugs in the .bmd importer.
- [@M-1-RLG](https://github.com/M-1-RLG), AKA M-1, as his [io_scene_cmb](https://github.com/M-1-RLG/io_scene_cmb) Blender plugin was used as the basis for the .cmb importer. He also provided [thorough documentation](https://github.com/M-1-RLG/010-Editor-Templates/tree/master/Grezzo) on each of Grezzo's formats.
- [@magcius](https://github.com/magcius), AKA Jasper, as their animated model viewer was used as the basis for the .csab importer.
- [@NerduMiner](https://github.com/NerduMiner), as their [Pikmin1Toolset](https://github.com/NerduMiner/Pikmin1Toolset) was ported to add texture support for Pikmin 1.
- [@nickbabcock](https://github.com/nickbabcock), whose [Pfim](https://github.com/nickbabcock/Pfim) library is used to extract the contents of .dds images.
- [@nico](https://github.com/nico), AKA thakis, as their [szstools](http://amnoid.de/gc/) CLI is used to extract the contents of GameCube .isos and their [ddsview](http://www.amnoid.de/ddsview/index.html) tool was referenced to fix Halo Wars DXT5A/ATI1/BC4 parsing.
- [@Ploaj](https://github.com/Ploaj), whose [HSDLib](https://github.com/Ploaj/HSDLib) library was heavily referenced to fix the Super Smash Bros. Melee DAT importer. 
- [@revel8n](https://github.com/revel8n), whose [Smashboards thread](https://smashboards.com/threads/melee-dat-format.292603/) was referenced to add support for the .dat importer.
- [@RenolY2](https://github.com/RenolY2), as their [bw-model-viewer](https://github.com/RenolY2/bw-model-viewer) tool was used as the basis for the .modl importer.
- [@Sage-of-Mirrors](https://github.com/Sage-of-Mirrors), as their [SuperBMD](https://github.com/Sage-of-Mirrors/SuperBMD) tool was referenced to clean up the .bmd logic.
- [@Sergio0694](https://github.com/Sergio0694), as their [BinaryPack](https://github.com/Sergio0694/BinaryPack) generator inspired the schema source generator used to generate read/write methods.
- [@shravan2x](https://github.com/shravan2x), as their [Gameloop.Vdf](https://github.com/shravan2x/Gameloop.Vdf) library is used to deserialize the contents of Steam's .vdf files.
- [@srogee](https://github.com/srogee), as his [HaloWarsTools](https://github.com/srogee/HaloWarsTools) program was used as the basis of the Halo Wars importer.
- TTEMMA, as their [Gar/Zar UnPacker v0.2](https://gbatemp.net/threads/release-gar-zar-unpacker-v0-1.385264/) tool is used to extract the contents of Ocarina of Time 3D files.
- Twili, for reverse-engineering and documenting the .zar archive format and various additional research.
- [@VPenades](https://github.com/vpenades), who created the [SharpGLTF](https://github.com/vpenades/SharpGLTF) library used to write GLTF models.
- [@Wiimm](https://github.com/Wiimm), who created the [wiimms-iso-tools](https://github.com/Wiimm/wiimms-iso-tools) used to extract the contents of Wii .isos.
- [@xdanieldzd](https://github.com/xdanieldzd), for reverse-engineering and documenting the .cmb and .csab formats. In addition, their [Scarlet](https://github.com/xdanieldzd/Scarlet) tool was referenced for dumping .gar files.=

## Supported formats/games

*Not all of the models for each game are currently supported, and not every feature of each model will be accurately recreated in the output GLTF/FBX files. To flag any broken/missing models that you'd like to see fixed, please feel free to file feedback via the Issues tab above.*

- .bmd (GCN)
  - Mario Kart: Double Dash (`mario_kart_double_dash.gcm`)
  - Pikmin 2 (`pikmin_2.gcm`)
  - Super Mario Sunshine (`super_mario_sunshine.gcm`)
- .cmb (3DS)
  - Ocarina of Time 3D (`ocarina_of_time_3d.cia`)
- .mod (GCN)
  - Pikmin 1 (`pikmin_1.gcm`)
- .modl (GCN)
  - Battalion Wars 1 (`battalion_wars_1.gcm`)

*Note:* For GameCube ROMs, files with an `.iso` extension should work as long as they are renamed to `[game_name].gcm`.

## Usage guide

Download a release via the Releases tab (for stability), or via the green "Code" button above (for latest). Extract this somewhere on your machine.

Then, follow the steps below.

0) For any 3DS titles, you must first install HackingToolkit9DS or else you won't be able to extract the contents of ROMs. This can be done by running `cli\tools\HackingToolkit9DSv12\SetupUS.exe`.
1) Drop the ROM in the `cli/roms/` directory. Make sure its name matches the corresponding name above!
2) Double click the corresponding `rip_[game_name].bat` file in the `cli/` directory. This will first rip all of the files from the game, and then the currently supported models. This can take a while on the first execution, but future executions will reuse the exported files.
3) Extracted models will appear within the corresponding `cli/out/[game_name]/` directory. Both GLTF (.glb) and FBX are exported, since each format has slightly different compatibility. FBX is generally preferred due to supporting additional UV channels, but GLTF is better supported within model viewer programs such as Noesis.
4) The materials for some models are broken/incomplete due to the complexity of recreating fixed-function pipeline effects in a standalone model. These will need to be manually recreated in whichever program you import the model into.
