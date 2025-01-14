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
        public List<HeightDifference> HeightDifferences { get; set; }

        private Dictionary<string, PointBase> benchmarksDict;

        [JsonConstructor]
        public LevelingProject(
            List<HeightDifference> HeightDifferences,
            string ProjectFolder,
            string Name,
            string SiteName,
            string Contractor,
            string Client
            //ProjectType ProjectType = ProjectType.Leveling,
            //DateTime? CreateDate = null,
            //DateTime? LastUpdateDate = null
            )
            : base(ProjectFolder, Name, SiteName, Contractor, Client/*, CreateDate, LastUpdateDate*/)
        {
            //if (ProjectType != ProjectType.Leveling) { throw new ArgumentException("Incorrect project type!"); }
            this.HeightDifferences = HeightDifferences;
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

        public void ToFile()
        {
            string projName = $"{Name}{fileExtension}";
            string path = Path.Combine(ProjectFolder, projName);
            byte[] bytes = this.AsBytes();
            using (var fileStream = new FileStream(path, FileMode.Create))
            using (var zipStream = new GZipStream(fileStream, CompressionMode.Compress))
            {
                zipStream.Write(bytes, 0, bytes.Length);
            }
        }

        public static LevelingProject FromFile(string path)
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
                return JsonSerializer.Deserialize<LevelingProject>(decompressed, options)!;
            }
        }
    }
}
