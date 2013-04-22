using HackerCentral.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace HackerCentral.Accessors
{
    public class ValidityRatingAccessor
    {
        private int conversationId;
        private string apiKey;
        private int userId;

        public ValidityRatingAccessor()
        {
            conversationId = AthenaBridgeAPISettings.CONVERSATION_ID;
            apiKey = AthenaBridgeAPISettings.API_KEY;
            userId = AthenaBridgeAPISettings.USER_ID;
        }

        public List<ValidityRating> GetAllValidityRatings()
        {
            string api_url = String.Format("http://athenabridge.com/api/{0}/{1}/validity_ratings", apiKey, conversationId);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = string.Empty;
                    jsonData = w.DownloadString(api_url);
                    List<ValidityRating> ratings = JsonConvert.DeserializeObject<List<ValidityRating>>(jsonData);
                    if (ratings.Count > 0)
                        return ratings;
                    else
                        return new List<ValidityRating>();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public ValidityRating GetValidityRating(int ratingId)
        {
            string api_url = String.Format("http://athenabridge.com/api/{0}/{1}/validity_ratings/{2}/show", apiKey, conversationId, ratingId);

            using (var w = new WebClient())
            {
                try
                {
                    var jsonData = string.Empty;
                    jsonData = w.DownloadString(api_url);
                    ValidityRating rating = JsonConvert.DeserializeObject<ValidityRating>(jsonData);
                    return rating;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public bool CreateValidityRating(ValidityRating rating)
        {
            string api_url = String.Format("http://athenabridge.com/api/{0}/{1}/validity_ratings/create", apiKey, conversationId);

            try
            {
                var client = new RestClient();
                var request = new RestRequest(api_url, Method.POST);
                request.AddParameter("user_id", userId);
                request.AddParameter("point_id", rating.point_id);
                request.AddParameter("rating_value", rating.validity_rating);
                var response = client.Execute(request);
                var content = response.Content;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateValidityRatin(ValidityRating rating)
        {
            return CreateValidityRating(rating);
        }

        public bool DestroyValidityRating(long id)
        {
            string api_url = String.Format("http://athenabridge.com/api/{0}/{1}/validity_ratings/{2}/destroy", apiKey, conversationId, id);

            try
            {
                var client = new RestClient();
                var request = new RestRequest(api_url);
                var response = client.Execute(request);
                var content = response.Content;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        
    }
}