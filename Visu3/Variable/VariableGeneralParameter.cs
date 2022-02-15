using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visu3
{
    public class VariableGeneralParameter : VariableParameter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Save { get; set; }
        public string Value { get; set; }

        public VariableGeneralParameter(int id, string name, string description, bool save, string value)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Save = save;
            this.Value = value;
        }

        public VariableGeneralParameter(int id, string name, string description, bool save)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Save = save;
            //this.Value = value;
        }
    }


}
