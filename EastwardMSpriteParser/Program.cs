using System.Drawing;
using EastwardMSpriteParser;

string json = File.ReadAllText(@"anim\npc_sam.json");
Image texture = Image.FromFile(@"anim\npc_sam_texture.png");
var mSprite = new MSprite(json, texture);
mSprite.ExtractTo(@"anim\frames");