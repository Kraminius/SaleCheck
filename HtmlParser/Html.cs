
namespace SaleCheck.HtmlLib;
public class Html
{
    public readonly string StringHtml;
    private readonly List<Tag> _tags = new List<Tag>();

    public Html(string html)
    {
        this.StringHtml = HtmlUtils.Trim(html);
        Parse();
    }
    private void Parse()
    {
        string current = StringHtml;
        while (true)
        {
            string temp = current;
            Tag tag = new Tag("html", new List<Property>());
            current = HtmlUtils.ParseHtml(current, tag);
            _tags.AddRange(tag.ChildTags);
            if (current.Length == 0) break;
            if (current == temp) return;
        }
    }

    public Tag? SearchForTag(Filter filter, Tag? specificChild = null)
    {
        List<Tag> tags = new List<Tag>();
        if(specificChild != null) tags.Add(specificChild);
        else tags.AddRange(_tags);
        List<Tag> result = TagsMeetingFilters(filter, tags);
        if(result.Count == 0) return null;
        return result[0];
    }

    public string? SearchForValue(Filter filter, Tag? specificChild = null)
    {
        List<Tag> tags = new List<Tag>();
        if(specificChild != null) tags.Add(specificChild);
        else tags.AddRange(_tags);
        List<Tag> result = TagsMeetingFilters(filter, tags);
        if(result.Count == 0) return null;
        string? outputType = filter.GetOutput();
        Filter? child = filter.GetChild();
        if (child != null)
        {
            return SearchForValue(child, result[0]);
        }
        if(outputType != null) {
            switch (outputType)
                {
                    case "content": return result[0].Content;
                    default: return result[0].GetProperty(outputType);
                }
        }
        return null;
    }

    public List<Tag> SearchForTags(Filter filter, Tag? specificChild = null)
    {
        List<Tag> tags = new List<Tag>();
        if(specificChild != null) tags.Add(specificChild);
        else tags.AddRange(_tags);
        return TagsMeetingFilters(filter, tags);
    }
   
    private List<Tag> TagsMeetingFilters(Filter filters, List<Tag> tags)
    {
        List<Tag> result = new List<Tag>();
        foreach (Tag tag in tags)
            tag.SearchWithFilter(result, filters);
        return result;
    }
    
    public override string ToString()
    {
        string output = "";
        foreach (Tag tag in _tags)
        {
            output += tag.ToString() + "\n";
        }
        return output;
    }
}