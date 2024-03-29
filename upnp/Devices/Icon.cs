﻿using System.Globalization;
using System.Xml;
using ssdp;
using utils.Logging;

namespace upnp.Devices
{
    public class Icon : IComparable<Icon>
    {
        #region IComparable implementation

        public override int GetHashCode()
        {
            return _relativeUrl != null ? _relativeUrl.GetHashCode() : 0;
        }

        public static bool operator <(Icon left, Icon right)
        {
            if (left != null)
            {
                return left.CompareTo(right) < 0;
            }
            return false;
        }

        public static bool operator >(Icon left, Icon right)
        {
            if (left != null)
            {
                return left.CompareTo(right) > 0;
            }
            return false;
        }

        public int CompareTo(Icon? other)
        {
            if (other != null)
            {
                if (Width < other.Width)
                {
                    return -1;
                }
                if (Width > other.Width)
                {
                    return 1;
                }
            }
            return 0;
        }

        #endregion

        public override bool Equals(object? obj)
        {
            if (obj is Icon other)
            {
                return this == other;
            }
            return false;
        }

        public Url? AbsoluteUrl =>
            _relativeUrl != null
                ? device.BaseUrl?.ResolveRelativeToThisBaseUrl(_relativeUrl)
                : null;

        readonly Url? _relativeUrl;

#pragma warning disable IDE0052 // Remove unread private members
        // Required. Number of color bits per pixel. Integer.
        readonly int depth;
#pragma warning restore IDE0052 // Remove unread private members

        // Required. Horizontal dimension of icon in pixels. Integer.
        public int Width { get; private set; }

        // Required. Vertical dimension of icon in pixels. Integer.
        public int Height { get; private set; }

        // Required. Icon's MIME type (cf. RFC 2045, 2046, and 2387). Single MIME image type.
        // At least one icon should be of type “image/png” (Portable Network Graphics, see IETF RFC 2083).
        internal string? mimetype;

        readonly UpnpDevice device;

        internal Icon(XmlReader reader, UpnpDevice device)
        {
            this.device = device;

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "mimetype":
                            reader.Read();
                            mimetype = reader.ReadContentAsString();
                            break;
                        case "width":
                            reader.Read();
                            Width = int.Parse(
                                reader.ReadContentAsString(),
                                CultureInfo.InvariantCulture
                            );
                            break;
                        case "height":
                            reader.Read();
                            Height = int.Parse(
                                reader.ReadContentAsString(),
                                CultureInfo.InvariantCulture
                            );
                            break;
                        case "depth":
                            reader.Read();
                            depth = int.Parse(
                                reader.ReadContentAsString(),
                                CultureInfo.InvariantCulture
                            );
                            break;
                        case "url":
                            reader.Read();
                            _relativeUrl = new Url(reader.ReadContentAsString());
                            break;
                        default:
                            Log.LogWarning("Unexpected Node in icon node: " + reader.Name);
                            reader.Read();
                            reader.ReadContentAsString();
                            break;
                    }
                }
                else
                {
                    return;
                }
            }
            //Debug.WriteLine("Found {0}x{1} icon", Width, Height);
        }
    }
}
