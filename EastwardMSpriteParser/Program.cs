using System.Drawing;
using CommandLine;
using EastwardMSpriteParser;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(OnParsed);

// OnParsed(new Options());

void OnParsed(Options o)
{
    if (!File.Exists(o.MSpritePath))
    {
        Console.WriteLine($"No MSprite Fount at: {o.MSpritePath}");
        return;
    }

    if (!File.Exists(o.TexturePath))
    {
        Console.WriteLine($"No Texture Fount at: {o.TexturePath}");
        return;
    }

    if (!Directory.Exists(o.OutputDirectory))
    {
        Directory.CreateDirectory(o.OutputDirectory);
    }

    string json = File.ReadAllText(o.MSpritePath);
    Image texture = Image.FromFile(o.TexturePath);
    var mSprite = new MSprite(json, texture);
    mSprite.ExtractTo(o.OutputDirectory);
}