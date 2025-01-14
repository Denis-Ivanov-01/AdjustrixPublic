using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace Adjustment.Project
{
    internal class NetworkTypeData
    {
        public string NetworkType { get; set; }
    }

    public class ProjectFile
    {
        private List<Tuple<ProjectType, Type>> ProjectTypeClassMapping;

        protected const string fileExtension = ".adjx";

        public ProjectFile()
        {
            ProjectTypeClassMapping = new();
            ProjectTypeClassMapping.Add(new(ProjectType.Leveling, typeof(LevelingProject)));
        }

        public string AsText()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.Cyrillic, UnicodeRanges.BasicLatin)
            };
            string jsonStr = JsonSerializer.Serialize(this, options);
            return Regex.Unescape(jsonStr);
        }

        public byte[] AsBytes()
        {
            return Encoding.UTF8.GetBytes(AsText());
        }

        public void ToFile(AdjustrixProject project)
        {
            string projName = $"{project.Name}{fileExtension}";
            string path = Path.Combine(project.ProjectFolder, projName);
            byte[] bytes = this.AsBytes();
            using (var fileStream = new FileStream(path, FileMode.Create))
            using (var zipStream = new GZipStream(fileStream, CompressionMode.Compress))
            {
                zipStream.Write(bytes, 0, bytes.Length);
            }
        }

        public AdjustrixProject FromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("The specified file does not exist.", path);

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            using (var memoryStream = new MemoryStream())
            {
                gzipStream.CopyTo(memoryStream);

                byte[] decompressedBytes = memoryStream.ToArray();
                string decompressed = Encoding.UTF8.GetString(decompressedBytes);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                return Deserialize(decompressed);
            }
        }

        private AdjustrixProject Deserialize(string projectString)
        {
            NetworkTypeData type = JsonSerializer.Deserialize<NetworkTypeData>(projectString)!;
            ProjectType projectType = (ProjectType)Enum.Parse(typeof(ProjectType), type.NetworkType, true);

            switch (projectType)
            {
                case ProjectType.Leveling:
                    return JsonSerializer.Deserialize<LevelingProject>(projectString)!;
                default:
                    throw new ArgumentException("Invalid type string!");
            }
        }
    }
}
