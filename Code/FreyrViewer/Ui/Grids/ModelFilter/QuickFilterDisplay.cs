using System.Collections.Generic;

namespace FreyrViewer.Ui.Grids.ModelFilter
{
    public class QuickFilterDisplay
    {
        private readonly List<QuickFilterValues> _quickFilterValues;
        private readonly string _displayString;

        public QuickFilterDisplay(List<QuickFilterValues> filtervalues, string displayString)
        {
            _quickFilterValues = filtervalues;
            _displayString = displayString;
        }

        public List<QuickFilterValues> GetFilters()
        {
            return _quickFilterValues;
        }

        public override string ToString()
        {
            return _displayString;
        }
    }
}
