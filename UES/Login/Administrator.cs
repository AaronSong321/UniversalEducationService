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
        [MaxLength(30),Required]
        public string AdminName { get; private set; }
        [MinLength(6),MaxLength(12), Required]
        public string Password { get; private set; }
        [Key]
        public int AdministratorID { get; private set; }

        public Administrator()
        {

        }
        public Administrator(string name, string password)
        {
            AdminName = name;
            Password = password;
        }

        public void GrantDepartmentAdminAuthority(Teacher teacher, out string em)
        {
            if (teacher.DepartmentAdminAuthority)
            {
                em = $"Teacher {teacher.PersonName} has already had the department administration authority.";
            }
            else
            {
                teacher.GetDepartmentAdminAuthority();
                Settings.SaveDataModification(teacher);
                em = null;
            }
        }
        public void RecallDepartmentAdminAuthority(Teacher teacher, out string em)
        {
            if (teacher.DepartmentAdminAuthority)
            {
                teacher.LoseDepartmentAdminAuthority();
                Settings.SaveDataModification(teacher);
                em = null;
            }
            else
            {
                em = $"Teacher {teacher.PersonName} does not have the department administration authority.";

            }
        }

        //public void SetDatabaseName(string name) => Settings.uesContext. = name;

        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
    }
}
