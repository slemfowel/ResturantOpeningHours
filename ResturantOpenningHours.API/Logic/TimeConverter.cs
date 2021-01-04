using ResturantOpenningHours.Model.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResturantOpenningHours.API.Logic
{
    public class TimeConverter
    {
        /// <summary>  
        /// this action get the UTC time from the unix time 
        /// </summary>  
        public static string UnixTimeStampToShortTimeString(double unixTimeStamp)
        {
            if (unixTimeStamp == 0)
            {
                return "(*)";
            }
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime.ToShortTimeString();
        }

        public OpenningAndClosingHoursResponse Converter(OpenningAndClosingHoursReqest list)
        {

            return new OpenningAndClosingHoursResponse
            {
                Sunday = string.Format("Sunday: {0}", PrintTime(TimeSorteer(list.Sunday))),
                Monday = string.Format("Monday: {0}", PrintTime(TimeSorteer(list.Monday))),
                Tuesday = string.Format("Tuesday: {0}", PrintTime(TimeSorteer(list.Tuesday))),
                Wednesday = string.Format("Wednesday: {0}", PrintTime(TimeSorteer(list.Wednesday))),
                Thursday = string.Format("Thursday: {0}", PrintTime(TimeSorteer(list.Thursday))),
                Friday = string.Format("Friday: {0}", PrintTime(TimeSorteer(list.Friday))),
                Saturday = string.Format("Saturday: {0}", PrintTime(TimeSorteer(list.Saturday))),

            };


        }



        public List<Time> TimeSorteer(List<OpenHourModel> openHourModel)
        {
            var times = new List<Time>();
            if (openHourModel == null) return times;

            //var hours = openHourModel.OrderBy(x => x.value);
            var allitems = openHourModel.OrderBy(x => x.Value).ToList();
            var openhours = openHourModel.OrderBy(x => x.Type).Where(x => x.Type.ToLower() == "open").ToList();
            var closehours = openHourModel.OrderBy(x => x.Type).Where(x => x.Type.ToLower() == "close").ToList();

            //if ((openhours.Count > closehours.Count))
            //{
            //    var time = new Time
            //    {
            //        OpenTime = openhours.LastOrDefault().Value,
            //        CloseTime = 0
            //    };
            //    times.Add(time);

            //}
            if (allitems.Count < 1)
            {
                return times;
            }
            if (allitems.LastOrDefault().Type.ToLower() == "open")
            {
                var time = new Time
                {
                    OpenTime = allitems.LastOrDefault().Value,
                    CloseTime = 0
                };
                times.Add(time);
            }
            if (allitems.FirstOrDefault().Type.ToLower() == "close")
            {
                var time = new Time
                {
                    OpenTime =0,
                    CloseTime = allitems.FirstOrDefault().Value
                };
                times.Add(time);
            }
            
            if(openhours.Count >= 1 && closehours.Count >= 1)
            {
                
                foreach (var openitemhour in openhours)
                {

                    foreach (var closeitemhour in closehours)
                    {
                        if (openitemhour.Value < closeitemhour.Value)
                        {
                            var time = new Time
                            {
                                OpenTime = openitemhour.Value,
                                CloseTime = closeitemhour.Value
                            };
                            times.Add(time);


                             closehours.Remove(closeitemhour);
                            break;
                            // openhours.Remove(openitemhour);
                        }
                        
                       
                    }
                }

            }
            
            return times;
        }
   
        public string PrintTime(List<Time> timer)
        {
            
            if (timer == null || timer.Count < 1) return "Closed";
            if (timer.Count == 1) return string.Format("{0} - {1}", UnixTimeStampToShortTimeString(timer.First().OpenTime), UnixTimeStampToShortTimeString(timer.First().CloseTime));
            else
            {
                var value = "";
                foreach (var item in timer)
                {
                  value =  string.Format("{0} - {1},", UnixTimeStampToShortTimeString(item.OpenTime), UnixTimeStampToShortTimeString(item.CloseTime));
                }
                return value;
               
            }
        }
    }

    public class Time
    {
        public int OpenTime { get; set; }
        public int CloseTime { get; set; }
    }
}
