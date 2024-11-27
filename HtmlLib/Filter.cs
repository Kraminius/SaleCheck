
namespace SaleCheck.HtmlLib;

public class Filter
{
    private string? _tagType;
    private List<string[]> _properties = new List<string[]>();
    private string? _content;
    private string? _output;
    private Filter? _child;

    public Filter(string? rule = null)
    {
        if (rule != null)
        {
            string[] rules = rule.Split('|', 5);
            if(rules[0] != "_") _tagType = rules[0];
            if (rules[1] != "_")
            {
                string[] properties = rules[1].Split(' ');
                foreach (string property in properties)
                {
                    string[] values = property.Split('=');
                    if(values.Length != 2) continue;
                    _properties.Add(new string[]{ values[0], values[1] });
                }
            }
            if(rules[2] != "_") _content = rules[2];
            if(rules[3] != "_") _output = rules[3];
            if(rules[4] != "_") _child = new Filter(rules[4]);
        }
    }
    public override string ToString()
    {
        string stringProperties = "";
        if (_properties.Count > 0)
        {
            foreach (string[] property in _properties)
            {
                if(property.Length != 2) continue;
                stringProperties += property[0] + "=" + property[1] + " ";
            }
        }
        else stringProperties = "_";
        return (_tagType ?? "_") + "|" + stringProperties + "|" + (_content ?? "_") + "|" + (_output ?? "_")  + "|" + (_child?.ToString() ?? "_");
    }

    public Filter Property(string property, string? value = null)
    {
        if(value != null) _properties.Add(new string[]{property, value});
        else _properties.Add(new string[]{property});
        return this;
    }
    public Filter Content(string content)
    {
        if(this._content != null) throw new Exception($"Previous content assigned: {_content}, you can only filter for one type of content at a time.");
        this._content = content;
        return this;
    }
    public Filter Tag(string tagType)
    {
        if(this._tagType != null) throw new Exception($"Previous tag type assigned {_tagType}, you can only filter for one type of tag at a time.");
        this._tagType = tagType;
        return this;
    }

    public Filter Output(string output)
    {
        this._output = output;
        return this;
    }

    public Filter Child(Filter child)
    {
        this._child = child;
        return this;
    }

    public Filter? GetChild()
    {
        return _child;
    }
    public string? GetTagType()
    {
        return this._tagType;
    }
    public string? GetContent()
    {
        return this._content;
    }
    public List<string[]> GetProperties()
    {
        return this._properties;
    }

    public string? GetOutput()
    {
        return _output;
    }

    
}