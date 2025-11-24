using UnityEngine;

public static class CurrencyFormatter
{
    public static string ToRupiah(float amount)
    {
        return "Rp " + string.Format("{0:N0}", amount).Replace(",", ".");
    }
}