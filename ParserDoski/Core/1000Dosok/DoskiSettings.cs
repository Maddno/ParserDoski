
namespace ParserDoski.Core._1000Dosok
{
    internal class DoskiSettings : IParserSettings
    {
        public DoskiSettings(int start, int end, string category, string linksPath, string dataPath)
        {
            StartPoint = start;
            EndPoint = end;
            Prefix = category;
            LinksPathSave = linksPath;
            DataPathSave = dataPath;
        }

        public string BaseUrl { get; set; } = "https://1000dosok.ru";
        public string Prefix { get; set; } 
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public string LinkUrl { get; set; } = "{LinkId}";
        public string LinksPathSave { get; set; }
        public string DataPathSave { get; set;}
    }
    //{CurrentId}
}
