using utils.Logging;

namespace ssdp
{
    /*
     *
     * 	https://www.ietf.org/rfc/rfc3986.txt
     *
     *    foo://example.com:8042/over/there?name=ferret#nose
     *    \_/   \______________/\_________/ \_________/ \__/
     *     |           |            |            |        |
     *  scheme     authority       path        query   fragment
     *     |   _____________________|__
     *    / \ /                        \
     *    urn:example:animal:ferret:nose
     *
    */
    public class Url
    {
        public override string ToString()
        {
            string res = string.Empty;
            if (HasScheme)
            {
                res = Scheme + ":";
            }
            if (HasAuthority)
            {
                res = res + "//" + Authority;
            }
            res += Path;
            if (HasQuery)
            {
                res = res + "?" + Query;
            }
            if (HasFragment)
            {
                res = res + "#" + Fragment;
            }
            return res;
        }

        bool HasScheme => !string.IsNullOrEmpty(Scheme);

        public string? Scheme { get; set; }

        public string? Authority { get; set; }

        bool HasAuthority => !string.IsNullOrEmpty(Authority);

        bool IsPathEmpty => string.IsNullOrEmpty(Path);

        string? Path { get; set; }

        string? Query { get; set; }

        bool HasQuery => !string.IsNullOrEmpty(Query);

        string? Fragment { get; set; }

        bool HasFragment => !string.IsNullOrEmpty(Fragment);

        public Url()
        {
            Scheme = string.Empty;
            Authority = string.Empty;
            Path = string.Empty;
            Query = string.Empty;
            Fragment = string.Empty;
        }

        public Url(string url)
        {
            if (url != null)
            {
                string urlWoQuery = url.Split('?')[0];
                string urlWoScheme;

                if (urlWoQuery.Contains(':'))
                {
                    Scheme = url.Split(':')[0];
                    urlWoScheme = url[(Scheme.Length + 1)..];
                }
                else
                {
                    Scheme = string.Empty;
                    urlWoScheme = url;
                }

                string pathQueryFragment;
                if (urlWoScheme.StartsWith("//", StringComparison.Ordinal))
                {
                    urlWoScheme = urlWoScheme[2..];
                    if (urlWoScheme.Contains('/'))
                    {
                        Authority = urlWoScheme.Split('/')[0];
                        pathQueryFragment = "/" + urlWoScheme.Split('/')[1];
                    }
                    else
                    {
                        Authority = urlWoScheme;
                        pathQueryFragment = string.Empty;
                    }
                }
                else
                {
                    Authority = string.Empty;
                    pathQueryFragment = urlWoScheme;
                }

                string queryFragment;
                if (pathQueryFragment.Contains('?'))
                {
                    Path = pathQueryFragment.Split('?')[0];
                    queryFragment = pathQueryFragment.Split('?')[1];
                }
                else
                {
                    Path = pathQueryFragment;
                    queryFragment = string.Empty;
                }

                if (queryFragment.Contains('#'))
                {
                    Query = queryFragment.Split('#')[0];
                    Fragment = queryFragment.Split('#')[1];
                }
                else
                {
                    Query = queryFragment;
                    Fragment = string.Empty;
                }
            }
        }

