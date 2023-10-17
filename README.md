# EastwardMSpriteParser
Eastward MSprite Parser

Read ".msprite" file and texture file, then extract to output directory.
Default extracted file type is apng(Animated PNG).

Parameters:
- -m or --msprite: Set MSprite path.
- -t or --texture: Set Texture Path.
- -o or -output_dir: Set output directory.
- -s or --size: Set size multiplier. Default value is 1
- --gif: Output files will be ".gif" instead of apng files.

Usage example: `EastwardMSpriteParser.exe -m sam.msprite -t sam_texture.png -o ./output`