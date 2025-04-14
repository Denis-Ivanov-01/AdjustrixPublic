using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace AdjustrixBase.Project
{
    internal class NetworkTypeData
    {
        public string NetworkType { get; set; }
    }

    public class ProjectFileManager
    {
        private readonly List<Tuple<ProjectType, Type>> ProjectTypeClassMapping;
        private string lastProjectFolder;
        public string fileName;

        protected const string fileExtension = ".adjx";

        public string LastProjectFolder
        {
            get
            {
                return lastProjectFolder;
            }
            private set
            {
                lastProjectFolder = value;
            }
        }

        public ProjectFileManager()
        {
            ProjectTypeClassMapping = new();
            ProjectTypeClassMapping.Add(new(ProjectType.Leveling, typeof(LevelingProject)));
        }

        public string AsText(AdjustrixProject project)
        {

            string jsonStr = Serialize(project);
            return Regex.Unescape(jsonStr).Replace("\\", "\\\\");
        }

        public byte[] AsBytes(AdjustrixProject project)
        {
            return Encoding.UTF8.GetBytes(AsText(project));
        }

        public void ToFile(AdjustrixProject project, bool projectIsNew, string projectFolder = "")
        {
            if (projectFolder == string.Empty)
            {//If no project folder is specified, we take the project folder of the last project file

                if (string.IsNullOrWhiteSpace(LastProjectFolder)) { throw new ArgumentNullException("An empty project folder path was passed!"); }

                projectFolder = LastProjectFolder;
            }
            if (projectIsNew)
            {
                fileName = project.Name;
            }
            string fullName = $"{fileName}{fileExtension}";
            string path = Path.Combine(projectFolder, fullName);
            byte[] bytes = AsBytes(project);
            using (var fileStream = new FileStream(path, FileMode.Create))
            using (var zipStream = new GZipStream(fileStream, CompressionMode.Compress))
            {
                zipStream.Write(bytes, 0, bytes.Length);
            }
            LastProjectFolder = projectFolder;
        }

        public AdjustrixProject FromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The specified file does not exist.", path);
            }

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            using (var memoryStream = new MemoryStream())
            {
                gzipStream.CopyTo(memoryStream);

                byte[] decompressedBytes = memoryStream.ToArray();
                string decompressed = Encoding.UTF8.GetString(decompressedBytes);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = false
                };

                LastProjectFolder = Path.GetDirectoryName(path)!;
                fileName = Path.GetFileNameWithoutExtension(path);

                AdjustrixProject project = Deserialize(decompressed, options);

                return Deserialize(decompressed, options);
            }
        }

        private AdjustrixProject Deserialize(string projectString, JsonSerializerOptions options)
        {
            NetworkTypeData type = JsonSerializer.Deserialize<NetworkTypeData>(projectString)!;
            ProjectType projectType = (ProjectType)Enum.Parse(typeof(ProjectType), type.NetworkType, true);

            switch (projectType)
            {
                case ProjectType.Leveling:
                    return JsonSerializer.Deserialize<LevelingProject>(projectString, options)!;
                default:
                    throw new ArgumentException("Invalid type string!");
            }
        }

        private string Serialize(AdjustrixProject project)
        {
            ProjectType projectType = (ProjectType)Enum.Parse(typeof(ProjectType), project.NetworkType, true);
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.Cyrillic, UnicodeRanges.BasicLatin)
            };
            switch (projectType)
            {
                case ProjectType.Leveling:
                    return JsonSerializer.Serialize<LevelingProject>((LevelingProject)project, options);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
