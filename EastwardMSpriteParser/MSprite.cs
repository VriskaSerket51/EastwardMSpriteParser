using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using SimpleJSON;

namespace EastwardMSpriteParser;

public class MSprite
{
    private struct Module
    {
        public string Id;
        public Rectangle Rect;

        public Module(string id, Rectangle rect)
        {
            Id = id;
            Rect = rect;
        }
    }

    private struct Frame
    {
        public string Id;
        public Dictionary<string, Point> Parts;

        public Frame(string id, Dictionary<string, Point> parts)
        {
            Id = id;
            Parts = parts;
        }
    }

    private readonly Dictionary<string, Module> _modules;
    private readonly Dictionary<string, Frame> _frames;
    private readonly Image _texture;

    public MSprite(string json, Image texture)
    {
        _texture = texture;
        var root = JSONNode.Parse(json);
        var modulesNode = root["modules"];
        _modules = new Dictionary<string, Module>(modulesNode.Count);
        foreach (var (id, value) in modulesNode)
        {
            var rectNode = value["rect"];
            Rectangle rect = new Rectangle(rectNode[0].AsInt, rectNode[1].AsInt, rectNode[2].AsInt, rectNode[3].AsInt);
            _modules[id] = new Module(id, rect);
        }

        var framesNode = root["frames"];
        _frames = new Dictionary<string, Frame>(framesNode.Count);
        foreach (var (id, value) in framesNode)
        {
            var partsNode = value["parts"];
            var parts = new Dictionary<string, Point>(partsNode.Count);
            foreach (var (_, partNode) in partsNode)
            {
                parts.Add(partNode[0].Value, new Point(partNode[1].AsInt, partNode[2].AsInt));
            }

            _frames[id] = new Frame(id, parts);
        }
    }

    private Rectangle CalculateBound(Frame frame)
    {
        int xMin = int.MaxValue, yMin = int.MaxValue, xMax = int.MinValue, yMax = int.MinValue;
        foreach (var (moduleId, point) in frame.Parts)
        {
            var module = _modules[moduleId];
            Point min = point;
            Point max = min + new Size(module.Rect.Width, module.Rect.Height);

            if (xMin > min.X)
            {
                xMin = min.X;
            }

            if (yMin > min.Y)
            {
                yMin = min.Y;
            }

            if (xMax < max.X)
            {
                xMax = max.X;
            }

            if (yMax < max.Y)
            {
                yMax = max.Y;
            }
        }

        return new Rectangle(new Point(xMin, yMin), new Size(xMax - xMin, yMax - yMin));
    }

    public void ExtractTo(string path)
    {
        foreach (var (frameId, frame) in _frames)
        {
            var rect = CalculateBound(frame);

            using Bitmap target = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using Graphics g = Graphics.FromImage(target);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            foreach (var (moduleId, point) in frame.Parts)
            {
                var module = _modules[moduleId];
                g.DrawImage(_texture, new Rectangle((Point)((Size)point - (Size)rect.Location), module.Rect.Size),
                    module.Rect,
                    GraphicsUnit.Pixel);
            }

            target.Save(Path.Combine(path, frameId + ".png"), ImageFormat.Png);
        }
    }
}