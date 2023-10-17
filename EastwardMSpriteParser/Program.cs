using System.Drawing;
using CommandLine;
using EastwardMSpriteParser;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(OnParsed);

void OnParsed(Options o)
{
    if (!File.Exists(o.MSpritePath))
    {
        Console.WriteLine($"No MSprite Found at: {o.MSpritePath}");
        return;
    }

    if (!File.Exists(o.TexturePath))
    {
        Console.WriteLine($"No Texture Found at: {o.TexturePath}");
        return;
    }

    if (!Directory.Exists(o.OutputDirectory))
    {
        Directory.CreateDirectory(o.OutputDirectory);
    }

    if (o.SizeMultiplier < 1)
    {
        o.SizeMultiplier = 1;
    }

    string json = File.ReadAllText(o.MSpritePath);
    Image texture = Image.FromFile(o.TexturePath);
    var mSprite = new MSprite(json, texture, o.SizeMultiplier);
    mSprite.ExtractTo(o.OutputDirectory, o.GifMode ? AnimatedWrapper.Type.Gif : AnimatedWrapper.Type.Apng);
}