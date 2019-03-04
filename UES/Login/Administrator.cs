using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIT.UES.Login
{
    public class Administrator : DatabaseType
    {
        public string AdminName { get; private set; }
        [Key]
        public int AdministratorID { get; private set; }

        public Administrator()
        {

        }
        public Administrator(string n)
        {
            AdminName = n;
        }

        public string GrantDepartmentAdminAuthority(Teacher teacher)
        {
            if (teacher.DepartmentAdminAuthority)
                return $"Teacher {teacher.PersonName} has already had the department administration authority.";
            else
            {
                teacher.GetDepartmentAdminAuthority();
                return null;
            }
        }
        public string RecallDepartmentAdminAuthority(Teacher teacher)
        {
            if (teacher.DepartmentAdminAuthority)
            {
                teacher.LoseDepartmentAdminAuthority();
                return null;
            }
            else
                return $"Teacher {teacher.PersonName} does not have the department administration authority.";
        }

        //public void SetDatabaseName(string name) => Settings.uesContext. = name;

        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
    }
}
