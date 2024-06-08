namespace BestPracticesCodeGenerator.Dtos
{
    public class PropertyInfo
    {
        public string Type { get; }
        public string Name { get; }
        public bool GenerateGetMethodOnRepository { get; set; }
        public bool PreventDuplication { get; set; }
        public PropertyInfo(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}