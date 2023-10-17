# EastwardMSpriteParser
Eastward MSprite Parser

Read ".msprite" file and texture file, then extract to output directory.
Default extracted file extension is ".apng".

Parameters:
- -m / --msprite: Set MSprite path.
- -t / --texture: Set Texture Path.
- -o / -output_dir: Set output directory.
- -s / --size: Set size multiplier. Default value is 1

Usage example: `EastwardMSpriteParser.exe -m sam.msprite -t sam_texture.png -o ./output -s 4`