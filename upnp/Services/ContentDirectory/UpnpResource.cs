using System.Xml;
using utils.Logging;

namespace upnp.Services.ContentDirectory
{
    public class UpnpResource : IComparable<UpnpResource>
    {
        #region IComparable implementation

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }

        public static bool operator <(UpnpResource left, UpnpResource right)
        {
            if (left != null)
            {
                return left.CompareTo(right) < 0;
            }
            return false;
        }

        public static bool operator >(UpnpResource left, UpnpResource right)
        {
            if (left != null)
            {
                return left.CompareTo(right) > 0;
            }
            return false;
        }

        public int CompareTo(UpnpResource other)
        {
            if (other != null)
            {
                if (HorizontalResolution < other.HorizontalResolution)
                {
                    return -1;
                }
                if (HorizontalResolution > other.HorizontalResolution)
                {
                    return 1;
                }
            }
            return 0;
        }

        #endregion

        public override bool Equals(object obj)
        {
            var other = obj as UpnpResource;
            if (other != null)
            {
                return Uri == other.Uri;
            }
            return false;
        }

        public uint BitsPerSample
        {
            get { return _bitsPerSample; }
        }

        readonly uint _bitsPerSample = 0;

        // Number of audio channels, e.g., 1 for mono, 2 for stereo, 6 for Dolby Surround
        public uint NrAudioChannels
        {
            get { return _nrAudioChannels; }
        }

        readonly uint _nrAudioChannels = 0;

        // Sample audio freq in Hz
        public uint SampleFrequency
        {
            get { return _sampleFrequency; }
        }

        readonly uint _sampleFrequency = 0;

        // Bitrate in bytes/seconds of the encoding of the resource
        public uint Bitrate
        {
            get { return _bitrate; }
        }

        readonly uint _bitrate = 0;

        // The form of the duration string is:
        // H+:MM:SS[.F+], or H+:MM:SS[.F0/F1]
        //	where :
        // H+ : number of digits (including no digits) to indicate elapsed hours,
        // MM : exactly 2 digits to indicate minutes (00 to 59),
        // SS : exactly 2 digits to indicate seconds (00 to 59),
        // F+ : any number of digits (including no digits) to indicate fractions of seconds,
        // F0/F1 : a fraction, with F0 and F1 at least one digit long, and F0 < F1.
        // The string may be preceded by an optional + or – sign, and the decimal point itself may be omitted if there are no fractional second digits.
        public string Duration { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode"
        )]
        public string Uri { get; private set; }

        // XAuflösung in Pixel
        public int HorizontalResolution
        {
            get { return _horizontalPixels; }
        }

        readonly int _horizontalPixels = -1;

        // YAuflösung in Pixel
        public int VerticalResolution
        {
            get { return _verticalResolution; }
        }

        readonly int _verticalResolution = -1;

        public long SizeInBytes
        {
            get { return _sizeInBytes; }
        }

        readonly long _sizeInBytes = -1;

        public int ColorDepth
        {
            get { return _colorDepth; }
        }

        readonly int _colorDepth = -1;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode"
        )]
        public UpnpProtocolInfo ProtocolInfo { get; private set; }

        public UpnpResource(XmlReader reader)
        {
            if (reader.HasAttributes)
            {
                //Debug.WriteLine("Attributes of <" + reader.Name + ">");
                while (reader.MoveToNextAttribute())
                {
                    switch (reader.Name)
                    {
                        case "size":
                            _sizeInBytes = long.Parse(reader.Value);
                            break;
                        case "resolution":
                            _horizontalPixels = int.Parse(reader.Value.Split('x')[0]);
                            _verticalResolution = int.Parse(reader.Value.Split('x')[1]);
                            break;
                        case "protocolInfo":
                            ProtocolInfo = new UpnpProtocolInfo(reader.Value);
                            break;
                        case "colorDepth":
                            _colorDepth = int.Parse(reader.Value);
                            break;
                        case "duration":
                            Duration = reader.Value;
                            break;
                        case "bitrate":
                            uint.TryParse(reader.Value, out _bitrate);
                            break;
                        case "sampleFrequency":
                            uint.TryParse(reader.Value, out _sampleFrequency);
                            break;
                        case "nrAudioChannels":
                            uint.TryParse(reader.Value, out _nrAudioChannels);
                            break;
                        case "bitsPerSample":
                            uint.TryParse(reader.Value, out _bitsPerSample);
                            break;
                        case "microsoft:codec":
                            // scheinbar Guids, z.B. "{00000001-0000-0010-8000-00AA00389B71}" od "{00000055-0000-0010-8000-00AA00389B71}"
                            break;
                        default:
                            Log.LogWarning(
                                "Unknown attribute <{0}> with value <{1}> in res node.",
                                reader.Name,
                                reader.Value
                            );
                            break;
                    }
                }
                reader.MoveToElement();
            }

            reader.Read();
            Uri = reader.ReadContentAsString();
        }
    }
}
