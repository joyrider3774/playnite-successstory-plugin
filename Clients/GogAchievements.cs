﻿using GogLibrary.Services;
using Newtonsoft.Json.Linq;
using Playnite.SDK;
using SuccessStory.Database;
using SuccessStory.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SuccessStory.Clients
{
    //https://gogapidocs.readthedocs.io/en/latest/
    // TODO GOG localization
    class GogAchievements
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private static IResourceProvider resources = new ResourceProvider();

        private GogAccountClient gogAPI;

        internal async Task<string> DonwloadStringData(string url1, string url2, string token)
        {
            using (var client = new HttpClient())
            {
                string resultLang = await client.GetStringAsync(url1).ConfigureAwait(false);

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                string result = await client.GetStringAsync(url2).ConfigureAwait(false);

                return result;
            }
        }


        public GameAchievements GetAchievements(IPlayniteAPI PlayniteApi, Guid Id)
        {
            GameAchievements Result = new GameAchievements();

            List<Achievements> Achievements = new List<Achievements>();
            string GameName = PlayniteApi.Database.Games.Get(Id).Name;
            string ClientId = PlayniteApi.Database.Games.Get(Id).GameId;
            bool HaveAchivements = false;
            int Total = 0;
            int Unlocked = 0;
            int Locked = 0;

            string ResultWeb = "";

            var view = PlayniteApi.WebViews.CreateOffscreenView();
            gogAPI = new GogAccountClient(view);

            // Only if user is logged. 
            if (gogAPI.GetIsUserLoggedIn())
            {
                string accessToken = gogAPI.GetAccountInfo().accessToken;

                string userId = gogAPI.GetAccountInfo().userId;
                string lang = resources.GetString("LOCLanguageCode");

                string[] arrayLang = { "de", "en", "fr", "ru", "zh", "zh-Hans" };
                if (!arrayLang.ContainsString(lang))
                {
                    lang = "en";
                }

                // Achievements
                    string url = string.Format(@"https://gameplay.gog.com/clients/{0}/users/{1}/achievements",
                    ClientId, userId);

                //logger.Debug($"SuccessStory - GOG.GetAchievements {url}");
                //logger.Debug($"SuccessStoryToken - {accessToken}");

                try
                {
                    string urlLang = string.Format(@"https://www.gog.com/user/changeLanguage/{0}", lang.ToLower());  
                    ResultWeb = DonwloadStringData(urlLang, url, accessToken).GetAwaiter().GetResult();

                    //logger.Debug(ResultWeb);
                }
                // TODO Environnement
                //catch (Exception e) when (!Environment.IsDebugBuild)
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError && e.Response != null)
                    {
                        var resp = (HttpWebResponse)e.Response;
                        switch (resp.StatusCode)
                        {
                            case HttpStatusCode.ServiceUnavailable: // HTTP 503
                                logger.Error(e, $"SuccessStory - HTTP 503 to load from {url}");
                                break;
                            default:
                                logger.Error(e, $"SuccessStory - Failed to load from {url}");
                                //PlayniteApi.Dialogs.ShowErrorMessage(e.Message, "SuccessStory error on GogAchievements");
                                AchievementsDatabase.ListErrors.Add("Error on GogAchievements: " + e.Message);
                                break;
                        }
                    }
                }

                    //webClient.Dispose();
                //}

                if (ResultWeb != "")
                {
                    JObject resultObj = JObject.Parse(ResultWeb);
                    try
                    {
                        JArray resultItems = (JArray)resultObj["items"];
                        if (resultItems.Count > 0)
                        {
                            HaveAchivements = true;

                            for (int i = 0; i < resultItems.Count; i++)
                            {
                                Achievements temp = new Achievements
                                {
                                    Name = (string)resultItems[i]["name"],
                                    Description = (string)resultItems[i]["description"],
                                    UrlUnlocked = (string)resultItems[i]["image_url_unlocked"],
                                    UrlLocked = (string)resultItems[i]["image_url_locked"],
                                    DateUnlocked = ((string)resultItems[i]["date_unlocked"] == null) ? default(DateTime) : (DateTime)resultItems[i]["date_unlocked"]
                                };

                                Total += 1;
                                if ((string)resultItems[i]["date_unlocked"] == null)
                                    Locked += 1;
                                else
                                    Unlocked += 1;

                                Achievements.Add(temp);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e, $"SuccessStory - Failed to parse.");
                        //PlayniteApi.Dialogs.ShowErrorMessage(e.Message, "SuccessStory error on GogAchievements");
                        AchievementsDatabase.ListErrors.Add("Error on GogAchievements: " + e.Message);
                    }
                }
            }

            Result = new GameAchievements
            {
                Name = GameName,
                HaveAchivements = HaveAchivements,
                Total = Total,
                Unlocked = Unlocked,
                Locked = Locked,
                Progression = (Total != 0) ? (int)Math.Ceiling((double)(Unlocked * 100 / Total)) : 0,
                Achievements = Achievements
            };

            return Result;
        }
    }
}
