using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTestBot.Models
{
    public class WeatherData
    {
        [JsonProperty(PropertyName = "HeWeather5")]
        public Heweather5[] HeWeather5 { get; set; }
    }

    public class Heweather5
    {
        public Aqi aqi { get; set; }
        public Basic basic { get; set; }
        public Daily_Forcast[] daily_forcast { get; set; }
        public Now now { get; set; }
        public Suggestion suggestion { get; set; }
    }
    public class Aqi
    {
        public City city { get; set; }
    }
    public class City
    {
        public string aqi { get; set; }
        public string pm25 { get; set; }
    }
    public class Basic
    {
        public string city { get; set; }
    }
    public class Daily_Forcast
    {
        public string data { get; set; }
        public Cond1 cond { get; set; }
    }
    public class Cond1
    {
        public string txt_d { get; set; }
        public string txt_n { get; set; }
    }
    public class Now
    {
        public Cond2 cond { get; set; }
        public string tmp { get; set; }
    }
    public class Cond2
    {
        public string txt { get; set; }
    }
    public class Suggestion
    {
        public Comf comf { get; set; }
        public Drsg drsg { get; set; }
        public Sport sport { get; set;}
    }
    public class Comf
    {
        public string brf { get; set; }
        public string txt { get; set; }
    }
    public class Drsg
    {
        public string brf { get; set; }
        public string txt { get; set; }
    }
    public class Sport
    {
        public string brf { get; set; }
        public string txt { get; set; }
    }

}