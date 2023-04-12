using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.GlobalSettingsModels
{
    public class Location
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int LocationTypeId { get; set; }
        public string LocationTypeName { get; set; }
        public int LocationCategoryId { get; set; }
        public string LocationCategoryName { get; set; }
        public int LocationHeadId { get; set; }
        public string LocationHeadName { get; set; }
        public int LocationAltHeadId { get; set; }
        public string LocationAltHeadName { get; set; }
        public string LocationCountry { get; set; }
        public string LocationState { get; set; }
    }
}
