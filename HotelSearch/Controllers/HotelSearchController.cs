using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HotelSearch.Models;

namespace HotelSearch.Controllers
{
    public class HotelSearchController : ApiController
    {
        private string _city;
        private string _latitude, _longitude;
        private dynamic _apiResponse;
        private string address;
        private static List<Hotel> _hotel = new List<Hotel>();

        public string CreateResponse(string request)
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(request).ToString();
            }
        }
        [HttpGet]
        public HttpResponseMessage HotelSearch(string city)
        {
            _apiResponse = CreateResponse("https://autocomplete.geocoder.cit.api.here.com/6.2/suggest.json?query="+ city + "&&app_id=ZdbhbwyRlX7OiU16lgKI&app_code=gvJSaPdIdJ1xxYs2HaAuVQ");
            _apiResponse = JsonConvert.DeserializeObject(_apiResponse);
            try
            {
                _city = _apiResponse.suggestions[0].label;
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "No Hotels in Your Preferred Location ");
            }

          _apiResponse = JsonConvert.DeserializeObject(CreateResponse("https://api.opencagedata.com/geocode/v1/json?q="+_city+"&key=c10e1cb8013d4a309fbe7157d9bcc96d"));
           _latitude = _apiResponse.results[0].geometry.lat;
           _longitude = _apiResponse.results[0].geometry.lng;
           _apiResponse= JsonConvert.DeserializeObject(CreateResponse("https://api.foursquare.com/v2/venues/search?ll="+_latitude+","+_longitude+ "&query=hotel&radius=200000&client_id=OUXJ5AAJJZMHT52ARYP4CBG3WHL2K0N4TGRMUCVAHD1QFHR1&client_secret=GTV2NDSDBNIYWRHUKICYOVXDRNRW5ZW0WNUQOLPQUA41IMBH&v=20180805"));
            foreach (var hotel in _apiResponse.response.venues)
            {
                foreach (var hotelAddress in hotel.location.formattedAddress)
                    address += hotelAddress + ", ";
                _hotel.Add(
                    new Hotel()
                    {

                        HotelName = hotel.name,
                        HotelAddress = address,
                       // ContactNumber = hotel.contact.formattedPhone,
                    });
            }
            return Request.CreateResponse(HttpStatusCode.OK, _hotel);
        }
    }
}