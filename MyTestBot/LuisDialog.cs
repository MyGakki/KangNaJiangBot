using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using MyTestBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyTestBot
{
    [LuisModel("436dfd08-0ee3-48a7-ac40-0fab3020ab3d", "f98a0b7e95e44d238ca56892571e19d1")]
    [Serializable]
    public class LuisDialog:LuisDialog<object>
    {
        [LuisIntent("打招呼")]
        public async Task Greeting(IDialogContext context,LuisResult result)
        {
            await context.PostAsync("你好啊，老铁");
            context.Wait(MessageReceived);
        }
        [LuisIntent("询问名字")]
        public async Task Name(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("我是康娜酱");
            context.Wait(MessageReceived);
        }
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("你在说啥，我不懂哎");
            context.Wait(MessageReceived);
        }
        public bool TryToFindLocation(LuisResult result, out String location)
        {
            location = "";
            EntityRecommendation title;
            if (result.TryFindEntity("地点", out title))
            {
                location = title.Entity;
            }
            else
            {
                location = "";
            }
            return !location.Equals("");
        }
        [LuisIntent("查询天气")]
        
        public async Task QueryWeather(IDialogContext context,LuisResult result)
        {
            string location = "";
            string replyString = "";
            if (TryToFindLocation(result, out location))
            {
                replyString = await GetWeather(location);
                await context.PostAsync(replyString);
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("你要查询哪里的天气呢？");
                context.Wait(AfterEnterLocation);

            }
        }

        private async Task AfterEnterLocation(IDialogContext context, IAwaitable<IMessageActivity> arguement)
        {
            var message = await arguement;
            string replyString = await GetWeather(message.Text);
            await context.PostAsync(replyString);
            context.Wait(MessageReceived);
        }

        private async Task<string> GetWeather(string cityname)
        {
            WeatherData weatherdata = await GetWeatherDataAsync(cityname);
            if (weatherdata == null || weatherdata.HeWeather5 == null)
            {
                return string.Format("无法获取\"{0}\"的天气信息",cityname);
            }
            else
            {
                Heweather5[] weatherServices = weatherdata.HeWeather5;
                if (weatherServices.Length <= 0)
                {
                    return string.Format("无法获取\"{0}\"的天气信息", cityname);
                }
                Basic cityInfo = weatherServices[0].basic;
                if (cityInfo == null)
                {
                    return string.Format("\"{0}\"应该不是城市的名字", cityname);
                }
                string cityInfoString = "城市：" + cityInfo.city + "\r\n";
                Now cityNowInfo = weatherServices[0].now;
                string cityNowInfoString = "现在天气：" + cityNowInfo.cond.txt + "\r\n"
                    + "现在温度：" + cityNowInfo.tmp;
                Suggestion citySuggestionInfo = weatherServices[0].suggestion;
                string citySuggestionInfoString = "舒适指数：" + citySuggestionInfo.comf.brf + "\r\n"
                    + "穿衣建议：" + citySuggestionInfo.drsg.txt + "\r\n"
                    + "运动建议：" + citySuggestionInfo.sport.txt + "\r\n";
                Aqi cityAqiInfo = weatherServices[0].aqi;
                string cityAqiInfoString = "空气质量指数：" + cityAqiInfo.city.aqi + "\r\n"
                    + "PM2.5:" + cityAqiInfo.city.pm25;
                return string.Format("现在{0}的天气：\r\n{1}", cityname, cityInfoString + cityNowInfoString + cityAqiInfoString + citySuggestionInfoString);

            }
        }
        public static async Task<WeatherData> GetWeatherDataAsync(string city)
        {
            try
            {
                string serviceUrl = $"https://free-api.heweather.com/v5/weather?city=" + city + $"&key=04b349e102f9433a893e588da6f202c6";
                string ResultString;
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    ResultString = await client.DownloadStringTaskAsync(serviceUrl).ConfigureAwait(false);
                }
                WeatherData weatherData = (WeatherData)JsonConvert.DeserializeObject(ResultString, typeof(WeatherData));
                return weatherData;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}