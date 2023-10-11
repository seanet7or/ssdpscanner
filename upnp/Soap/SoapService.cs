using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace upnp.Soap
{
    class SoapService
    {
        readonly string controlUrl;
        readonly string serviceTypeUri;

        readonly IHttpClient httpClient;

        public SoapService(string controlUrl, string serviceTypeUri, IHttpClient httpClient)
        {
            this.controlUrl = controlUrl;
            this.serviceTypeUri = serviceTypeUri;
            this.httpClient = httpClient;
        }

        internal async Task<IEnumerable<ISoapActionResponseArgument>> CallActionAsync(
            string actionName
        )
        {
            return await CallActionAsync(actionName, null);
        }

        internal async Task<IReadOnlyList<ISoapActionResponseArgument>> CallActionAsync(
            string actionName,
            Dictionary<string, string> inArguments
        )
        {
            string argsData = string.Empty;
            if (inArguments != null)
            {
                foreach (var inArg in inArguments)
                {
                    argsData += "<" + inArg.Key + ">" + inArg.Value + "</" + inArg.Key + ">";
                }
            }

            string data =
                @"<?xml version=""1.0""?>
<s:Envelope
	xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""
	s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
	<s:Body>
		<u:"
                + actionName
                + @" xmlns:u="""
                + serviceTypeUri
                + @""">"
                + argsData
                + @"
		</u:"
                + actionName
                + @">
	</s:Body>
</s:Envelope>";
#if DEBUG
            //Debug.WriteLine(data);
#endif
            CancellationTokenSource cancelOnTimeout = new CancellationTokenSource();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "text/xml; charset=utf-8");
            headers.Add("SOAPACTION", "\"" + serviceTypeUri + "#" + actionName + "\"");

            var response = await httpClient.PostDataAsync(
                headers,
                Encoding.UTF8.GetBytes(data),
                controlUrl
            );

            // UPNP 1.1 3.2.2
            // The service MUST complete invoking the action and respond within 30 seconds, including expected transmission time
            // (measured from the time the action message is transmitted until the time the associated response is received).

#if DEBUG
            //Debug.WriteLine(response);
#endif

            var reader = XmlReader.Create(response);
            var outArguments = new List<ISoapActionResponseArgument>();
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.IsEmptyElement)
                    {
                        //Debug.WriteLine("<{0}/>", reader.Name);
                    }
                    else
                    {
                        if (reader.LocalName == "Envelope")
                        {
                            while (reader.Read())
                            {
                                if (reader.IsStartElement())
                                {
                                    if (reader.LocalName == "Body")
                                    {
                                        while (reader.Read())
                                        {
                                            if (reader.IsStartElement())
                                            {
                                                if (reader.LocalName == actionName + "Response")
                                                {
                                                    while (reader.Read())
                                                    {
                                                        if (reader.IsStartElement())
                                                        {
                                                            var name = reader.Name;
                                                            reader.Read();
                                                            var value =
                                                                reader.ReadContentAsString();
                                                            outArguments.Add(
                                                                new SoapActionResponseArgument
                                                                {
                                                                    Name = name,
                                                                    Value = value
                                                                }
                                                            );
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
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
            return outArguments;
        }
    }
}
