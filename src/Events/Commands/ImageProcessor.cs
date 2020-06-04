using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Graph;

namespace Events
{
    public class ImageProcessor
    {
        const string IMAGE_PREFIX = "$labyrinth-image";
        public static string ImagePath(Image image) => $"{IMAGE_PREFIX}/{image.Id}";
        List<Image> _foundImages;

        static string[] _excludeUrls = new [] {
            "statics.teams.cdn.office.net/evergreen-assets",
            "statics.teams.microsoft.com/evergreen-assets",
        };

        public Image[] GetImages(ChatMessage message, Network network)
        {
            _foundImages = new List<Image>();

            ExtractImages(message.Body.Content, message.Id, network);

            return _foundImages.ToArray();
        }

        private void ExtractImages(string message, string entityId, Network network)
        {
            var matches = ExtractImages(message);
            foreach (var image in matches)
            {
                if (!_foundImages.Any(i => i.Url == image.Url))
                {
                    image.FromEntityId = entityId;
                    image.Id = Guid.NewGuid().ToString();
                    image.Network = network;
                    
                    _foundImages.Add(image);
                }
            }
        }

        public Image[] ExtractImages(string text) {
            var htmlDoc = new HtmlDocument();
            
            if (text != null) // can be null in the case of deleted
                htmlDoc.LoadHtml(text);
            
            var imgs = htmlDoc.DocumentNode.SelectNodes("//img");

            var images = new List<Image>();

            if (imgs != null) {
                foreach (var img in imgs)
                {
                    var src = img.GetAttributeValue<string>("src", null);
                    var style = img.GetAttributeValue<string>("style", null);
                    var (width, height) = GetDimensions(img, style);

                    if (!_excludeUrls.Any(p => src.Contains(p)) && !string.IsNullOrWhiteSpace(src))
                        images.Add(new Image { Url = src, Width = width, Height = height });
                }
            }

            return images.ToArray();
        }

        private static (int width, int height) GetDimensions(HtmlNode img, string style)
        {
            int width = 0;
            int height = 0;
            if (style != null)
            {
                var styleMatches = Regex.Matches(style, @"width:(\d+)(?:px)?; height:(\d+)(?:px)?")
                    .GetGroupMatches();
                if (styleMatches.Count() == 2) {
                    width = int.Parse(styleMatches[0]);
                    height = int.Parse(styleMatches[1]);
                    return (width, height);
                }
            }
            
            width = img.GetAttributeValue<int>("width", 128);
            height = img.GetAttributeValue<int>("height", 128);
            
            return (width, height);
        }
    }
}