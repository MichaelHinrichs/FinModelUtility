# Fin Model Utility

## Credits

- cooliscool, as their Utility of Time program was used as the basis for the OoT exporter.
- @Gericom, as their MKDS Course Modifier program was used as the basis for the Pikmin 2 exporter.
- @intns, as their [MODConv](https://github.com/intns/MODConv) tool was ported to add support for Pikmin 1.

## Usage guide

For now, the only usable part is the Pikmin 2 model exporter:
- Drop a Pikmin2 rom in the "cli/roms/" directory. Make sure it's named "pkmn2.gcm"!
- Double-click "rip_pkmn2.bat" in the "cli/" directory. This will first rip all of the files from the game, and then the currently supported models. This can take a while on the first execution, but future executions will reuse the exported files.
- Exported models will appear within the "cli/out/" directory.
- The materials for most models are broken due to the complexity of recreating fixed-function pipeline effects in a standalone model. These will need to be manually recreated in whichever program you import the model into. To help with this process, a mat3.txt file is generated in each directory that contains a raw JSON representation of the materials.