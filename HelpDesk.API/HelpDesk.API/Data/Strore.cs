using HelpDesk.API.Models;
using System.Reflection;

namespace HelpDesk.API.Data
{
    public static class Store
    {
        public static List<Department> departmentsList = new List<Department>
        {
            new Department{Id=1, Title="Рота Звя'зку" },
            new Department{Id=2, Title="Рота Охорони" },
            new Department{Id=3, Title="Рота Підтримки" },
            
        };
    }
}
