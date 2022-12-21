using System.Collections;

class MimeTypesModel
{
    public SortedList<string, string> Types { get; set; }

    public MimeTypesModel()
    {
        this.Types = GenerateListMimeTyper();
    }

    private SortedList<string, string> GenerateListMimeTyper()
    {
        
        SortedList<string, string> types = new SortedList<string, string>();
        
        types.Add(".html", "text/html");
        types.Add(".htm", "text/html");
        types.Add(".css", "text/css");
        types.Add(".js", "application/javascript");
        types.Add(".png", "image/png");
        types.Add(".jpg", "image/jpeg");
        types.Add(".gif", "image/gif");
        types.Add(".svg", "image/svg+xml");
        types.Add(".webp", "image/webp");
        types.Add(".ico", "image/x-icon");
        types.Add(".woff", "font/woff");
        types.Add(".woff2", "font/woff2");

        return types;
    }
}