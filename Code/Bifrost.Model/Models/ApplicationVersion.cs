using System;

namespace Bifrost.Model.Models
{

    public class ApplicationVersion
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Revision { get; set; }
        public bool Public { get; set; }
        public bool Published { get; set; }
        public string PackageUri { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Md5 { get; set; }
        public string Filename { get; set; }


        public override string ToString()
        {
            return Major + "." + Minor + "." + Build + "." + Revision;
        }
    }

}