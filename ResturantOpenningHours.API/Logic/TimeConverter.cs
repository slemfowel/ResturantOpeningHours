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
            //if (unixTimeStamp == 0)
            //{
            //    return "(*)";
            //}
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime.ToShortTimeString();
        }

        public OpenningAndClosingHoursResponse Converter(OpenningAndClosingHoursReqest list)
        {

            return new OpenningAndClosingHoursResponse
            {
                Sunday = string.Format("Sunday: {0}", PrintTime(TimeSorteer(list.Sunday,list.Saturday,list.Monday,"Saturday","Monday"))),
                Monday = string.Format("Monday: {0}", PrintTime(TimeSorteer(list.Monday, list.Sunday, list.Tuesday, "Sunday","Tuesday"))),
                Tuesday = string.Format("Tuesday: {0}", PrintTime(TimeSorteer(list.Tuesday, list.Monday, list.Wednesday, "Monday", "Wednesday"))),
                Wednesday = string.Format("Wednesday: {0}", PrintTime(TimeSorteer(list.Wednesday, list.Tuesday,list.Thursday, "Tuesday", "Thursday"))),
                Thursday = string.Format("Thursday: {0}", PrintTime(TimeSorteer(list.Thursday, list.Wednesday, list.Friday,"Wednesday","Friday"))),
                Friday = string.Format("Friday: {0}", PrintTime(TimeSorteer(list.Friday,list.Thursday,list.Saturday,"Thursday","Saturday"))),
                Saturday = string.Format("Saturday: {0}", PrintTime(TimeSorteer(list.Saturday,list.Friday,list.Sunday,"Friday","Sunday"))),

            };


        }



        public List<Time> TimeSorteer(List<OpenHourModel> openHourModel, List<OpenHourModel> previousDay, List<OpenHourModel> nextDay, string previousDayAsWord, string nextDayAsWord)
        {
            var times = new List<Time>();
            if (openHourModel == null) return times;

            //var hours = openHourModel.OrderBy(x => x.value);
            var allitems = openHourModel.OrderBy(x => x.Value).ToList();
            var openhours = openHourModel.OrderBy(x => x.Type).Where(x => x.Type.ToLower() == "open").ToList();
            var closehours = openHourModel.OrderBy(x => x.Type).Where(x => x.Type.ToLower() == "close").ToList();
            nextDay = nextDay.OrderBy(x => x.Value).ToList();
            previousDay = previousDay.OrderBy(x => x.Value).ToList();

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
            if (allitems.LastOrDefault().Type.ToLower() == "open" && (nextDay.Count > 0 && nextDay.FirstOrDefault().Type.ToLower() == "close"))
            {
                var time = new Time
                {
                    OpenTime = UnixTimeStampToShortTimeString(allitems.LastOrDefault().Value),
                    CloseTime = string.Format("{0} {1}",nextDayAsWord , UnixTimeStampToShortTimeString(nextDay.FirstOrDefault().Value))
                };
                times.Add(time);
            }
            if (allitems.FirstOrDefault().Type.ToLower() == "close" && (previousDay.Count > 0 && previousDay.LastOrDefault().Type.ToLower() == "open"))
            {
                var time = new Time
                {
                    OpenTime = string.Format("{0} {1}", previousDayAsWord  ,  UnixTimeStampToShortTimeString(previousDay.FirstOrDefault().Value)),
                    CloseTime = UnixTimeStampToShortTimeString(allitems.FirstOrDefault().Value)
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
                                OpenTime = UnixTimeStampToShortTimeString(openitemhour.Value),
                                CloseTime = UnixTimeStampToShortTimeString(closeitemhour.Value)
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
            if (timer.Count == 1) return string.Format("{0} - {1}", timer.First().OpenTime, timer.First().CloseTime);
            else
            {
                var value = "";
                foreach (var item in timer)
                {
                  value =  string.Format("{0} - {1},", item.OpenTime, item.CloseTime);
                }
                return value;
               
            }
        }
    }

    public class Time
    {
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
    }
}
