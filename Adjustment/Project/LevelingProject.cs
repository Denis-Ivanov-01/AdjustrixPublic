using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text;
using System.IO.Compression;
using System.Text.Json.Serialization;

namespace Adjustment.Project
{
    public class LevelingProject : AdjustrixProject
    {
        private const ProjectType projectType = ProjectType.Leveling;

        public override string NetworkType
        {
            get
            {
                return projectType.ToString();
            }
        }

        public List<HeightDifference> HeightDifferences { get; set; }

        //private Dictionary<string, PointBase> benchmarksDict;

        [JsonConstructor]
        public LevelingProject(
            List<HeightDifference> HeightDifferences,
            //string ProjectFolder,
            string Name,
            string SiteName,
            string Contractor,
            string Client
            //ProjectType ProjectType = ProjectType.Leveling,
            //DateTime? CreateDate = null,
            //DateTime? LastUpdateDate = null
            )
            : base(Name, SiteName, Contractor, Client/*, CreateDate, LastUpdateDate*/)
        {
            //if (ProjectType != ProjectType.Leveling) { throw new ArgumentException("Incorrect project type!"); }
            this.HeightDifferences = HeightDifferences;
        }
    }
}
