using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adjustment.Project
{
    internal interface IMemento<TObject> where TObject : class
    {
        public TObject GetState();
    }
}
