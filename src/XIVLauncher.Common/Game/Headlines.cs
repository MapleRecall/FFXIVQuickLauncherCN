﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Serilog;
using XIVLauncher.Common.Util;

namespace XIVLauncher.Common.Game
{
    public partial class Headlines
    {
        [JsonProperty("news")]
        public News[] News { get; set; }

        [JsonProperty("topics")]
        public News[] Topics { get; set; }

        [JsonProperty("pinned")]
        public News[] Pinned { get; set; }

        [JsonProperty("banner")]
        public Banner[] Banner { get; set; }
    }

    public class Banner
    {
        [JsonProperty("HomeImagePath")]
        public Uri LsbBanner { get; set; }

        [JsonProperty("OutLink")]
        public Uri Link { get; set; }
    }

    public class News
    {
        [JsonProperty("PublishDate")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Author")]
        public string Url { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("tag", NullValueHandling = NullValueHandling.Ignore)]
        public string Tag { get; set; }
    }

    public class SdoBanner {
        public Banner[] Data { get; set; }
    }

    public class SdoNews
    {
        public News[] Data { get; set; }
    }

    public partial class Headlines
    {
        public static async Task<Headlines> Get(Launcher game, ClientLanguage language)
        {
            var headlines = new Headlines();
            headlines.Banner = await GetBanner(game);
            headlines.News = await GetNews(game);
            return headlines;
        }

        private static async Task<Banner[]> GetBanner(Launcher game) {
            var json = Encoding.UTF8.GetString(await game.DownloadAsLauncher("https://ff.web.sdo.com/inc/newdata.ashx?url=List?gameCode=ff&category=5203&pageIndex=0&pageSize=6&callback=lundivFun", ClientLanguage.ChineseSimplified, "*/*").ConfigureAwait(false));
            json = json.Trim();
            json = json.Substring("lundivFun(".Length);
            json = json.Substring(0, json.Length - 1);
            //json = $"{{\"banner\":{json}}}";
            var sdoBanner = JsonConvert.DeserializeObject<SdoBanner>(json);
            return sdoBanner.Data;
        }

        private static async Task<News[]> GetNews(Launcher game)
        {
            var json = Encoding.UTF8.GetString(await game.DownloadAsLauncher("https://ff.web.sdo.com/inc/newdata.ashx?url=List?gameCode=ff&category=5310,5311,5312,5313,5316&pageIndex=0&pageSize=12&callback=boxscrollFun", ClientLanguage.ChineseSimplified, "*/*").ConfigureAwait(false));
            json = json.Trim();
            json = json.Substring("boxscrollFun(".Length);
            json = json.Substring(0, json.Length - 1);
            //json = $"{{\"banner\":{json}}}";
            var sdoNews = JsonConvert.DeserializeObject<SdoNews>(json);
            return sdoNews.Data;
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings SETTINGS = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            }
        };
    }

}