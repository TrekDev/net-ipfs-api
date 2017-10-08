using System;
using System.Collections.Generic;
using System.Linq;

namespace Ipfs.Utilities
{
    public static class UriHelper
    {
        public static Uri AppendPath(Uri baseUri, string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                return baseUri;
            }

            var uriBuilder = new UriBuilder(baseUri);

            string pathToAppend = path.TrimStart('/').TrimEnd('/');
            string oldPath = uriBuilder.Path.TrimEnd('/');

            uriBuilder.Path = oldPath + "/" + pathToAppend;

            return uriBuilder.Uri;
        }

        public static Uri AppendQuery(Uri baseUri, IEnumerable<Tuple<string,string>> args)
        {
            if (args == null || !args.Any())
            {
                return baseUri;
            }

            var uriBuilder = new UriBuilder(baseUri);

            string query = uriBuilder.Query.TrimStart('?');

            if(!string.IsNullOrEmpty(query))
            {
                query += "&";
            }

            query += string.Join("&", args
                .Where(x=>!string.IsNullOrEmpty(x.Item1) && !string.IsNullOrEmpty(x.Item2))
                .Select(x => string.Format("{0}={1}", x.Item1, x.Item2)));

            uriBuilder.Query = query;

            return uriBuilder.Uri;
        }
    }
}
