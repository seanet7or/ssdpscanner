using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using upnp.Soap;

namespace upnp.Services.ContentDirectory
{
    public class UpnpContentDirectory : UpnpService, IContentDirectory
    {
        #region IContentDirectory implementation

        public async Task GetSearchCapabilitiesAsync()
        {
            var searchCaps = await soapService.CallActionAsync("GetSearchCapabilities");
#if DEBUG
            if (searchCaps != null)
            {
                /*foreach (var searchCap in searchCaps)
                {
                    Debug.WriteLine(searchCap.Name + ": " + searchCap.Value);
                }*/
            }
#endif
        }

        public async Task GetSortCapabilitiesAsync()
        {
            var sortCaps = await soapService.CallActionAsync("GetSortCapabilities");
            if (sortCaps != null)
            {
                /*foreach (var sortCap in sortCaps)
                {
                    Debug.WriteLine(sortCap.Name + ": " + sortCap.Value);
                }*/
            }
        }

        public async Task<UInt32> GetSystemUpdateIdAsync()
        {
            var response = await soapService.CallActionAsync("GetSystemUpdateID");
            if (response != null)
            {
                var idArg = response.FirstOrDefault(a => a.Name == "Id");
                if (idArg != null)
                {
                    return idArg.ParseUi4();
                }
            }
            return 0;
        }

        public async Task<BrowseResponse?> BrowseAsync(
            ArgTypeObjectId objectId,
            ArgTypeBrowse browse,
            ArgTypeFilter filter,
            UInt32 startingIndex,
            UInt32 requestedCount,
            ArgTypeSortCriteria sortCriteria
        )
        {
            var inArgs = new Dictionary<string, string>();
            inArgs.Add("ObjectID", objectId.ToString());
            inArgs.Add("BrowseFlag", browse.ToString());
            inArgs.Add("Filter", filter.ToString());
            inArgs.Add("StartingIndex", startingIndex.ToString());
            inArgs.Add("RequestedCount", requestedCount.ToString());
            inArgs.Add("SortCriteria", sortCriteria.ToString());

            var soapResponse = await soapService.CallActionAsync("Browse", inArgs);

            if (soapResponse != null)
            {
                if (soapResponse.Count() >= 4)
                {
                    var resultXml = soapResponse.First(sr => sr.Name == "Result")?.Value;
                    var updateIdVal = soapResponse.First(sr => sr.Name == "UpdateID").Value;
                    var totalMatchesVal = soapResponse.First(sr => sr.Name == "TotalMatches").Value;
                    var numberReturnedVal = soapResponse
                        .First(sr => sr.Name == "NumberReturned")
                        .Value;
                    if (
                        resultXml != null
                        && updateIdVal != null
                        && totalMatchesVal != null
                        && numberReturnedVal != null
                    )
                    {
                        var response = new BrowseResponse(new ArgTypeResult(resultXml))
                        {
                            UpdateId = ArgTypeUpdateId.FromString(updateIdVal),
                            TotalMatches = ArgTypeCount.FromString(totalMatchesVal),
                            NumberReturned = ArgTypeCount.FromString(numberReturnedVal)
                        };
                        return response;
                    }
                }
            }

            return null;
        }

        #endregion

        readonly SoapService soapService;

        //readonly UpnpServiceDescription serviceDesc;

        public UpnpContentDirectory(UpnpServiceDescription serviceDesc, IHttpClient httpClient)
        {
            if (
                serviceDesc != null
                && serviceDesc.AbsoluteServiceControlUrl != null
                && serviceDesc.ServiceTypeUri != null
            )
            {
                //this.serviceDesc = serviceDesc;
                soapService = new SoapService(
                    serviceDesc.AbsoluteServiceControlUrl.ToString(),
                    serviceDesc.ServiceTypeUri,
                    httpClient
                );
            }
            else
            {
                throw new ArgumentException("serviceDesc may not be null");
            }
        }
    }
}
