#r "Newtonsoft.Json"
#load "GeoCodeRequest.csx"
#load "GeoCodeResponse.csx"

using System.Net;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a GeoCode request. RequestUri={req.RequestUri}");
    string json = await req.Content.ReadAsStringAsync();

    GeoCodeRequest data = JsonConvert.DeserializeObject<GeoCodeRequest>(json);

    string address1 = data.address1;
    string address2 = data.address2;
    string city = data.city;
    string state = data.state;
    string zip = data.zip;

    var address = CreateAddress(address1, address2, city, state, zip);

    var xdoc = GeocodeRequest(address);

    decimal? latitude;
    decimal? longitude;
    if (!ParseGeocodeResponse(xdoc, out latitude, out longitude))
        return req.CreateResponse(HttpStatusCode.ServiceUnavailable, "Geocode Error");

    GeoCodeResponse response = new GeoCodeResponse();
    response.Latitude = latitude;
    response.Longitude = longitude;

    var r = req.CreateResponse(HttpStatusCode.OK);
    r.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json");
    return r;
}

private static bool ParseGeocodeResponse(XDocument xdoc, out decimal? latitude, out decimal? longitude)
{
    latitude = null;
    longitude = null;

    var xElement = xdoc.Element("GeocodeResponse");
    if (xElement == null)
        return false;

    var result = xElement.Element("result");
    if (result == null)
        return false;

    var element = result.Element("geometry");
    if (element == null)
        return false;

    var locationElement = element.Element("location");
    if (locationElement == null)
        return false;

    var latElement = locationElement.Element("lat");
    if (latElement != null)
        latitude = decimal.Parse(latElement.Value);

    var lngElement = locationElement.Element("lng");
    if (lngElement != null)
        longitude = decimal.Parse(lngElement.Value);

    return true;
}

private static XDocument GeocodeRequest(string address)
{
    var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

    var request = WebRequest.Create(requestUri);
    var response = request.GetResponse();
    var xdoc = XDocument.Load(response.GetResponseStream());

    return xdoc;
}

private static string CreateAddress(string address1, string address2, string city, string state, string zip)
{
    string address = address1;

    if (!string.IsNullOrEmpty(address2))
        address += " " + address2;

    if (!string.IsNullOrEmpty(city))
        address += " " + city;

    if (!string.IsNullOrEmpty(state))
        address += " " + state;

    if (!string.IsNullOrEmpty(zip))
        address += " " + zip;

    return address;
}