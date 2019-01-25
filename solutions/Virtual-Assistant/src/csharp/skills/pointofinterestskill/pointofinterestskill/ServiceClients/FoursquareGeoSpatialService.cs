﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PointOfInterestSkill.Models;
using PointOfInterestSkill.Models.Foursquare;

namespace PointOfInterestSkill.ServiceClients
{
    public sealed class FoursquareGeoSpatialService : IGeoSpatialService
    {
        private static readonly string SearchForVenuesUrl = $"https://api.foursquare.com/v2/venues/search?ll={{0}},{{1}}&query={{2}}&limit=3";
        private static readonly string ExploreNearbyVenuesUrl = $"https://api.foursquare.com/v2/search/explore?ll={{0}},{{1}}&limit=3";
        private static readonly string GetVenueDetailsUrl = $"https://api.foursquare.com/v2/venues/{{0}}?";
        private readonly string userLocale;
        private readonly string clientId;
        private readonly string clientSecret;

        /// <summary>
        /// Versioning is controlled by the v parameter, which is a date that represents the “version” of the API for which you expect from Foursquare.
        /// </summary>
        private readonly string apiVersion = "20190123";

        public FoursquareGeoSpatialService(string id, string secret, string locale = "en")
        {
            clientId = id;
            clientSecret = secret;
            userLocale = locale;
        }

        public async Task<RouteDirections> GetRouteDirectionsAsync(double currentLatitude, double currentLongitude, double destinationLatitude, double destinationLongitude, string routeType = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of venues near the provided coordinates, matching a search term.
        /// </summary>
        public async Task<List<PointOfInterestModel>> GetPointOfInterestByQueryAsync(double latitude, double longitude, string query, string country = null)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            return await GetVenueAsync(string.Format(CultureInfo.InvariantCulture, SearchForVenuesUrl, latitude, longitude, query));
        }

        /// <summary>
        /// This provider does not offer search by address.
        /// </summary>
        public async Task<List<PointOfInterestModel>> GetPointOfInterestByAddressAsync(string address)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This provider does not offer search by only coordinates.
        /// </summary>
        public async Task<List<PointOfInterestModel>> GetPointOfInterestByPointAsync(double latitude, double longitude)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get venue recommendations using the latitude and longitude of the user's location.
        /// </summary>
        public async Task<List<PointOfInterestModel>> GetLocationsNearby(double latitude, double longitude)
        {
            return await GetVenueAsync(
                string.Format(CultureInfo.InvariantCulture, ExploreNearbyVenuesUrl, latitude, longitude));
        }

        /// <summary>
        /// Returns available image from point of interest.
        /// </summary>
        public async Task<PointOfInterestModel> GetPointOfInterestDetails(PointOfInterestModel pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                throw new ArgumentNullException(nameof(pointOfInterest));
            }

            var pointOfInterestList = await GetVenueAsync(
                string.Format(CultureInfo.InvariantCulture, GetVenueDetailsUrl, pointOfInterest.Id));

            return pointOfInterestList.FirstOrDefault() ?? pointOfInterest;
        }

        private async Task<List<PointOfInterestModel>> GetVenueAsync(string url)
        {
            using (var client = new HttpClient())
            {
                url = url + $"&client_id={clientId}&client_secret={clientSecret}&v={apiVersion}";

                var response = await client.GetStringAsync(url);

                var apiResponse = JsonConvert.DeserializeObject<VenueResponse>(response);

                var pointOfInterestList = new List<PointOfInterestModel>();

                if (apiResponse?.Response != null)
                {
                    if (apiResponse.Response.Venue != null)
                    {
                        var venue = apiResponse.Response.Venue;
                        var newPointOfInterest = new PointOfInterestModel(venue);
                        pointOfInterestList.Add(newPointOfInterest);
                    }
                    else if (apiResponse?.Response?.Venues != null)
                    {
                        foreach (var venue in apiResponse.Response.Venues)
                        {
                            var newPointOfInterest = new PointOfInterestModel(venue);
                            pointOfInterestList.Add(newPointOfInterest);
                        }
                    }
                }

                return pointOfInterestList;
            }
        }
    }
}