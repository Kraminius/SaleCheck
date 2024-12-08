
namespace SaleCheck.HtmlLib;
public class Tag(string type, List<Property> properties)
{
    public string? Content { get; set; }
    public readonly List<Tag> ChildTags = new List<Tag>();
    public Tag? ParentTag { get; set; }
    public readonly string TagType = type;
    private readonly List<Property> _properties = properties;

    public string? GetProperty(string property)
    {
        foreach (Property p in _properties)
        {
            if(p.Type == property) return p.Value;
        }
        return null;
    }
    public void SearchWithFilter(List<Tag> head, Filter filters)
    {
        foreach (Tag child in ChildTags)
            child.SearchWithFilter(head, filters);
        
        if(filters.GetTagType() != null && filters.GetTagType() != TagType) return;
        if(filters.GetContent() != null && !(Content??"").Contains(filters.GetContent()??"cannot be null")) return;
        bool meetsAll = true;
        foreach (string[] property in filters.GetProperties())
        {
            bool found = false;
            foreach (Property p in _properties)
            {
                if (property[0] == p.Type)
                {
                    if(property.Length == 1) found = true;
                    else if(p.Value.Contains(property[1])) found = true;
                }
            }
            if (!found)
            {
                meetsAll = false;
                break;
            }
        }
        if(meetsAll) head.Add(this);
    }

    public string ToString(int? indents = 0)
    {
        string indentString = "";
        for (int i = 0; i < indents; i++)
        {
            indentString += "   ";
        }

        string propertyString = "";
        foreach (Property p in _properties)
        {
            propertyString += p + ", ";
        }

        string childrenString = "\n";
        foreach (Tag tag in ChildTags)
        {
            childrenString += tag.ToString(indents+1) + "\n";
        }

        string content = Content ?? "";
        return
            indentString + "type: "+ TagType + "\n" +
            indentString + "content: " + content.Trim() + "\n" +
            indentString + "properties: " + propertyString + "\n" +
            indentString + "children: " + childrenString + "\n";

    }
}

