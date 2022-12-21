using System.Collections;

class MimeTypesModel
{
    public SortedList<string, string> Types { get; set; }

    public MimeTypesModel()
    {
        this.Types.Add(".html", "text/html");
        this.Types.Add(".htm", "text/html");
        this.Types.Add(".css", "text/css");
        this.Types.Add(".js", "application/javascript");
        this.Types.Add(".png", "image/png");
        this.Types.Add(".jpg", "image/jpeg");
        this.Types.Add(".gif", "image/gif");
        this.Types.Add(".svg", "image/svg+xml");
        this.Types.Add(".webp", "image/webp");
        this.Types.Add(".ico", "image/x-icon");
        this.Types.Add(".woff", "font/woff");
        this.Types.Add(".woff2", "font/woff2");
    }
}