using System;

public static class ResourceUI
{
    public static string FormatRp(float value)
    {
        return string.Format("Rp{0:#,0}", value);
    }
}