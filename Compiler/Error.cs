public class Error
{
    public override string ToString()
    {
        return Type + " " + Content;
    }
    public string Type;

    public string Content;

    public static List<Error> errors = new List<Error>();
}