        // https://www.ietf.org/rfc/rfc3986.txt 5.2.2.
        public Url? ResolveRelativeToThisBaseUrl(Url relativeReference)
        {
            if (relativeReference != null)
            {
                Url target = new();

                if (relativeReference.HasScheme)
                {
                    target.Scheme = relativeReference.Scheme;
                    target.Authority = relativeReference.Authority;
                    target.Path = relativeReference.Path;
                    target.Query = relativeReference.Query;
                }
                else
                {
                    if (relativeReference.HasAuthority)
                    {
                        target.Authority = relativeReference.Authority;
                        target.Path = relativeReference.Path;
                        target.Query = relativeReference.Query;
                    }
                    else
                    {
                        if (relativeReference.IsPathEmpty)
                        {
                            target.Path = Path;
                            if (relativeReference.HasQuery)
                            {
                                target.Query = relativeReference.Query;
                            }
                            else
                            {
                                target.Query = Query;
                            }
                        }
                        else
                        {
                            string mergedPath;
                            if (IsPathEmpty)
                            {
                                if (
                                    relativeReference.Path != null
                                    && relativeReference.Path.StartsWith('/')
                                )
                                {
                                    mergedPath = relativeReference.Path;
                                }
                                else
                                {
                                    mergedPath = "/" + relativeReference.Path;
                                }
                            }
                            else
                            {
                                if (
                                    relativeReference.Path != null
                                    && relativeReference.Path.StartsWith('/')
                                )
                                {
                                    mergedPath =
                                        (Path != null ? RemoveLastSegment(Path) : "")
                                        + relativeReference.Path;
                                }
                                else
                                {
                                    mergedPath =
                                        (Path != null ? RemoveLastSegment(Path) : "")
                                        + "/"
                                        + relativeReference.Path;
                                }
                            }
                            target.Path = RemoveDotSegments(mergedPath);

                            target.Query = relativeReference.Query;
                        }
                        target.Authority = Authority;
                    }
                    target.Scheme = Scheme;
                }
                target.Fragment = relativeReference.Fragment;

                return target;
            }
            return null;
        }

        static string RemoveLastSegment(string path)
        {
            string res;
            int lastSep = path.LastIndexOf('/');
            if (lastSep >= 0)
            {
                res = path[..lastSep];
            }
            else
            {
                res = string.Empty;
            }
            return res;
        }

        // s. RFC 5.2.4
        static string RemoveDotSegments(string appendedPaths)
        {
            string input = appendedPaths;
            string output = string.Empty;

            try
            {
                while (!string.IsNullOrEmpty(input))
                {
                    if (input.StartsWith("../", StringComparison.Ordinal))
                    {
                        input = input[3..];
                    }
                    else if (input.StartsWith("./", StringComparison.Ordinal))
                    {
                        input = input[2..];
                    }
                    else if (input.StartsWith("/./", StringComparison.Ordinal))
                    {
                        input = "/" + input[3..];
                    }
                    else if (input.Equals("/."))
                    {
                        input = "/";
                    }
                    else if (input.StartsWith("/../", StringComparison.Ordinal))
                    {
                        input = "/" + input[4..];
                        output = RemoveLastSegment(output);
                    }
                    else if (input.Equals("/.."))
                    {
                        input = "/";
                        output = RemoveLastSegment(output);
                    }
                    else if (input.Equals("."))
                    {
                        input = string.Empty;
                    }
                    else if (input.Equals(".."))
                    {
                        input = string.Empty;
                    }
                    else
                    {
                        int segSep = input.IndexOf('/', 1);
                        if (segSep >= 0)
                        {
                            output += input[..segSep];
                            input = input[segSep..];
                        }
                        else
                        {
                            output += input;
                            input = string.Empty;
                        }
                    }
                }
                return output;
            }
            catch (Exception e)
            {
                Log.LogWarning(
                    "Relative path resolving failed for {0}: {1}",
                    appendedPaths,
                    e.ToString()
                );
                return string.Empty;
            }
        }
    }
}

/*
             * -- The URI reference is parsed into the five URI components
      --
      (R.scheme, R.authority, R.path, R.query, R.fragment) = parse(R);

          ***   else
        *** if defined(R.authority) then
          *** T.authority = R.authority;
          *** T.path      = remove_dot_segments(R.path);
          *** T.query     = R.query;
        *** else
           *** if (R.path == "") then
             ***  T.path = Base.path;
             ***  if defined(R.query) then
                ***  T.query = R.query;
                *** else
                ***  T.query = Base.query;
               **** endif;
          ***  else
            ***   if (R.path starts-with "/") then
                ***  T.path = remove_dot_segments(R.path);
              *** else
                  T.path = merge(Base.path, R.path);
                  T.path = remove_dot_segments(T.path);
            ***   endif;
             ***  T.query = R.query;
           *** endif;
          *** T.authority = Base.authority;
        *** endif;
        *** T.scheme = Base.scheme;
      *** endif;

      *** T.fragment = R.fragment;*/
