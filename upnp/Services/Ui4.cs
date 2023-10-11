using System.Globalization;

namespace upnp.Services
{
    public class Ui4<T>
        where T : Ui4<T>, new()
    {
        UInt32 val;

        public static T FromString(string source)
        {
            return new T { val = UInt32.Parse(source, CultureInfo.InvariantCulture) };
        }

        public static implicit operator UInt32(Ui4<T> ui4)
        {
            if (ui4 != null)
            {
                return ui4.val;
            }
            else
            {
                return 0;
            }
        }

        public static implicit operator Ui4<T>(UInt32 value)
        {
            return new Ui4<T> { val = value };
        }
    }
}
