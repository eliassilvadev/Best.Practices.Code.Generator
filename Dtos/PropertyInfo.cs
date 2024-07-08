using BestPracticesCodeGenerator.Extensions;

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

        public bool IsListProperty()
        {
            return Type.Contains("EntityList") || Type.Contains("List") || Type.Contains("Enumerable") || Type.Contains("Collection");
        }

        public string GetTypeConvertingToDtoWhenIsComplex(string prefixClassName = "", string sufixClassName = "")
        {
            var type = Type;

            type = type.Replace("IEntityList", "IList");
            type = type.Replace("EntityList", "List");

            if (type.Contains("<") && type.Contains(">") && (!string.IsNullOrWhiteSpace(prefixClassName) || !string.IsNullOrWhiteSpace(sufixClassName)))
            {
                var className = type.GetSubstringBetween("<", ">");

                type = type.Replace(className, prefixClassName + className + sufixClassName);
            }

            return type;
        }
    }
}