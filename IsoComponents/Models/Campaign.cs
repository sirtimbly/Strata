using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace IsoComponents.Models
{
    public class Campaign
    {

    }

    /// <summary>
    /// Entity to hold information about the user created Advocates Campaign
    /// </summary>
    public class AdvocateCampaign
    {
        /// <summary>
        /// 
        /// </summary>
        public AdvocateCampaign()
        {
            Title = string.Empty;
            Goal = 0;
            Url = string.Empty;
        }

        /// <summary>
        /// The unique id for the campaign from the database
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// The user entered title of the campaign
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// User entered Team Name
        /// </summary>
        public string TeamName { get; set; }
        /// <summary>
        /// User selected or entered type of team?
        /// </summary>
        public string TeamType { get; set; }
        /// <summary>
        /// The goal of the campaign of either dollars or sponsorships
        /// </summary>
        public int Goal { get; set; }
        /// <summary>
        /// The vanity url the user selects or is guided to
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// The selected cause that the campaign is focused on (Water for Life, Bite Back, ....)
        /// NOT REQUIRED for Sponsorship campaigns
        /// 
        /// *** The cause will need to map to a Campaign Code that is needed for the Source code data updating - 6 characters (ie. CC2004, CD2005, DSP03)
        /// We may needd to have another field to track the Campaign Code seperate from the Fund Code for the cause if we need to track both pieces of data.
        /// </summary>
        public string Cause { get; set; }
        /// <summary>
        /// The product that is the focus of the event (Sponsorship, Donations, etc)
        /// </summary>
        public string ProductType { get; set; }
        /// <summary>
        /// The selected type of campaign (Birthday, Run, Church Event, Just Because...)
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// The user entered or selected headline for the campaign
        /// </summary>
        public string Headline { get; set; }
        /// <summary>
        /// The users entered or selected description for the campaign
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The url of the image that the user either uploaded or selected from a list
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// The path of the photo that the user uploaded for themselves
        /// </summary>
        public string ProfilePhoto { get; set; }
        /// <summary>
        /// The source code of the communication/campaign/etc that the user followed to bring them to this product
        /// </summary>
        public string ReferralSourceCode { get; set; }
        /// <summary>
        /// The Compass assigned source code for this specific campaign
        /// (note: Will be assigned only when the user "launches" the campaign)
        /// </summary>
        public string SourceCode { get; set; }
        /// <summary>
        /// User set date of any "event" that ties to the campaign (ie. Birthday, Run Event, etc)
        /// </summary>
        public DateTimeOffset? EventDate { get; set; }
        /// <summary>
        /// Determines if the EventDate field has a real time value entered or not.
        /// </summary>
        /// <remarks>
        /// Currently, we are just using midnight + 1 second in GMT as a magic value to say no. Eventually this might
        /// become a bit field in the database
        /// </remarks>
        public bool HasEventTimeValue
        {
            get
            {
                if (EventDate.HasValue)
                {
                    return !(EventDate.Value.Offset.Equals(new TimeSpan(0))
                        && EventDate.Value.Hour.Equals(0)
                        && EventDate.Value.Minute.Equals(0)
                        && EventDate.Value.Second.Equals(1));
                }
                return false;
            }
        }

        /// <summary>
        /// The name of the event's location
        /// </summary>
        public string EventLocation { get; set; }
        /// <summary>
        /// Event Address line 1
        /// </summary>
        public string EventAddress { get; set; }
        /// <summary>
        /// Event Address line 2
        /// </summary>
        public string EventAddress2 { get; set; }
        /// <summary>
        /// City for the event's address
        /// </summary>
        public string EventCity { get; set; }
        /// <summary>
        /// State for the event's address
        /// </summary>
        public string EventState { get; set; }
        /// <summary>
        /// Zip/Postal code for the event's address
        /// </summary>
        public string EventZip { get; set; }
        /// <summary>
        /// Number of guests expected
        /// </summary>
        public int? ExpectedAttendees { get; set; }
        /// <summary>
        /// The ID of the Content Theme
        /// </summary>
        public string ThemeContentId { get; set; }
        /// <summary>
        /// User set date that the campaign starts
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// User set date that the campaign ends
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// User set date that the campaign ends (display version)
        /// </summary>
        public String DisplayEndDate
        {
            get
            {
                if (EndDate.HasValue)
                {
                    return Convert.ToDateTime(EndDate).ToString("MMMM d, yyyy");
                }
                else
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// The date is used to pre-populate the calendar widget
        /// </summary>
        public String CalendarEndDate
        {
            get
            {
                if (EndDate.HasValue)
                {
                    return Convert.ToDateTime(EndDate).ToString("dd-MM-yyyy");
                }
                else
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// Is the Campaign currently active
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Date that the Campaign was initially launched by the user
        /// </summary>
        public DateTime? DateLaunched { get; set; }
        /// <summary>
        /// Date that the Campaign was saved to the database
        /// </summary>
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// An identifier that is specific to an advocate in the Compass/ARC system.  (ie. V10, AV1000...)
        /// </summary>
        public string AdvocateCode { get; set; }
        /// <summary>
        /// Constituent Id
        /// </summary>
        public int? ConId { get; set; }
        /// <summary>
        /// Constituent Db Id
        /// </summary>
        public int? ConDbId { get; set; }
        /// <summary>
        /// Guid that uniquely identifies this Campagin and is tied to a user's cookie
        /// </summary>
        public Guid Token { get; set; }
        ///<summary>
        /// The Owner's Name
        ///</summary>
        public string OwnerName { get; set; }
        ///<summary>
        /// The Owner's Email
        ///</summary>
        public string OwnerEmail { get; set; }
        /// <summary>
        /// Suspended Date
        /// </summary>
        public DateTime? LastStatusChgDateTime { get; set; }
        /// <summary>
        /// Suspended Date as a custom date display
        /// </summary>
        public string DisplayLastStatusChgDateTime
        {
            get
            {
                if (LastStatusChgDateTime.HasValue)
                {
                    return Convert.ToDateTime(LastStatusChgDateTime).ToString("MMMM d, yyyy");
                }
                else
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// Suspended Reason
        /// </summary>
        public string SuspendReason { get; set; }
        /// <summary>
        /// Donations as an integer amount
        /// </summary>
        public int Donations { get; set; }
        /// <summary>
        /// DaysLeft calculated from EndDate to Now and conditionally has string return values
        /// </summary>
        public int? DaysLeft
        {
            get
            {
                if (EndDate.HasValue)
                {
                    return Convert.ToInt32((Convert.ToDateTime(EndDate) - DateTime.Now).TotalDays);
                }
                else
                {
                    return null;
                }
            }
        }

        public AdvocateCampaignCause CauseData { get; set; }


        /// <summary>
        /// Total amount of donations made
        /// </summary>
        public string Total { get; set; }

        /// <summary>
        /// Unique donors on the campaign
        /// </summary>
        public int DonorCount { get; set; }

        /// <summary>
        /// Number of donations made to the campaign
        /// </summary>
        public int DonationCount { get; set; }

    }

    public static class AdvocateCampaignTypes
    {
        public static string Donation = "donation";
        public static string Sponsorship = "sponsorship";
    }

    public static class AdvocateCampaignStatuses
    {
        public static string Created = "Created";
        public static string Active = "Active";
        public static string Completed = "Completed";
        public static string Suspended = "Suspended";
        public static string Request_Relaunch = "Suspended Relaunch";

        /// <summary>
        /// Returns a dictionary used to order a list of campaigns by a specific status list.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> ListOrder()
        {
            return new string[] { Suspended, Request_Relaunch, Created, Active, Completed }.Select((x, i) => new { x, i }).ToDictionary(z => z.x, z => z.i);
        }
    }

}