
namespace SaleCheck.HtmlLib;

public static class HtmlUtils
{
    public static string Trim(string html)
    {
        html = html.Trim();
        html = html.Replace("<!DOCTYPE html>", "", StringComparison.OrdinalIgnoreCase);
        html = html.Replace("<!doctype html>", "", StringComparison.OrdinalIgnoreCase);
        
        int startHead = html.IndexOf("<head", StringComparison.OrdinalIgnoreCase);
        int endHead = html.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);

        if (startHead != -1 && endHead != -1)
        {
            // Calculate the length to remove, including the closing </head> tag
            int lengthToRemove = (endHead + "</head>".Length) - startHead;
            html = html.Remove(startHead, lengthToRemove);
        }
            
        return html;
    }
    private static bool IsSelfClosingTag(string tag)
    {
        if (tag.Contains("!--")) return true; //html comments
        tag = tag.ToLower();
        switch (tag)
        {
            case "area":
            case "base":
            case "br":
            case "col":
            case "embed":
            case "hr":
            case "img":
            case "input":
            case "link":
            case "meta":
            case "source":
            case "track":
            case "wbr":
            case "frame":    
            case "spacer":
            case "circle":
            case "rect":
            case "eclipse":
            case "line":
            case "polygon":
            case "polyline": 
            case "animate":    
            case "animatetransform":    
            case "stop":    
            case "use":    
            case "fegaussianblur":    
            case "feoffset":    
            case "feblend":    
            case "fecolormatrix":    
            case "fecomposite":    
            case "feflood":    
            case "femergenode":    
            case "femorphology":    
            case "feturbulence":    
            case "fefunca":    
                return true;
            default: 
                return false;
        }
    }

    private static bool IsTag(string tag)
    {
        if (tag.Equals("")) return false; 
        if (tag.Contains("!--")) return true; //html comments
        if (tag[0] == ' ') return false;
        tag = tag.ToLower();
        switch (tag)
        {
            case "a":
            case "div":
            case "p":
            case "h1":
            case "h2":
            case "h3":
            case "h4":
            case "h5":
            case "h6":
            case "img":
            case "ul":
            case "ol":
            case "li":
            case "table":
            case "th":
            case "tr":
            case "td":
            case "tbody":
            case "thead":
            case "tfoot":
            case "input":
            case "button":
            case "form":
            case "span":
            case "header":
            case "footer":
            case "section":
            case "nav":
            case "article":
            case "label":
            case "svg":
            case "blockquote":
            case "em":
            case "strong":
            case "iframe":
            case "audio":
            case "video":
            case "script":
            case "style":
            case "area":
            case "base":
            case "br":
            case "col":
            case "colgroup":
            case "embed":
            case "hr":
            case "link":
            case "meta":
            case "source":
            case "track":
            case "wbr":
            case "head":
            case "html":
            case "title":
            case "body":
            case "address":
            case "abbr":
            case "b":
            case "basefont":
            case "bdi":
            case "bdo":
            case "canvas":
            case "caption":
            case "cite":
            case "code":
            case "data":
            case "datalist":
            case "dd":
            case "del":
            case "dfn":
            case "dl":
            case "dt":
            case "figcaption":
            case "figure":
            case "ins":
            case "kbd":
            case "main":
            case "mark":
            case "noscript":
            case "object":
            case "optgroup":
            case "option":
            case "output":
            case "picture":
            case "pre":
            case "q":
            case "rp":
            case "rt":
            case "ruby":
            case "s":
            case "samp":
            case "small":
            case "sub":
            case "summary":
            case "sup":
            case "template":
            case "time":
            case "u":
            case "var":
            case "fieldset":
            case "legend":
            case "details":
            case "dialog":
            case "acronym":
            case "applet":
            case "big":
            case "blink":
            case "center":
            case "content":
            case "font":
            case "frame":
            case "frameset":
            case "marquee":
            case "nobr":
            case "plaintext":
            case "shadow":
            case "spacer":
            case "strike":
            case "tt":
            case "xmp":
            case "g":
            case "circle":
            case "rect":
            case "path":
            case "defs":
            case "symbol":
            case "marker":
            case "pattern":
            case "mask":
            case "filter":
            case "clipPath":
            case "lineargradient":
            case "radialgradient":
            case "text":
            case "textPath":
            case "eclipse":
            case "line":
            case "polygon":
            case "polyline":
            case "animate":    
            case "animatetransform":    
            case "stop":    
            case "use":    
            case "fegaussianblur":    
            case "feoffset":    
            case "feblend":    
            case "fecolormatrix":    
            case "fecomposite":    
            case "feflood":    
            case "femergenode":    
            case "femorphology":    
            case "feturbulence":    
            case "animatemotion":    
            case "fecomponenttransfer":    
            case "femerge":    
            case "aside":    
            case "fefunca":    
            case "i":    
            case "select":    
                return true;
            default:
                return true;
        }
    }

    
    /**
  * @param string str, a string that could contain properties
  * @returns List of Property, the properties and their values in order.
  */
    private static List<Property> ParseProperties(string str)
    {
        List<Property> properties = new List<Property>();
        char[] arr = str.ToCharArray();
        string key = "";
        string value = "";
        bool isInString = false;
        bool isParsingValue = false;

        for (int i = 0; i < arr.Length; i++)
        {
            char current = arr[i];

            if (current == '"')
            {
                isInString = !isInString;
            }
            else if (current == '=' && !isInString)
            {
                isParsingValue = true;
            }
            else if (char.IsWhiteSpace(current) && !isInString)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    properties.Add(new Property(key, value));
                    key = "";
                    value = "";
                    isParsingValue = false;
                }
            }
            else
            {
                if (isParsingValue)
                {
                    value += current;
                }
                else
                {
                    key += current;
                }
            }
        }
        
        if (!string.IsNullOrEmpty(key))
        {
            properties.Add(new Property(key, value));
        }

        return properties;
    }

    /** 
     * @param string html, a html to be parsed into an object-oriented format.
     * @param Tag parent, the parent tag, that will hold the children.
     * @returns string, the rest of the html after a tag is found within.
     */
    public static string ParseHtml(string html, Tag parent)
    {
        int currentIndex = 0;

        while (currentIndex < html.Length)
        {
            // Find the next opening tag
            int openStartIndex = html.IndexOf('<', currentIndex);
            if (openStartIndex == -1) 
            {
                // No more tags, remaining content is plain text
                parent.Content += html.Substring(currentIndex).Trim();
                break;
            }

            // Add text content between the last tag and this one
            if (openStartIndex > currentIndex)
            {
                parent.Content += html.Substring(currentIndex, openStartIndex - currentIndex).Trim();
            }

            // Find the end of the opening tag
            int openEndIndex = html.IndexOf('>', openStartIndex);
            if (openEndIndex == -1)
            {
                throw new FormatException("Malformed HTML: Missing '>' for opening tag.");
            }

            // Extract tag information
            string tagDefinition = html.Substring(openStartIndex + 1, openEndIndex - openStartIndex - 1).Trim();

            // Handle closing tags
            if (tagDefinition.StartsWith("/"))
            {
                return html.Substring(openEndIndex + 1); // Return remaining HTML
            }

            // Check for self-closing tag
            bool selfClosing = IsSelfClosingTag(tagDefinition) || tagDefinition.EndsWith("/");

            // Parse the tag
            Tag childTag = ParseTag(tagDefinition);
            parent.ChildTags.Add(childTag);

            if (selfClosing)
            {
                currentIndex = openEndIndex + 1;
                continue;
            }

            // Recursively parse children
            string childHtml = html.Substring(openEndIndex + 1);
            string remainingHtml = ParseHtml(childHtml, childTag);

            // Move to the remaining HTML
            currentIndex = html.Length - remainingHtml.Length;
        }

        return html.Substring(currentIndex);
    }


    private static int IndexOfClosingTag(string html, string tag, int startIndex)
    {
        Stack<int> openTags = new Stack<int>();
        while (startIndex < html.Length)
        {
            int openIndex = html.IndexOf($"<{tag}", startIndex, StringComparison.OrdinalIgnoreCase);
            int closeIndex = html.IndexOf($"</{tag}>", startIndex, StringComparison.OrdinalIgnoreCase);

            if (openIndex != -1 && (closeIndex == -1 || openIndex < closeIndex))
            {
                openTags.Push(openIndex);
                startIndex = openIndex + tag.Length;
            }
            else if (closeIndex != -1)
            {
                if (openTags.Count > 0)
                    openTags.Pop();
                else
                    return closeIndex; // Found unmatched closing tag
                startIndex = closeIndex + tag.Length;
            }
            else
            {
                break; // No more tags
            }
        }
        return openTags.Count == 0 ? -1 : openTags.Peek();
    }


    private static Tag ParseTag(string tagDefinition)
    {
        string[] parts = tagDefinition.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
        string tagType = parts[0].ToLower();
        string properties = parts.Length > 1 ? parts[1] : string.Empty;

        List<Property> parsedProperties = ParseProperties(properties);
        return new Tag(tagType, parsedProperties);
    }

}