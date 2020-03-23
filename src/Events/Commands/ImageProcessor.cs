using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Events
{
    public class ImageProcessor
    {
        const string IMAGE_PREFIX = "$labyrinth-image";
        List<Image> _foundImages;
        public Image[] Process(Message message)
        {
            _foundImages = new List<Image>();
            ExtractImages(message.BodyParsed, message.Id);
            ExtractImages(message.BodyPlain, message.Id);

            foreach (var image in _foundImages)
            {
                message.BodyParsed = message.BodyParsed.Replace(image.Url, $"{IMAGE_PREFIX}/{image.Id}");
                message.BodyPlain = message.BodyPlain.Replace(image.Url, $"{IMAGE_PREFIX}/{image.Id}");
            }

            return _foundImages.ToArray();
        }

        private void ExtractImages(string message, string entityId)
        {
            var matches = ExtractImages(message);
            foreach (var image in matches)
            {
                if (!_foundImages.Any(i => i.Url == image.Url))
                {
                    image.FromEntityId = entityId;
                    image.Id = Guid.NewGuid().ToString();
                    
                    _foundImages.Add(image);
                }
            }
        }

        public Image[] ExtractImages(string text) {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);

            var imgs = htmlDoc.DocumentNode.SelectNodes("//img");

            var images = new List<Image>();

            if (imgs != null) {
                foreach (var img in imgs)
                {
                    var src = img.GetAttributeValue<string>("src", null);
                    var style = img.GetAttributeValue<string>("style", null);
                    int width = 0;
                    int height = 0;
                    if (style != null) {
                        var styleMatches = Regex.Matches(style, @"width:(\d+)(?:px)?; height:(\d+)(?:px)?")
                            .Cast<Match>()
                            .Single() .Groups
                            .Cast<Group>()
                            .Skip(1)
                            .Select(g => g.Value)
                            .ToArray();
                        width = int.Parse(styleMatches[0]);
                        height = int.Parse(styleMatches[1]);
                    } else {
                        width = img.GetAttributeValue<int>("width", 128);
                        height = img.GetAttributeValue<int>("height", 128);
                    }

                    images.Add(new Image { Url = src, Width = width, Height = height });
                }
            }

            return images.ToArray();
        }
    }
}