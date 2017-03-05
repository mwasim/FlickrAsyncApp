using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Core.Contracts;

namespace Core
{
    public class FlickrRequest : IFlickrRequest
    {
        //https://www.flickr.com/services/api/keys/
        private const string AppId = "1c3d0f483bfecef4457f40522512cb29";

        public string Url
        {
            get
            {
                return $"https://api.flickr.com/services/rest?api_key={AppId}&method=flickr.photos.search&content_type=1&text={SearchTerm}&per_page={Count}&page={Page}";
            }
        }

        public FlickrRequest()
        {
            Count = 50;
            Page = 1;
        }


        public int Count { get; set; }
        public int Page { get; set; }

        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set { _searchTerm = value; }
        }

        public IEnumerable<SearchItemResult> Parse(string xml)
        {
            XElement respXml = XElement.Parse(xml);
            return (from item in respXml.Descendants("photo")
                    select new SearchItemResult
                    {
                        Title = new string(item.Attribute("title").Value.Take(50).ToArray()),
                        Url = string.Format("http://farm{0}.staticflickr.com/{1}/{2}_{3}_z.jpg",
                        item.Attribute("farm").Value, item.Attribute("server").Value, item.Attribute("id").Value,
                        item.Attribute("secret").Value),
                        ThumbnailUrl = string.Format("http://farm{0}.staticflickr.com/{1}/{2}_{3}_t.jpg",
                        item.Attribute("farm").Value, item.Attribute("server").Value, item.Attribute("id").Value,
                        item.Attribute("secret").Value),
                        Source = "Flickr"
                    }).ToList();
        }

        public ICredentials Credentials => null;
    }
}
