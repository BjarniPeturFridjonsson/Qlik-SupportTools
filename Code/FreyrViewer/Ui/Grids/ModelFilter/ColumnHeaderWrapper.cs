using System;

namespace FreyrViewer.Ui.Grids.ModelFilter
{
    public class ColumnHeaderWrapper<T>
    {
        public string HeaderName { get; set; }
        public int MaxCharCount { get; set; }
        public string HeaderNameInFile { get; set; }
        public string ColumnNameInModel { get; set; }
        public int Ordinal { get; set; }
        public bool ShowInGrid { get; set; } = true;
        public bool ShowInDetailView { get; set; } = true;
        public bool HasWarning { get; set; } = true;
        public bool HasErrors { get; set; } = true;
        public bool FileIsEmpty { get; set; } = true;
        public Type ColumnType { get; set; }
        public Func<T, string> AspectGetter { get; set; }
        //public Func<T, string> AspectGetterString { get; set; }
        //public Func<T, long> AspectGetterLong { get; set; }
        //public Func<T, int> AspectGetterInt { get; set; }
        //public Func<T, bool> AspectGetterBool { get; set; }
        //public Func<T, DateTime> AspectGetterDateTime { get; set; }
        public ColumnHeaderWrapper<T> IgnoreInGrid()
        {
            ShowInGrid = false;
            return this;
        }

    }
}
