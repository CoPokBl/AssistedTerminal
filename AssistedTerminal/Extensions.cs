namespace AssistedTerminal; 

public static class Extensions {
    public static string Shorten(this string str, int maxLength) {
        if (str.Length > maxLength) {
            str = str[..(maxLength / 2)] + "..." + str[^(maxLength / 2)..];
        }
        return str;
    }
}