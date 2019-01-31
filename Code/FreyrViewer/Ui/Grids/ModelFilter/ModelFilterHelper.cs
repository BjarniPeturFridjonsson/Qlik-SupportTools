using System.Collections.Generic;
using System.Net;

namespace FreyrViewer.Ui.Grids.ModelFilter
{
    public class ModelFilterHelper
    {
        public ColumnHeaderWrapper<T> CreateColumnHeaderWrapper<T,TType>(string columnName, string columnNameInModel)
        {
            var ret = new ColumnHeaderWrapper<T>
            {
                ColumnType = typeof(TType),
                HeaderName = columnName,
                Ordinal = 0,
                ColumnNameInModel = columnNameInModel,
                AspectGetter = p => GetAspectGetter<T, TType>(columnNameInModel, p)
            };
          
            return ret;
        }
       
        public List<QuickFilterDisplay> CreateDisplayOfQuickFilters(ModelFilterValues filter)
        {
            var ret = new List<QuickFilterDisplay>();
            var innerList = new List<QuickFilterValues>();
            bool isInGroup = false;
            string display = "";
            filter.QuickFilters.ForEach(p =>
            {
                if (p.ToBeModifiedInFilterEditor)
                {
                    return;
                }
                innerList.Add(p);
                if (!isInGroup && p.IsGroup)
                {
                    display += $" {p.FriendlyName} AND ";
                    isInGroup = true;
                }
                if (!p.IsGroup)
                {
                    display += $"{p.FriendlyName}";
                    isInGroup = false;
                    
                    ret.Add(new QuickFilterDisplay(innerList, display));
                    display = "";
                    innerList = new List<QuickFilterValues>();
                }
            });
            return ret;
        }
        private string GetAspectGetter<T, TType>(string nameInObject, T predicate ) 
        {

            return typeof(T).GetProperty(nameInObject)?.GetValue(predicate)?.ToString();

            
            //switch (typeof(TType).Name)
            //{
            //    case "Int64":
            //        ret.AspectGetterLong = p => (long)typeof(T).GetProperty(nameInObject)?.GetValue(p);
            //        break;
            //    case "Int32":
            //        ret.AspectGetterInt = p => typeof(T).GetProperty(nameInObject)?.GetValue(p) as int?;
            //        break;
            //    case "DateTime":
            //        ret.AspectGetterDateTime = p => typeof(T).GetProperty(nameInObject)?.GetValue(p) as DateTime?;
            //        break;
            //    case "Boolean":
            //        ret.AspectGetterBool = p => typeof(T).GetProperty(nameInObject)?.GetValue(p) as bool?;
            //        break;
            //    case "String":
            //        ret.AspectGetterString = p => typeof(T).GetProperty(nameInObject)?.GetValue(p) as string;
            //        break;
            //    default:
            //        break;
            //}
        }
        /*           AspectGetter = p => GetAspectGetter(nameInObject, p)
            };
            return ret;
        }

        private string GetAspectGetter<T>(string nameInObject, T p)
        {

            return typeof(T).GetProperty(nameInObject)?.GetValue(p) as string;
        }*/


    }
}
