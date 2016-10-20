using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml.Serialization;

namespace TLDDesigns.Home
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap
    {
        private ArrayList map;

        public Sitemap()
        {
            map = new ArrayList();
        }

        [XmlElement("url")]
        public Location[] Locations
        {
            get
            {
                Location[] items = new Location[map.Count];
                map.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null)
                    return;
                Location[] items = (Location[])value;
                map.Clear();
                foreach (Location item in items)
                    map.Add(item);
            }
        }

        public int Add(Location item)
        {
            return map.Add(item);
        }
    }

    // Items in the shopping list
    public class Location
    {
        public enum eChangeFrequency
        {
            always,
            hourly,
            daily,
            weekly,
            monthly,
            yearly,
            never
        }

        [XmlElement("loc")]
        public string Url { get; set; }

        [XmlElement("changefreq")]
        public eChangeFrequency? ChangeFrequency { get; set; }
        public bool ShouldSerializeChangeFrequency() { return ChangeFrequency.HasValue; }

        [XmlElement("lastmod")]
        public DateTime? LastModified { get; set; }
        public bool ShouldSerializeLastModified() { return LastModified.HasValue; }

        [XmlElement("priority")]
        public double? Priority { get; set; }
        public bool ShouldSerializePriority() { return Priority.HasValue; }
    }
    public partial class sitemap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Sitemap sitemap = new Sitemap();

            string[] excludedPaths = new string[] { "assets", "css", "fonts", "js", "templates", "node_modules", "obj","App_Data","bin","imagecache","My Project","Properties",".well-known" };

            string pageName = "sitemap.aspx";

            string defaultDocument = @"\index.min.html";

            string baseUrl = HttpContext.Current.Request.Url.ToString().Replace(pageName, "");

            string physicalBasePath = Server.MapPath(".");

            FileInfo fileInfo;

            DateTime lastModified;

            //iterate through directories

            DirectoryInfo di = new DirectoryInfo(physicalBasePath);

            DirectoryInfo[] diArr = di.GetDirectories("*", SearchOption.AllDirectories);

            foreach (DirectoryInfo dri in diArr)
            {
                string fullPath = dri.FullName;
                string relativePath = fullPath.Replace(physicalBasePath, "");

                if (relativePath.Substring(0, 1) == @"\")
                {
                    relativePath = relativePath.Substring(1, (relativePath.Length - 1));
                }

                string relativeUrl = relativePath.Replace(@"\", "/");
                string absoluteUrl = baseUrl + relativeUrl;

                bool excluded = false;

                foreach (string ex in excludedPaths)
                {

                    if (relativePath.Length >= ex.Length)
                    {

                        if (ex.ToUpper() == relativePath.Substring(0, ex.Length).ToUpper())
                        //if (ex == relativePath.Substring(0, ex.Length))
                        {
                            excluded = true;

                        }
                        
                }
            }

                if (excluded == false)
                {
                    fileInfo = new FileInfo(fullPath + defaultDocument);

                    if (fileInfo.Exists)
                    {

                        lastModified = fileInfo.LastWriteTimeUtc;

                        sitemap.Add(new Location()
                        {
                            Url = absoluteUrl,
                            LastModified = lastModified
                            //LastModified = DateTime.UtcNow.AddDays(-1)
                        });
                    }
                }
            }

            fileInfo = new FileInfo(physicalBasePath + defaultDocument);

            lastModified = fileInfo.LastWriteTimeUtc;

            sitemap.Add(new Location()
            {
                Url = baseUrl,
                LastModified = lastModified
                //LastModified = DateTime.UtcNow.AddDays(-1)
            });

            //sitemap.Add(new Location()
            //{
            //    Url = physicalBasePath,
            //    LastModified = DateTime.UtcNow.AddDays(-1)
            //});

            //sitemap.Add(new Location()
            //{
            //    Url = "http://blog.mikecouturier.com/2011/07/create-zoomable-images-using-google.html"
            //});

            //sitemap.Add(new Location()
            //{
            //    Url = "http://blog.mikecouturier.com/p/sitemap.html",
            //    ChangeFrequency = Location.eChangeFrequency.daily,
            //    Priority = 0.8D
            //});

            // one more random example
            //for (int i = 0; i < 3; i++)
            //    sitemap.Add(new Location()
            //    {
            //        Url = "http://blog.mikecouturier.com/dynamic-url/" + i
            //    });

            // In MVC you would just return an XmlReturn, in WebForms
            // you can do this...
            Response.Clear();
            XmlSerializer xs = new XmlSerializer(typeof(Sitemap));
            Response.ContentType = "text/xml";
            xs.Serialize(Response.Output, sitemap);
            Response.End();
        }
    }
}