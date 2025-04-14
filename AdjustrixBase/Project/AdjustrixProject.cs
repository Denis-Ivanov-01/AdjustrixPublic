using System.Text.Json.Serialization;

namespace AdjustrixBase.Project
{

    public enum ProjectType
    {
        Leveling
    }

    public abstract class AdjustrixProject
    {
        public abstract string NetworkType { get; }

        public string Name { get; set; }

        public string SiteName { get; set; }

        public string Contractor { get; set; }

        public string Client { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateDate { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime LastUpdateDate { get; set; }

        public AdjustrixProject(
            string Name,
            string SiteName,
            string Contractor,
            string Client,
            DateTime? CreateDate = null,
            DateTime? LastUpdateDate = null
            )
        {
            this.Name = Name;
            this.SiteName = SiteName;
            this.Contractor = Contractor;
            this.Client = Client;
            this.CreateDate = CreateDate == null ? DateTime.Now : (DateTime)CreateDate;
            this.LastUpdateDate = LastUpdateDate == null ? DateTime.Now : (DateTime)LastUpdateDate;
        }

        public abstract void EnsureValidDataTypes();
    }
}
