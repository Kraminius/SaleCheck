
namespace SaleCheck.HtmlLib;

public static class HtmlUtils
{
    public static string Trim(string html)
    {
        html = html.Trim();
        html = html.Replace("<!DOCTYPE html>", "");
        html = html.Replace("<!doctype html>", "");
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
  * @returns List of string, the properties and their values in order.
  */
    private static List<string> ParseProperties(string str){
        List<string> list = new List<string>();
        char[] arr = str.ToCharArray();
        string current = "";
        bool isInString = false;
        for(int i = 0; i < arr.Length; i++){
            if(arr[i]== '"') {
                if(i > 0 && !arr[i-1].Equals('/') ) {
                    isInString = !isInString;
                }
            }
            else if(arr[i].Equals(' ')){
                if(!isInString){
                    list.Add(current);
                    current = "";
                }
                else current += arr[i];
            }
            else if(arr[i].Equals('=')){
                if(!isInString){
                    list.Add(current);
                    current = "";
                }
            }
            else current += arr[i];
        }
        list.Add(current);
        return list;
    }
    /** 
     * @param string html, a html to be parsed into an object-oriented format.
     * @param Tag parent, the parent tag, that will hold the children.
     * @returns string, the rest of the html after a tag is found within.
     */
    public static string ParseHtml(string html, Tag parent)
    {
        if (parent.TagType == "script" || parent.TagType.Contains("<--")) //Is script or html comment
        {
            parent.Content = html;
            return "";
        }
        int globalStartIndex = 0;
        int openStartIndex;
        while (true)
        {
            openStartIndex = html.IndexOf('<', globalStartIndex);
            if (openStartIndex == -1) //End of html
            {
                parent.Content += html;
                return "";
            }
            globalStartIndex = openStartIndex + 1;
            if(html[globalStartIndex].Equals('/')) continue; //Found a closing tag, that is not what we are looking for.
            if(html[globalStartIndex].Equals(' ')) continue; //Found a space like '< ' meaning it is probably in text
            break;
        }
        parent.Content += html.Substring(0, openStartIndex);
        int openEndIndex = html.IndexOf('>', openStartIndex);
        if (openEndIndex == -1)
        {
            //Invalid html format, missing '>' after '<'
            parent.Content += html.Substring(openStartIndex);
            return "";
        }
        Tag? tag = null;
        try
        {
            tag = ReadTagAndProperties(html, openStartIndex, openEndIndex);
        }
        catch (FormatException e)
        {
            if (parent.TagType == "script") return ""; //Someone inserted < into a piece of text like in a written for loop or a lesser sign.
            throw;
        }
        parent.ChildTags.Add(tag);
        if (IsSelfClosingTag(tag.TagType))
        {
            if (openEndIndex + 1 >= html.Length) return "";
            return html.Substring(openEndIndex + 1);
        }
        int closingStartIndex = IndexOfClosingTag(html, tag.TagType, openEndIndex);
        if (closingStartIndex == -1)
        {
            parent.Content += html;
            return "";
        }
        int closingEndIndex = html.IndexOf('>', closingStartIndex);
        if(closingEndIndex == -1) throw new FormatException("Invalid html format, '>' is missing for closing tag: " + tag.TagType);
        string childrenHtml = html.Substring(openEndIndex+1, closingStartIndex-openEndIndex-1);
        while(true)
        {
            childrenHtml = ParseHtml(childrenHtml, tag);
            if(childrenHtml.Length == 0) break;
        }
        if (closingEndIndex + 1 >= html.Length) return "";
        return html.Substring(closingEndIndex + 1);
    }

    private static int IndexOfClosingTag(string html, string tag, int startIndex)
    {
        int total = 0;
        int opens = 0;
        int closes = 0;
        int last = 0;
        while (true)
        {
            last = startIndex;
            int nextIndexOfCloser = html.IndexOf("</"+tag, startIndex, StringComparison.Ordinal);
            if(nextIndexOfCloser == -1) return -1;
            int nextIndexOfOpener = html.IndexOf('<'+tag, startIndex, StringComparison.Ordinal);
            if(nextIndexOfOpener == -1) 
            {
                if(opens == closes) return nextIndexOfCloser;
                closes++;
                startIndex = nextIndexOfCloser + tag.Length + 1;
                continue;
            }
            if (nextIndexOfOpener < nextIndexOfCloser)
            {
                startIndex = nextIndexOfOpener + tag.Length + 1;
                opens++;
            }
            else if (opens == closes)
            {
                return nextIndexOfCloser;
            }
            else
            {
                startIndex = nextIndexOfCloser+ tag.Length + 1;
                closes++;
            }

            if (startIndex < last)
            {
                return -1;
            };
        }
    }

    private static Tag ReadTagAndProperties(string html, int startIndex, int endIndex)
    {
        string insideHtml = html.Substring(startIndex + 1, endIndex - startIndex - 1);
        List<String> components = HtmlUtils.ParseProperties(insideHtml);
        string tagType = components[0];
        if(!HtmlUtils.IsTag(tagType)) throw new FormatException("Invalid tag type: " + components[0] + " : " + insideHtml);
        List<Property> properties = new List<Property>();
        for (int i = 1; i+1 < components.Count; i+=2)
        {
            properties.Add(new Property(components[i], components[i+1]));
        }
        Tag tag = new Tag(tagType, properties);
        return tag;
    }
}