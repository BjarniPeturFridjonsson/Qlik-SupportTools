using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BrightIdeasSoftware;

namespace FreyrViewer.Ui.Grids.ModelFilter
{
    public class ModelFilterService<T> : IModelFilter
    {
        private readonly ModelFilterValues _filter;
        private readonly Dictionary<string,ColumnHeaderWrapper<T>> _headers;
        public Func<T,DateTime> DateColumnGetter { get; set; }

        public ModelFilterService(ModelFilterValues filter, Dictionary<string, ColumnHeaderWrapper<T>> headers, Func<T, DateTime> dateTimeFilterGetter)
        {
      
            _filter = filter;
            _headers = headers;
            DateColumnGetter = dateTimeFilterGetter;
            if (headers == null)
            {
                throw new Exception("The headers collection in filters cant be null");
            }
        }

        public void ColorFilter(object modelObject, OLVListItem item)
        {
            
            var row = ((T)modelObject);
            foreach (var qFilter in _filter.QuickFilters)
            {
                if (!qFilter.ColorFilter)
                {
                    continue;
                }
                bool qFound = false;
                var regex = new Regex(Regex.Escape(qFilter.FilterValue), RegexOptions.IgnoreCase);
                if (!_headers.ContainsKey(qFilter.ColumnName))
                    continue;
                var stuff = _headers[qFilter.ColumnName].AspectGetter.Invoke(row);
                if(stuff == null)
                    continue;
                if (regex.Matches(stuff).Count > 0) qFound = true;
              

                if (qFound)
                {
                    item.BackColor = qFilter.RowColor;
                    return;
                }
                    
            }
        }

        public bool Filter(object modelObject)
        {
            var row = ((T)modelObject); //SuccessAudit
            if (_filter.UseDate && DateColumnGetter != null)
            {
                var date = DateColumnGetter.Invoke(row);
                if (_filter.UseDate && !(date >= _filter.DateFrom && date <= _filter.DateTo))
                    return false;
            }
       

           
            if (_filter.QuickFilters.Any())
            {
                bool result = true;
                bool isInGroup = false;
                bool groupResult = true;
                

                foreach (var filterRow in _filter.QuickFilters)
                {
                    
                    if (filterRow.ColorFilter ) continue;
                    if (!isInGroup && filterRow.IsGroup) isInGroup = true;

                    bool foundInRow = false;
                    
                    var regex = new Regex(filterRow.FilterValue,RegexOptions.IgnoreCase);
                    if (filterRow.ColumnName == null)//basic filters on all columns returns on first hit.
                    {
                        
                        foreach (KeyValuePair<string, ColumnHeaderWrapper<T>> item in _headers)
                        {
                            if (item.Value.ColumnType == typeof(DateTime))
                            {//this is string only comparison
                                continue;
                            }
                            var columnValue = item.Value.AspectGetter.Invoke(row);
                            if (columnValue != null)
                            {
                                if (regex.Matches(columnValue).Count > 0)
                                {
                                    foundInRow = true;
                                    //return true;
                                    break;
                                    
                                }
                                
                            }
                        }
                        
                    }
                    else // filter on specific column and multiple columns conditions supported
                    {
                        
                        if (!_headers.ContainsKey(filterRow.ColumnName))
                            continue;
                        var stuff = _headers[filterRow.ColumnName].AspectGetter.Invoke(row);
                        if (stuff == null)
                            continue;
                        if (regex.Matches(stuff).Count > 0) foundInRow = true;

                        if (filterRow.NegativeFilter && !filterRow.IsGroup && !isInGroup)
                        {
                            foundInRow = foundInRow == false;
                            if (!foundInRow) return false;
                        }
                    }


                    if (!foundInRow)
                    {
                        if (filterRow.IsGroup || isInGroup)
                            groupResult = false;
                        else
                            result = false;
                    }
                        

                    if (!filterRow.IsGroup && isInGroup)
                    {//end of a group.
                        if (filterRow.NegativeFilter)
                        {
                            groupResult = groupResult == false;
                        }
                        
                        if (!groupResult)
                            result = false;

                        isInGroup = false;
                        groupResult = true;
                    }

                }

               //if (tFound.HasValue && !tFound.Value) return false;
               //if (tFound.HasValue && tFound.Value && !result.HasValue) return true;

                return result;
            }

            return true;
        }
    }
}
