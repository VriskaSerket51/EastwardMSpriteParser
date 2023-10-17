using CommandLine;

namespace EastwardMSpriteParser;

public class Options
{
    [Option('m', "msprite", Required = true, HelpText = "Set MSprite path.")]
    public required string MSpritePath { get; set; }

    [Option('t', "texture", Required = true, HelpText = "Set Texture Path.")]
    public required string TexturePath { get; set; }

    [Option('o', "output_dir", Required = true, HelpText = "Set output directory.")]
    public required string OutputDirectory { get; set; }

    [Option('s', "size", Required = false, HelpText = "Set size multiplier. Default value is 1", Default = 1)]
    public int SizeMultiplier { get; set; }

    [Option("gif", Required = false, HelpText = "Change output to gif. Default value is false", Default = false)]
    public bool GifMode { get; set; }
}