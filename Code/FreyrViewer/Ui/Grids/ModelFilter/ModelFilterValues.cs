using System;
using System.Collections.Generic;
using FreyrViewer.Models;

namespace FreyrViewer.Ui.Grids.ModelFilter
{
    public class ModelFilterValues
    {
        public DateTime DateFrom { get; set; } = DateTime.Today;
        public DateTime DateTo { get; set; } = DateTime.Today;
        //public List<string> TextFilters { get; set; } = new List<string>();
        public List<QuickFilterValues> QuickFilters { get; set; } = new List<QuickFilterValues>();
        public GridSearchType TextFilterType { get; set; }
        public List<EventLogFilterLevel> FilterLevel { get; set; } = new List<EventLogFilterLevel>();
        public bool UseDate { get; set; } = false;

    }
}
