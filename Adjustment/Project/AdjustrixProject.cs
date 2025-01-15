using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Adjustment.Project
{

    public enum ProjectType
    {
        Leveling
    }

    public abstract class AdjustrixProject
    {
        public abstract string NetworkType { get; }

        //todo: this shouldn't be a part of the project.
        //If the project is moved to a different folder, this will be invalid!
        //public string ProjectFolder { get; set; }

        //public ProjectType ProjectType { get; set; }

        public string Name { get; set; }

        public string SiteName { get; set; }

        public string Contractor { get; set; }

        public string Client { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateDate { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime LastUpdateDate { get; set; }

        public AdjustrixProject(
            //string ProjectFolder,
            string Name, 
            string SiteName, 
            string Contractor,
            string Client,
            DateTime? CreateDate = null,
            DateTime? LastUpdateDate = null
            )
        {
            //this.ProjectFolder = ProjectFolder;
            this.Name = Name;
            this.SiteName = SiteName;
            this.Contractor = Contractor;
            this.Client = Client;
            this.CreateDate = CreateDate == null ? DateTime.Now : (DateTime)CreateDate;
            this.LastUpdateDate = LastUpdateDate == null ? DateTime.Now : (DateTime)LastUpdateDate;
        }
    }
}
