﻿using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PluginCommon;

namespace SuccessStory.Models
{
    public class Achievements
    {
        public string Name { get; set; }
        public string ApiName { get; set; } = string.Empty;
        public string Description { get; set; }
        public string UrlUnlocked { get; set; }
        public string UrlLocked { get; set; }
        public DateTime? DateUnlocked { get; set; }
        public bool IsHidden { get; set; } = false;
        public float Percent { get; set; } = 100;

        [JsonIgnore]
        public string CacheUnlocked {
            get
            {
                string ImageFileName = string.Empty;

                if (!UrlUnlocked.IsNullOrEmpty()) {
                    ImageFileName = GetNameFromUrl(UrlUnlocked);
                    ImageFileName = string.Concat(ImageFileName.Split(Path.GetInvalidFileNameChars()));
                    ImageFileName += "_Unlocked";
                }

                return ImageFileName;
            }
        }
        [JsonIgnore]
        public string ImageUnlocked
        {
            get
            {
                string pathImageUnlocked = PlayniteTools.GetCacheFile(CacheUnlocked, "SuccessStory");
                if (pathImageUnlocked.IsNullOrEmpty())
                {
                    pathImageUnlocked = UrlUnlocked;
                }
                return pathImageUnlocked;
            }
        }

        [JsonIgnore]
        public string CacheLocked
        {
            get
            {
                string ImageFileName = string.Empty;

                if (!UrlLocked.IsNullOrEmpty())
                {
                    ImageFileName = GetNameFromUrl(UrlLocked);
                    ImageFileName = string.Concat(ImageFileName.Split(Path.GetInvalidFileNameChars()));
                    ImageFileName += "_Locked";
                }

                return ImageFileName;
            }
        }
        [JsonIgnore]
        public string ImageLocked
        {
            get
            {
                if (!UrlLocked.IsNullOrEmpty() && UrlLocked != UrlUnlocked)
                {
                    string pathImageLocked = PlayniteTools.GetCacheFile(CacheLocked, "SuccessStory");
                    if (pathImageLocked.IsNullOrEmpty())
                    {
                        pathImageLocked = UrlLocked;
                    }
                    return pathImageLocked;
                }
                else
                {
                    string pathImageLocked = PlayniteTools.GetCacheFile(CacheUnlocked, "SuccessStory");
                    if (pathImageLocked.IsNullOrEmpty())
                    {
                        pathImageLocked = UrlUnlocked;
                    }
                    return pathImageLocked;
                }
            }
        }


        private string GetNameFromUrl(string url)
        {
            string NameFromUrl = string.Empty;
            List<string> urlSplited = url.Split('/').ToList();

            if (url.IndexOf(".xboxlive.com") > -1)
            {
                NameFromUrl = "xbox_" + Name.Replace(" ", "");
            }

            if (url.IndexOf("steamcommunity") > -1)
            {                
                NameFromUrl = "steam_" + ApiName;
                if (urlSplited.Count >= 8)
                {
                    NameFromUrl += "_" + urlSplited[7];
                }
            }

            if (url.IndexOf(".gog.com") > -1)
            {
                NameFromUrl = "gog_" + ApiName;
            }

            if (url.IndexOf(".ea.com") > -1)
            {
                NameFromUrl = "ea_" + Name.Replace(" ", "");
            }

            return NameFromUrl;
        }
    }
}
