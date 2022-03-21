using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAMM_APIs.Models
{
    public class IssueAuth
    {
        public LicenseInfo LicenseInfo { get; set; }
        public string fromHijri { get; set; }
        public string toHijri { get; set; }
        public string authenticationCode { get; set; }
        public string correlationId { get; set; }
    }
}