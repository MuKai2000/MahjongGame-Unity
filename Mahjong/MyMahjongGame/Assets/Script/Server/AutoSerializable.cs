using System;
using System.Collections.Generic;
using System.Text;

namespace Mahjong_Server
{
    interface AutoSerializable<T>
    {
        public T convert(Object obj);
    }
}
