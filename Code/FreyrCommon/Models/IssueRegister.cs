namespace FreyrCommon.Models
{
    public class IssueRegister
    {
        public override string ToString()
        {
            return Name;
        }

        public string Name { get; set; }
        public string Type { get; set; }

    }
}
