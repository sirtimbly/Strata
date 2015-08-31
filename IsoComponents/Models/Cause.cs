using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IsoComponents.Models
{
    public class AdvocateCampaignCause
    {
        private List<string> _descriptions = new List<string>();
        private List<string> _headlines = new List<string>();
        private Dictionary<string, string> _coverPhotos = new Dictionary<string, string>();
        public string TcmId { get; set; }
        public string Name { get; set; }
        public string FundId { get; set; }
        public string Logo { get; set; }
        public string CompletedFundraiserText { get; set; }
        public string CompletedFundraiserLink { get; set; }

        public List<string> Descriptions
        {
            get
            {
                return _descriptions;
            }
            set
            {
                _descriptions = value;
            }
        }
        public List<string> Headlines
        {
            get
            {
                return _headlines;
            }
            set
            {
                _headlines = value;
            }
        }


        public string EmailThanksComplete { get; set; }
        public string EmailThanksCompleteSubject { get; set; }
        public string Id { get; set; }
        public string EmailShareSpread { get; set; }
        public string EmailShareSpreadSubject { get; set; }
        public string EmailShareUpdate { get; set; }
        public string EmailShareUpdateSubject { get; set; }
        public string SocialFacebook { get; set; }
        public string SocialTwitter { get; set; }

        public List<AdvocateCampaignCauseSocial> Social { get; set; }

        public AdvocateCampaignCause()
        {
            Social = new List<AdvocateCampaignCauseSocial>();
        }

        public string Video { get; set; }
        public string FooterImage { get; set; }

        public List<ActTheme> Themes { get; set; }
        public List<ActCategory> Categories { get; set; }

        public List<SlideshowImage> CoverPhotos { get; set; }
        public string CompassCampaignId { get; set; }
    }



    public class AdvocateCampaignCauseSocial
    {
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Used for Facebook, Google+ or other service that allows long text
        /// </summary>
        public string Long { get; set; }
        /// <summary>
        /// Used for Twitter or other service with a limited text length
        /// </summary>
        public string Short { get; set; }
    }

    public static class MessageTypes
    {
        public static string Share = "share";
        public static string Spread = "share";
        public static string Thanks = "thanks";
        public static string Update = "update";
    }

    public class ActTheme
    {
        public string tcmId { get; set; }
        public string shortName { get; set; }
        public string description { get; set; }
        public string video { get; set; }
        public string portrait { get; set; }
        public List<SlideshowImage> coverPhotos { get; set; }

    }
    public class ActCategory
    {
        public string tcmId { get; set; }
        public string name { get; set; }
        public string goal { get; set; }
        public string donation { get; set; }
        public bool requireEventDate { get; set; }
        public bool showLocationFields { get; set; }
        public string eventDateLabel { get; set; }
        public List<SlideshowImage> coverPhotos { get; set; }
        //TODO: add email and social sharing text fields
    }

    public class SlideshowImage
    {
        public string title { get; set; }
        public string image { get; set; }
        public string caption { get; set; }
        public string altText { get; set; }
    }
}