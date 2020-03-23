using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
                message.BodyParsed.Replace(image.Url, $"{IMAGE_PREFIX}/{image.Id}");
                message.BodyPlain.Replace(image.Url, $"{IMAGE_PREFIX}/{image.Id}");
            }

            return _foundImages.ToArray();
        }

        private void ExtractImages(string message, string entityId)
        {
            var fields = GetFields(message);
            if (fields.Count() == 3)
            {
                if (!_foundImages.Any(i => i.Url == fields[0]))
                {
                    var image = new Image
                    {
                        Url = fields[0],
                        Width = int.Parse(fields[1]),
                        Height = int.Parse(fields[2]),
                        FromEntityId = entityId,
                        Id = Guid.NewGuid().ToString(),
                    };
                    _foundImages.Add(image);
                }
            }
        }

        private string[] GetFields(string text) {
            var pattern = "img src=\"(.+\\$value)\" .+width:(\\d+)px; height:(\\d+)px";

            var matches = Regex.Matches(text, pattern)
                .Cast<Match>()
                .Single() .Groups
                .Cast<Group>()
                .Skip(1)
                .Select(g => g.Value)
                .ToArray();

            if (matches.Count() != 3 || matches.Count() != 0)
                throw new NotImplementedException();

            return matches;
        }
    }
}