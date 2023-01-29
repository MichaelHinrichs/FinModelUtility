# Fin Model Utility

![GitHub](https://img.shields.io/github/license/MeltyPlayer/FinModelUtility)
![Unit tests](https://github.com/MeltyPlayer/FinModelUtility/actions/workflows/dotnet.yml/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/MeltyPlayer/FinModelUtility/badge.svg)](https://coveralls.io/github/MeltyPlayer/FinModelUtility)

## Overview

Model viewer and command-line tools for extracting models from games en-masse. Separate batch scripts are provided for each supported game in order to simplify the process.

![image](https://user-images.githubusercontent.com/15970939/204156969-084cc1a4-1824-45c9-becc-e44ad69668d6.png)

## Credits

- [@Asia81](https://github.com/Asia81), whose [HackingToolkit9DS](https://github.com/Asia81/HackingToolkit9DS-Deprecated-) is used to extract the contents of 3DS .cias.
- [@Chadderz121](https://github.com/Chadderz121), AKA Chadderz, whose [CTools](https://www.chadsoft.co.uk/wiicoder/) suite is used to read .bmd texture formats.
- cooliscool, whose [Utility of Time](http://wiki.maco64.com/Tools/Utility_of_Time) program was used as the basis for the F3DZEX2/F3DEX2 importer.
- [@Cuyler36](https://github.com/Cuyler36), aka CulyerAC, whose [RELDumper](https://github.com/Cuyler36/RELDumper) is used to extract the contents of .rel/.map files.
- [@DickBlackshack](https://github.com/DickBlackshack), aka Rich Whitehouse, whose [Noesis](https://richwhitehouse.com/index.php?content=inc_projects.php&showproject=91) model viewer's plugins were leveraged to add support for various formats.
- [@DavidSM64](https://github.com/DavidSM64), whose [Quad64](https://github.com/DavidSM64/Quad64) is used as the base for the SM64 importer.
- [@EstevanBR](https://github.com/EstevanBR), whose [DATReaderC](https://github.com/EstevanBR/DATReaderC) was referenced as the starting point for the .dat importer.
- [@Gericom](https://github.com/Gericom), whose [MKDS Course Modifier](https://www.romhacking.net/utilities/1285/) program was used as the basis for the .bmd importer.
- [@hci64](https://github.com/hcs64), whose [vgm_ripping](https://github.com/hcs64/vgm_ripping) tool was ported to add support for parsing .AST PCM16 data.
- [@HimeWorks](https://github.com/HimeWorks), whose [Noesis plugins](https://himeworks.com/noesis-plugins/) were used to add support for various formats.
- [@intns](https://github.com/intns), whose [MODConv](https://github.com/intns/MODConv) tool was used as the basis for the .mod importer.
- [@IronLanguages](https://github.com/IronLanguages), whose [IronPython](https://github.com/IronLanguages/ironpython3) was used to add support for calling Python plugins from C#.
- [@jefffhaynes](https://github.com/jefffhaynes), whose [BinarySerializer](https://github.com/jefffhaynes/BinarySerializer) attribute library inspired the schema attributes for configuring how binary data is read.
- [@KillzXGaming](https://github.com/KillzXGaming), whose [Switch-Toolbox](https://github.com/KillzXGaming/Switch-Toolbox) was referenced to add support for LZSS decompression.
- [@kornman00](https://github.com/kornman00), AKA [@KornnerStudios](https://github.com/KornnerStudios), for documenting the Halo Wars formats in [HaloWarsDocs](https://github.com/HaloMods/HaloWarsDocs) and providing the [KSoft suite](https://github.com/KornnerStudios/KSoft) to extract the contents of the game.
- [@leftp](https://github.com/leftp), whose [SharpDirLister](https://github.com/EncodeGroup/SharpDirLister) API was used to dramatically improve listing out the file hierarchy.
- [@LordNed](https://github.com/LordNed), whose [J3D-Model-Viewer](https://github.com/LordNed/J3D-Model-Viewer) tool and [JStudio](https://github.com/LordNed/JStudio) library were referenced to fix bugs in the .bmd importer.
- [@M-1-RLG](https://github.com/M-1-RLG), AKA M-1, as his [io_scene_cmb](https://github.com/M-1-RLG/io_scene_cmb) Blender plugin was used as the basis for the .cmb importer. He also provided [thorough documentation](https://github.com/M-1-RLG/010-Editor-Templates/tree/master/Grezzo) on each of Grezzo's formats.
- [@magcius](https://github.com/magcius), AKA Jasper, as their animated model viewer was used as the basis for the .csab importer.
- [@mhvuze](https://github.com/mhvuze), whose [3ds-xfsatool](https://github.com/mhvuze/3ds-xfsatool) is used to extract XFSA archives.
- [@Nanook](https://github.com/Nanook), whose [NKitv1](https://github.com/Nanook/NKitv1) is used to read NKit ISOs.
- [@NerduMiner](https://github.com/NerduMiner), whose [Pikmin1Toolset](https://github.com/NerduMiner/Pikmin1Toolset) was ported from C++ to C# to add texture support for Pikmin 1.
- [@nickbabcock](https://github.com/nickbabcock), whose [Pfim](https://github.com/nickbabcock/Pfim) library is used to extract the contents of .dds images.
- [@nico](https://github.com/nico), AKA thakis, whose [szstools](http://amnoid.de/gc/) CLI is used to extract the contents of GameCube .isos and their [ddsview](http://www.amnoid.de/ddsview/index.html) tool was referenced to fix Halo Wars DXT5A/ATI1/BC4 parsing.
- [@Ploaj](https://github.com/Ploaj), whose [HSDLib](https://github.com/Ploaj/HSDLib) library was heavily referenced to fix the Super Smash Bros. Melee DAT importer and [Metanoia](https://github.com/Ploaj/Metanoia) library was heavily referenced to add support for parsing Level 5 formats. 
- [@revel8n](https://github.com/revel8n), whose [Smashboards thread](https://smashboards.com/threads/melee-dat-format.292603/) was referenced to add support for the .dat importer.
- [@RenolY2](https://github.com/RenolY2), AKA Yoshi2, whose [bw-model-viewer](https://github.com/RenolY2/bw-model-viewer) tool was used as the basis for the .modl importer.
- [@Sage-of-Mirrors](https://github.com/Sage-of-Mirrors), whose [SuperBMD](https://github.com/Sage-of-Mirrors/SuperBMD) tool was referenced to clean up the .bmd logic.
- [@Sergio0694](https://github.com/Sergio0694), whose [BinaryPack](https://github.com/Sergio0694/BinaryPack) generator inspired the schema source generator used to generate read/write methods.
- [@shravan2x](https://github.com/shravan2x), whose [Gameloop.Vdf](https://github.com/shravan2x/Gameloop.Vdf) library is used to deserialize the contents of Steam's .vdf files.
- [@srogee](https://github.com/srogee), as his [HaloWarsTools](https://github.com/srogee/HaloWarsTools) program was used as the basis of the Halo Wars importer.
- TTEMMA, whose [Gar/Zar UnPacker v0.2](https://gbatemp.net/threads/release-gar-zar-unpacker-v0-1.385264/) tool is used to extract the contents of Ocarina of Time 3D files.
- [@SuperHackio](https://github.com/SuperHackio), whose [Hack.io](https://github.com/SuperHackio/Hack.io) library was referenced to improve the BMD parser.
- Twili, for reverse-engineering and documenting the .zar archive format and various additional research.
- [@vgmstream](https://github.com/vgmstream), whose [https://github.com/vgmstream/vgmstream](https://github.com/vgmstream/vgmstream) tool was ported to add support for parsing .AST ADPCM data.
- [@VPenades](https://github.com/vpenades), who created the [SharpGLTF](https://github.com/vpenades/SharpGLTF) library used to write GLTF models.
- [@Wiimm](https://github.com/Wiimm), who created the [wiimms-iso-tools](https://github.com/Wiimm/wiimms-iso-tools) used to extract the contents of Wii .isos.
- [@xdanieldzd](https://github.com/xdanieldzd), for reverse-engineering and documenting the .cmb and .csab formats. In addition, their [Scarlet](https://github.com/xdanieldzd/Scarlet) tool was referenced for dumping .gar files.

## Supported formats/games

*Not all of the models for each game are currently supported, and not every feature of each model will be accurately recreated in the output GLTF/FBX files. To flag any broken/missing models that you'd like to see fixed, please feel free to file feedback via the Issues tab above.*

- J3D (.bmd/.bdl) (GCN)
  - Mario Kart: Double Dash (`mario_kart_double_dash.gcm`)
  - Pikmin 2 (`pikmin_2.gcm`)
  - Super Mario Sunshine (`super_mario_sunshine.gcm`)
- .cmb (3DS)
  - Ocarina of Time 3D (`ocarina_of_time_3d.cia`)
- .glo (PC)
  - Glover (Steam)
- .mod (GCN)
  - Pikmin 1 (`pikmin_1.gcm`)
- .modl (GCN/WII)
  - Battalion Wars 1 (`battalion_wars_1.gcm`)
  - Battalion Wars 2 (`battalion_wars_2.iso`)
- .xc (3DS)
  - Professor Layton vs. Phoenix Wright (`professor_layton_vs_phoenix_wright.cia`)

*Note:* For GameCube ROMs, files with an `.iso` extension should work as long as they are renamed to `[game_name].gcm`.

## Usage guide

Download a release via the Releases tab (for stability), or via the green "Code" button above (for latest). Extract this somewhere on your machine.

Then, follow the steps below.

0) For any 3DS titles, you must first install HackingToolkit9DS or else you won't be able to extract the contents of ROMs. This can be done by running `cli\tools\HackingToolkit9DSv12\SetupUS.exe`.
1) Drop ROM(s) in the `cli/roms/` directory. Make sure their names match the corresponding name above!

### Viewing/extracting models via the UI

2) Double-click the `launch_ui.bat` file in the `cli/` directory. This will first rip all of the files from the game, and then gather the currently supported models. This can take a while on the first execution, but future executions will reuse the exported files.
3) The viewer will open with nothing initially selected. Navigate through the models in the left-hand column and select a model you wish to view/extract. Once selected, the model will be loaded/rendered with as close of a representation to the original game as possible.
4) The model can be extracted by clicking the ![export](https://user-images.githubusercontent.com/15970939/204157246-43aa2e0d-628b-49c7-abb9-bfc2d52b16a0.png) icon in the top bar. It will automatically be placed relative to the `cli/out/[game_name]/` directory, in a chain of folders matching the ROM's original file structure. The model may not look exactly as it appeared in the viewer, because some material features are not supported in common model formats.
5) All of the models in a given directory can be extracted by clicking the ![export_all](https://user-images.githubusercontent.com/15970939/204157353-7b3dffb6-7061-4c3d-b90f-461764598b4d.png) icon in the top bar. *This cannot be stopped once started, so be aware of how many models you may be extracting at once.*

### Extracting all files from a given game via a batch script

2) Double-click the corresponding `rip_[game_name].bat` file in the `cli/` directory. This will first rip all of the files from the game, and then the currently supported models. This can take a while on the first execution, but future executions will reuse the exported files.
3) Extracted models will appear within the corresponding `cli/out/[game_name]/` directory.  

## Notes about exported models

- Both GLTF (.glb) and FBX models will be created when exporting, since each format has slightly different compatibility. FBX is generally preferred due to supporting additional UV channels, but GLTF is better supported within model viewer programs such as Noesis.
- The materials for some models are broken/incomplete due to the complexity of recreating fixed-function pipeline effects in a standalone model. These will need to be manually recreated in whichever program you import the model into.
