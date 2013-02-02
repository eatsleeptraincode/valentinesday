using System.Collections.Generic;

namespace Scoring.Web.Models
{
    public class Athlete : Entity
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public IList<string> Members { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}