using System.Collections.Generic;
using System.Net;

namespace Core.Contracts
{
    public interface IFlickrRequest
    {
        string Url { get; }
        int Count { get; set; }
        int Page { get; set; }
        string SearchTerm { get; set; }
        ICredentials Credentials { get; }
        IEnumerable<SearchItemResult> Parse(string xml);
    }
}