using System;
using System.Collections.Generic;
using FreyrViewer.Ui.Grids.ModelFilter;

namespace FreyrViewer.Ui.Grids
{
    internal class GridAndDataWrapper<T>
    {
        public bool GridInvoked { get; set; }
        public Func<GridHelper<T>> GridInvoker { get; set; }
        public GridHelper<T> GridHelper { get; set; }
        public List<T> Data { get; set; }
        public ModelFilterValues Filter = new ModelFilterValues();
        public ModelFilterService<T> ColorFilterer;
        public ModelFilterValues ColorFilter = new ModelFilterValues();
        public Dictionary<string, ColumnHeaderWrapper<T>> Headers { get; set; }
        public Func<T, DateTime> DateColumnGetter { get; set; }
    }
}
