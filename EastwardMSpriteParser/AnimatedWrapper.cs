using System.Drawing;
using AnimatedGif;
using CMK;

namespace EastwardMSpriteParser;

public class AnimatedWrapper
{
    public enum Type
    {
        Apng,
        Gif,
        Webp
    }

    private readonly Type _type;
    private readonly AnimatedGifCreator? _gifCreator;
    private readonly AnimatedPngCreator? _pngCreator;

    public AnimatedWrapper(Type type, string path, int x, int y)
    {
        _type = type;
        var fs = File.Create(path);
        switch (_type)
        {
            case Type.Apng:
                _pngCreator = new AnimatedPngCreator(fs, x, y, new AnimatedPngCreator.Config()
                {
                    FilterUnchangedPixels = false
                });
                break;
            case Type.Gif:
                _gifCreator = new AnimatedGifCreator(fs);
                break;
            case Type.Webp:
            default:
                throw new NotSupportedException();
        }
    }

    public static string GetExtension(Type type)
    {
        switch (type)
        {
            case Type.Apng:
                return ".png";
            case Type.Gif:
                return ".gif";
            case Type.Webp:
                return ".webp";
            default:
                throw new NotSupportedException();
        }
    }

    public void WriteFrame(Image image, int delay)
    {
        switch (_type)
        {
            case Type.Apng:
                _pngCreator?.WriteFrame(image, (short)delay, 0, 0, 1);
                break;
            case Type.Gif:
                _gifCreator?.AddFrame(image, delay);
                break;
        }
    }
}