using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAMM_APIs.Models
{
    public class CancelAuth
    {
        public LicenseInfo LicenseInfo { get; set; }
        public string authorizationNumber { get; set; }
    }
}