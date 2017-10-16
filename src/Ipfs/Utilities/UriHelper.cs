using System;
using System.Collections.Generic;
using System.Linq;

namespace Ipfs.Utilities
{
    public static class UriHelper
    {
        public static Uri AppendPath(Uri baseUri, string path)
        {
            if(String.IsNullOrEmpty(path))
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
            if (args == null || args.Count() <= 0) { return baseUri; }

            var uriBuilder = new UriBuilder(baseUri);

            string query = uriBuilder.Query.TrimStart('?');

            if(!String.IsNullOrEmpty(query))
            {
                query += "&";
            }

            query += String.Join("&", args
                .Where(x=>!String.IsNullOrEmpty(x.Item1) && !String.IsNullOrEmpty(x.Item2))
                .Select(x => String.Format("{0}={1}", x.Item1, x.Item2)));

            uriBuilder.Query = query;

            return uriBuilder.Uri;
        }
    }
}
