using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES
{
    interface IOrderedDataContainer<T> where T: DatabaseType
    {
        void ExchangeOrder(T data1, T data2);
        void ChangeOrder(T data, ushort newOrder);
    }
}
