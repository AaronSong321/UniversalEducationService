using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES
{
    interface IConditionallyRetrievableContainer<T> where T: DatabaseType
    {
        List<T> GetAllConditionallyRetrievableData();
        List<T> GetConditionallyRetrievableData(Predicate<T> filter);
        List<T> GetConditionallyRetrievableData(string indexWord);
    }
}
