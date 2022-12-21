using System.Text;

class HandledRequest
{
    public string MethodHttp { get; set; }
    public string VersionHttp { get; set; }
    public string ResourceRequested { get; set; }
    public string HostName { get; set; }
    public string MimeType { get; set; }
    public string RawText { get; set; }
    public SortedList<string, string> Params { get; set; }

    public HandledRequest(byte[] bytesRequest)
    {

        string textRequest = Encoding.UTF8.GetString(bytesRequest).Replace((char)0, ' ').Trim();

        this.RawText = textRequest;

        string[] linesTextRequest = textRequest.Split("\r\n");
        int iFirstSpace = linesTextRequest[0].IndexOf(' ');
        int iSecondSpace = linesTextRequest[0].LastIndexOf(' ');

        this.MethodHttp = linesTextRequest[0].Substring(0, iFirstSpace);
        this.VersionHttp = linesTextRequest[0].Substring(iSecondSpace + 1);

        this.ResourceRequested = ExtractResourceRequested(linesTextRequest, iFirstSpace, iSecondSpace);

        this.Params = ExtractParams(linesTextRequest, iFirstSpace, iSecondSpace);

        iFirstSpace = linesTextRequest[1].IndexOf(' ');
        this.HostName = linesTextRequest[1].Substring(iFirstSpace + 1);
    }

    private string ExtractResourceRequested(string[] linesTextRequest, int iFirstSpace, int iSecondSpace)
    {
        string resource = linesTextRequest[0].Substring(
            iFirstSpace + 1, iSecondSpace - iFirstSpace - 1);

        resource = resource.Contains("?") ? resource.Split("?")[0] : resource;

        resource = resource == "/" ? "/index.html" : resource;

        return resource;
    }

    private SortedList<string, string> ExtractParams(string[] linesTextRequest, int iFirstSpace, int iSecondSpace)
    {
        SortedList<string, string> paramsRequest = new SortedList<string, string>();

        string resource = linesTextRequest[0].Substring(
            iFirstSpace + 1, iSecondSpace - iFirstSpace - 1);

        string textParams = resource.Contains("?") ? resource.Split("?")[0] : "";

        if (!string.IsNullOrEmpty(textParams.Trim()))
        {
            string[] pairKeysValues = textParams.Split("&");

            foreach (string pair in pairKeysValues)
            {
                string key = pair.Split("=")[0].ToLower();
                string value = pair.Split("=")[1].ToLower();

                paramsRequest.Add(key, value);
            }
        }
        
        return paramsRequest;
    }
}