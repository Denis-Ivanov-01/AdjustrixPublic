namespace AdjustrixBase.Project
{
    public class ProjectGeneralProperties : IMemento<ProjectGeneralProperties>
    {
        public string ProjectName { get; set; }

        public string SiteName { get; set; }

        public string Contractor { get; set; }

        public string Client { get; set; }

        public ProjectGeneralProperties(string name, string site, string contractor, string client)
        {
            ProjectName = name;
            SiteName = site;
            Contractor = contractor;
            Client = client;
        }

        public ProjectGeneralProperties()
        {

        }

        public ProjectGeneralProperties GetState()
        {
            return new(ProjectName, SiteName, Contractor, Client);
        }
    }
}
