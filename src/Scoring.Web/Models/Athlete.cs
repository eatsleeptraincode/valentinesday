using Newtonsoft.Json;

namespace Scoring.Web.Models
{
    public class Athlete : Entity
    {
        public string Name { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}