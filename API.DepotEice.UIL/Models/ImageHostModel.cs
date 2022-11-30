namespace API.DepotEice.UIL.Models
{
    public class Image
    {
        public string name { get; set; }
        public string extension { get; set; }
        public int size { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string date { get; set; }
        public string date_gmt { get; set; }
        public object storage_id { get; set; }
        public object description { get; set; }
        public string nsfw { get; set; }
        public string md5 { get; set; }
        public string storage { get; set; }
        public string original_filename { get; set; }
        public object original_exifdata { get; set; }
        public string views { get; set; }
        public string id_encoded { get; set; }
        public string filename { get; set; }
        public double ratio { get; set; }
        public string size_formatted { get; set; }
        public string mime { get; set; }
        public int bits { get; set; }
        public object channels { get; set; }
        public string url { get; set; }
        public string url_viewer { get; set; }
        public Thumb thumb { get; set; }
        public Medium medium { get; set; }
        public string views_label { get; set; }
        public string display_url { get; set; }
        public string how_long_ago { get; set; }
    }

    public class Medium
    {
        public string filename { get; set; }
        public string name { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public double ratio { get; set; }
        public int size { get; set; }
        public string size_formatted { get; set; }
        public string mime { get; set; }
        public string extension { get; set; }
        public int bits { get; set; }
        public object channels { get; set; }
        public string url { get; set; }
    }

    public class Root
    {
        public int status_code { get; set; }
        public Success success { get; set; }
        public Image image { get; set; }
        public string status_txt { get; set; }
    }

    public class Success
    {
        public string message { get; set; }
        public int code { get; set; }
    }

    public class Thumb
    {
        public string filename { get; set; }
        public string name { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int ratio { get; set; }
        public int size { get; set; }
        public string size_formatted { get; set; }
        public string mime { get; set; }
        public string extension { get; set; }
        public int bits { get; set; }
        public object channels { get; set; }
        public string url { get; set; }
    }

}
