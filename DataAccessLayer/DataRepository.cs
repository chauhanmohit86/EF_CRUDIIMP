using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DataRepository
    {
        public static DemoEntities DemoEntitiesContext = new DemoEntities(); 
    }
}
