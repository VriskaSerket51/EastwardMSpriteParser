﻿using CommandLine;

namespace EastwardMSpriteParser;

public class Options
{
    [Option('m', "mSprite", Required = true, HelpText = "Set MSprite path.")]
    public string MSpritePath { get; set; }

    [Option('t', "texture", Required = true, HelpText = "Set Texture Path.")]
    public string TexturePath { get; set; }

    [Option('o', "output_dir", Required = true, HelpText = "Set output directory.")]
    public string OutputDirectory { get; set; }
}