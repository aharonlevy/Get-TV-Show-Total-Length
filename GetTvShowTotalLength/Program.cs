
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace GetTvShowTotalLength
{
    class GetTvShowTotalLength
    {
        // Gets a single string as argv containing a tv show name
        // searches for the most recent show from TVmazes API
        // returns the watch time in minutes of the most recent show from the list
        private static string GetShowList(string showName, HttpClient client)
        {
            const string SEARCH_SHOWS_LIST = "https://api.tvmaze.com/search/shows?q=:";
            // This function get from TVmaze json of every show containing the showName in it
            // then the function will return the id of the most recent show according to the premire date

            Uri searchShows = new( SEARCH_SHOWS_LIST + showName);
            var result = client.GetAsync(searchShows).Result;
            while (true)
            {
                int http_status_code = (int)result.StatusCode;
                // Makes sure the API can handle the requst.
                if (http_status_code == 429)
                {
                    Thread.Sleep(10000);
                    result = client.GetAsync(searchShows).Result;
                    continue;
                }

                if (http_status_code == 200)
                {
                    break;
                }
                result = client.GetAsync(searchShows).Result;
            }
            Thread.Sleep(2000);
            return result.Content.ReadAsStringAsync().Result;
        }

        private static int GetMostRecentID(List<ShowData> showList)
        {
            // Make sure showList is valid
            if (showList == null)
            {
                return -2;
            }
            if (showList.Count <= 0)
            {
                return -1;
            }

            // Find the most recent show and return it's id

            // Loop thrugh the list find the most recent show with the same name as requsted
            int _mostRecent_id = -20;
            DateTimeOffset _mostRecentDate = DateTimeOffset.MinValue;
            foreach (ShowData show in showList)
            {                       
                if (show.show.premiered == null)
                {
                    continue;
                }
                if (DateTimeOffset.Compare(_mostRecentDate, (DateTimeOffset)show.show.premiered) < 0)
                {                
                    _mostRecentDate = (DateTimeOffset)show.show.premiered;
                    _mostRecent_id = show.show.id;
                }
            }
            return _mostRecent_id;
        }

        static int TotalTime(int id, HttpClient client)
        {
            // This function get a show id in TV maze site and a client object
            // the function then return the sum of minots needed to watch every episode that was aired

            int _curTime = 0;
            const string SEARCH_SHOW_EPP_LIST = "https://api.tvmaze.com/shows/";
            // Get the episode list from TVmaze 
            
            Uri _searchShows = new Uri(SEARCH_SHOW_EPP_LIST + id + "/episodes");
            var _result = client.GetAsync(_searchShows).Result;
            string jsonString = _result.Content.ReadAsStringAsync().Result;

            try
            {
                // Parse json and get the relevant data for every individoal show, we need: show id , a way to find
                List<Episode>? _EpisodesList_try = JsonSerializer.Deserialize<List<Episode>>(jsonString);
            }
            catch (Exception ex)
            {
                return -1;
            }
            List<Episode>? _EpisodesList = JsonSerializer.Deserialize<List<Episode>>(jsonString);

            if (_EpisodesList != null && _EpisodesList.Count > 0)
            {
                // Calculate the time of every episode in the epusode list
                foreach (Episode _episode in _EpisodesList)
                {
                    if (!(_episode.runtime is int) || (DateTimeOffset.Compare(_episode.airdate, second: DateTimeOffset.Now) > 0))
                    {
                        continue;
                    }
                    if (_episode.runtime is int value_runtime)
                    {
                        _curTime += value_runtime;

                    }
                }

            }
            return _curTime;
        }

        static void Main(string[] args)
        {
            var showList = new List<ShowData>();
            int totalTime = 0;

            // Connecet to TVmaze
            using (HttpClient client = new())
            {
                // Get a list of all the shows with this name as a json
                string jsonString = GetShowList(args[0], client);

                // Parse json and get the relevant data for every individoal show, we need: show id , a way to find
                showList = JsonSerializer.Deserialize<List<ShowData>>(jsonString);

            }
            int id = -1;         
                id = GetMostRecentID(showList);
            // Find id of most recent show.


            // "showList" is null
            if (id == -2)
            {
                Console.WriteLine("Show search error try again");
                Environment.Exit(2);
            }

            // "showList" is empty
            if (id == -1)
            {
                Environment.Exit(10);
            }

            using (HttpClient client = new())
            {
                // Get the show episode list, inclodeing spacials
                totalTime = TotalTime(id, client);
            }
            Console.WriteLine(totalTime);
        }

        // Defines class for the json parser
        public class ShowData
        {
            public float score { get; set; }
            public Show? show { get; set; }
        }
        public class Show
        {
            public int id { get; set; }

            public DateTimeOffset? premiered { get; set; }

        }

        public class Episode
        {
            public int? runtime { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public DateTimeOffset airdate { get; set; }        
        }
    }
}
