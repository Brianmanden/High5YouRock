﻿using h5yr.Settings;
using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace h5yr.ViewComponents
{
    public class TweetsViewComponent : ViewComponent
    {
        private readonly ITwitterHelper twitterHelper;
        private readonly IOptions<APISettings> apiSettings;
      private readonly ILogger<TweetsViewComponent> logger;


        public TweetsViewComponent(ITwitterHelper twitterHelper, IOptions<APISettings> apiSettings, ILogger<TweetsViewComponent> logger)
        {
            this.twitterHelper = twitterHelper;
            this.apiSettings = apiSettings;
            this.logger = logger;
        }

        public IViewComponentResult Invoke(string loadmore = "false")
        {

            HttpContext.Session.SetInt32("NumberOfTweetsDisplayed", 12);

            List<TweetModel> tweet = new();

            if(apiSettings.Value.Offline == null || apiSettings.Value.Offline.ToLowerInvariant() != "true")
            {
                tweet = twitterHelper.GetAllTweets(0, 12);

                // You can only create a new TestTweets file if you have
                // a valid Twitter API key setup
                if (apiSettings.Value.CreateOfflineFile != null || apiSettings.Value.CreateOfflineFile?.ToLowerInvariant() == "true")
                {
                    try
                    {
                        var json = JsonConvert.SerializeObject(tweet);
                        string fileName = "TestTweets.json";
                        string jsonString = System.Text.Json.JsonSerializer.Serialize(json);
                        File.WriteAllText(fileName, jsonString);
                    }
                    catch(Exception ex)
                    {
                        logger.LogError("Error: Unable to write Test Tweets Json file", ex);
                    }
                    

                }
            }
            else
            {
                try
                {
                    string fileName = "TestTweets.json";
                    string jsonString = File.ReadAllText(fileName);
                    tweet = System.Text.Json.JsonSerializer.Deserialize<List<TweetModel>>(jsonString)!;
                }
                catch (Exception ex)
                {
                    logger.LogError("Error: Unable to read Tweet Json file", ex);
                }
                
            }

            return View(tweet);
        }

    }
}

