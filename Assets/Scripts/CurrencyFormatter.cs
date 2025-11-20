using UnityEngine;

public static class CurrencyFormatter
{
    public static string ToRupiah(int amount)
    {
        return "Rp " + string.Format("{0:N0}", amount).Replace(",", ".");
    }
}