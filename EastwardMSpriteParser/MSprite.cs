using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SimpleJSON;

namespace EastwardMSpriteParser;

public class MSprite
{
    private struct Module
    {
        public readonly Rectangle Rect;

        public Module(Rectangle rect)
        {
            Rect = rect;
        }
    }

    private struct Frame
    {
        public readonly Dictionary<string, Point> Parts;

        public Frame(Dictionary<string, Point> parts)
        {
            Parts = parts;
        }
    }

    private struct Anim
    {
        public struct Sequence
        {
            public readonly string FrameId;
            public readonly float Delay;
            public readonly Point Origin;

            public Sequence(string frameId, float delay, Point origin)
            {
                FrameId = frameId;
                Delay = delay;
                Origin = origin;
            }
        }

        public readonly string Name;
        public readonly List<Sequence> Sequences;

        public Anim(string name, List<Sequence> sequences)
        {
            Name = name;
            Sequences = sequences;
        }
    }

    private readonly Dictionary<string, Module> _modules;
    private readonly Dictionary<string, Frame> _frames;
    private readonly Dictionary<string, Anim> _anims;
    private readonly Image _texture;
    private readonly int _multiplier;

    public MSprite(string json, Image texture, int multiplier)
    {
        _texture = texture;
        _multiplier = multiplier;
        var root = JSONNode.Parse(json);
        var modulesNode = root["modules"];
        _modules = new Dictionary<string, Module>(modulesNode.Count);
        foreach (var (id, value) in modulesNode)
        {
            var rectNode = value["rect"];
            Rectangle rect = new Rectangle(rectNode[0].AsInt, rectNode[1].AsInt, rectNode[2].AsInt, rectNode[3].AsInt);
            _modules[id] = new Module(rect);
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

            _frames[id] = new Frame(parts);
        }

        var animsNode = root["anims"];
        _anims = new Dictionary<string, Anim>(animsNode.Count);
        foreach (var (id, value) in animsNode)
        {
            var name = value["name"].Value;
            var seqNode = value["seq"].AsArray;
            var sequences = new List<Anim.Sequence>();
            foreach (var (_, sequenceNode) in seqNode)
            {
                var sequence = new Anim.Sequence(sequenceNode[0].Value, sequenceNode[1].AsFloat,
                    new Point(sequenceNode[2].AsInt, sequenceNode[3].AsInt));
                sequences.Add(sequence);
            }

            _anims.Add(id, new Anim(name, sequences));
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

    private Rectangle CalculateBound(IEnumerable<Frame> frames)
    {
        int xMin = int.MaxValue, yMin = int.MaxValue, xMax = int.MinValue, yMax = int.MinValue;
        foreach (var frame in frames)
        {
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
        }

        return new Rectangle(new Point(xMin, yMin), new Size(xMax - xMin, yMax - yMin));
    }

    public void ExtractTo(string path, AnimatedWrapper.Type type)
    {
        int idx = 1;
        foreach (var (_, anim) in _anims)
        {
            string name = anim.Name.Replace(":", "_") + AnimatedWrapper.GetExtension(type);
            Console.WriteLine($"Extracting {name}... ({idx}/{_anims.Count})");
            var rect = CalculateBound(anim.Sequences.Select(s => _frames[s.FrameId]));
            using var wrapper = new AnimatedWrapper(type, Path.Combine(path, name), rect.Width, rect.Height);

            foreach (var sequence in anim.Sequences)
            {
                var frame = _frames[sequence.FrameId];
                using var image = Frame2Image(frame);
                using Bitmap target = new Bitmap(rect.Width * _multiplier, rect.Height * _multiplier,
                    PixelFormat.Format32bppArgb);
                if (frame.Parts.Count > 0)
                {
                    var frameRect = CalculateBound(frame);
                    using Graphics g = Graphics.FromImage(target);
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(image,
                        new Rectangle((Point)(((Size)frameRect.Location - (Size)rect.Location) * _multiplier),
                            frameRect.Size * _multiplier),
                        frameRect with { X = 0, Y = 0 }, GraphicsUnit.Pixel);
                }

                wrapper.WriteFrame(target, (int)(sequence.Delay * 1000));
            }

            idx++;
        }

        Console.WriteLine("Extracting Finished!");
    }

    private Image Frame2Image(Frame frame)
    {
        var rect = CalculateBound(frame);
        Bitmap target = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(target);
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        foreach (var (moduleId, point) in frame.Parts)
        {
            var module = _modules[moduleId];
            g.DrawImage(_texture, new Rectangle(point - (Size)rect.Location, module.Rect.Size), module.Rect,
                GraphicsUnit.Pixel);
        }

        return target;
    }
}