using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using utils.Logging;

namespace upnp.Services
{
    public class StateVariable
    {
        internal string? Name { get; private set; }

        //string _dataType;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1823:AvoidUnusedPrivateFields"
        )]
        string? defaultValue;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode"
        )]
        internal IEnumerable<string>? AllowedValueList
        {
            get { return _allowedValueList; }
        }

        List<string>? _allowedValueList;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode"
        )]
        internal AllowedValueRange? AllowedValueRange { get; private set; }

        void ReadAllowedValueList(XmlReader reader)
        {
            _allowedValueList = new List<string>();
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name == "allowedValue")
                    {
                        reader.Read();
                        _allowedValueList.Add(reader.ReadContentAsString());
                    }
                    else
                    {
                        Log.LogWarning(
                            "Unexpected node {0} in allowed value list node",
                            reader.Name
                        );
                        reader.Read();
                        reader.ReadContentAsString();
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /*
        public StateVariable(XmlNode stateVarNode)
        {
            
            foreach (XmlNode node in stateVarNode.ChildNodes)
            {
                XmlNode dataTypeNode = stateVarNode["dataType"];
                if (dataTypeNode == null)
                {
                    Console.WriteLine("State variable node is missing dataType child node");
                }
                else
                {
                    _dataType = node.InnerText;

                    switch (_dataType)
                    {
                        case "ui1":
                            // Unsigned 1 Byte int. Same format as int without leading sign.
                            //AllowedValueRange = (IAllowedValueRange<object>)new AllowedValueRange<Byte>(stateVarNode["allowedValueRange"]);
                            break;
                        case "ui2":
                            // Unsigned 2 Byte int. Same format as int without leading sign.
                            //AllowedValueRange = (IAllowedValueRange<object>)new AllowedValueRange<UInt16>(stateVarNode["allowedValueRange"]);
                            break;
                        case "ui4":
                            // Unsigned 4 Byte int. Same format as int without leading sign.
                            //AllowedValueRange = (IAllowedValueRange<object>)new AllowedValueRange<UInt32>(stateVarNode["allowedValueRange"]);
                            break;
                        case "i1":
                            // 1 Byte int. Same format as int.
                            //AllowedValueRange = (IAllowedValueRange<object>)new AllowedValueRange<Char>(stateVarNode["allowedValueRange"]);
                            break;
                        case "i2":
                            // 2 Byte int. Same format as int.
                            //AllowedValueRange = (I//AllowedValueRange<object>)new //AllowedValueRange<Int16>(stateVarNode["allowedValueRange"]);
                            break;
                        case "i4":
                            // 4 Byte int. Same format as int. Must be between -2147483648 and 2147483647.
                            //AllowedValueRange = (I//AllowedValueRange<object>)new //AllowedValueRange<Int32>(stateVarNode["allowedValueRange"]);
                            break;
                        case "int":
                            // Fixed point, integer number. May have leading sign. May have leading zeros.
                            // (No currency symbol.) (No grouping of digits to the left of the decimal, e.g., no commas.)
                            //AllowedValueRange = (I//AllowedValueRange<object>)new //AllowedValueRange<Int64>(stateVarNode["allowedValueRange"]);
                            break;
                        case "r4":
                            // 4 Byte float. Same format as float. Must be between 3.40282347E+38 to 1.17549435E-38.
                            //AllowedValueRange = (I//AllowedValueRange<object>)new //AllowedValueRange<float>(stateVarNode["allowedValueRange"]);
                            break;
                        case "r8":
                            // 8 Byte float. Same format as float. Must be between -1.79769313486232E308 and -4.94065645841247E-324
                            // for negative values, and between 4.94065645841247E-324 and 1.79769313486232E308 for positive values,
                            // i.e., IEEE 64-bit (8-Byte) double.
                            //AllowedValueRange = (I//AllowedValueRange<object>)new //AllowedValueRange<double>(stateVarNode["allowedValueRange"]);
                            break;
                        case "number":
                            // Same as r8.
                            //AllowedValueRange = (I//AllowedValueRange<object>)new //AllowedValueRange<double>(stateVarNode["allowedValueRange"]);
                            break;
                        case "fixed.14.4":
                            // Same as r8 but no more than 14 digits to the left of the decimal point and no more than 4 to the right.
                            //AllowedValueRange = (I//AllowedValueRange<object>)new //AllowedValueRange<double>(stateVarNode["allowedValueRange"]);
                            break;
                        case "float":
                            // Floating point number. Mantissa (left of the decimal) and/or exponent may have a leading sign.
                            // Mantissa and/or exponent may have leading zeros. Decimal character in mantissa is a period, i.e.,
                            // whole digits in mantissa separated from fractional digits by period. Mantissa separated from
                            // exponent by E. (No currency symbol.) (No grouping of digits in the mantissa, e.g., no commas.)
                            //AllowedValueRange = (I//AllowedValueRange<object>)new //AllowedValueRange<double>(stateVarNode["allowedValueRange"]);
                            break;
                        case "char":
                            // Unicode string. One character long.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "string":
                            // Unicode string. No limit on length.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "date":
                            // Date in a subset of ISO 8601 format without time data.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "dateTime":
                            // Date in ISO 8601 format with optional time but no time zone.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "dateTime.tz":
                            // Date in ISO 8601 format with optional time and optional time zone.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "time":
                            // Time in a subset of ISO 8601 format with no date and no time zone.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "time.tz":
                            // Time in a subset of ISO 8601 format with optional time zone but no date.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "boolean":
                            // “0” for false or “1” for true. The values “true”, “yes”, “false”, or “no” may also be used but are not recommended.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "bin.base64":
                            // MIME-style Base64 encoded binary BLOB. Takes 3 Bytes, splits them into 4 parts, and maps each
                            // 6 bit piece to an octet. (3 octets are encoded as 4.) No limit on size.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "bin.hex":
                            // Hexadecimal digits representing octets. Treats each nibble as a hex digit and encodes as a separate Byte.
                            // (1 octet is encoded as 2.) No limit on size.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "uri":
                            // Universal Resource Identifier.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        case "uuid":
                            // Universally Unique ID. Hexadecimal digits representing octets. Optional embedded hyphens are ignored.
                            //ReadAllowedValueList(stateVarNode["allowedValueList"]);
                            break;
                        default:
                            if (!string.IsNullOrEmpty(_dataType))
                            {
#if DEBUG
                                Console.WriteLine("Unknown SOAP type {0}", _dataType);
#endif
                            }
                            break;
                    }
                }

                if (node.Name == "name")
                {
                    Name = node.InnerText;
                }
                else if (node.Name == "dataType")
                {
                    
                }
                else if (node.Name == "defaultValue")
                {
                    DefaultValue = node.InnerText;
                }
                else if (node.Name == "allowedValueList")
                {
                    ReadAllowedValueList(node);
                }
                else if (node.Name == "allowedValueRange")
                {
                    AllowedValueRange = new AllowedValueRange(node);
                }
                else
                {
                    Console.WriteLine("Unexpected node {0} in state variable node", node.Name);
                }
            }

        }
*/
        public StateVariable(XmlReader reader)
        {
            if (reader != null)
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "name":
                                reader.Read();
                                Name = reader.ReadContentAsString();
                                break;
                            case "dataType":
                                reader.Read();
                                reader.ReadContentAsString();
                                break;
                            case "defaultValue":
                                reader.Read();
                                defaultValue = reader.ReadContentAsString();
                                break;
                            case "allowedValueList":
                                ReadAllowedValueList(reader);
                                break;
                            case "allowedValueRange":
                                AllowedValueRange = new AllowedValueRange(reader);
                                break;
                            default:
                                reader.Read();
                                reader.ReadContentAsString();
                                break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
