using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace DevHub.Helpers;

public class VnpayLibrary
{
    private readonly SortedList<string, string> _req = new(new VnpCompare());
    private readonly SortedList<string, string> _res = new(new VnpCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _req[key] = value;
        }
    }

    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _res[key] = value;
        }
    }

    public string GetResponseData(string key)
    {
        return _res.TryGetValue(key, out var value) ? value : string.Empty;
    }

    public string CreateRequestUrl(string baseUrl, string hashSecret)
    {
        var data = new StringBuilder();
        foreach (var kv in _req)
        {
            data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
        }
        var query = data.ToString();
        var sign = query.Remove(query.Length - 1);
        var secureHash = HmacSha512(hashSecret, sign);
        return $"{baseUrl}?{query}vnp_SecureHash={secureHash}";
    }

    public bool ValidateSignature(string inputHash, string hashSecret)
    {
        var data = new StringBuilder();
        foreach (var kv in _res.Where(k => k.Key != "vnp_SecureHash" && k.Key != "vnp_SecureHashType"))
        {
            data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
        }
        var sign = data.ToString();
        if (sign.Length > 0)
        {
            sign = sign.Remove(sign.Length - 1);
        }
        return HmacSha512(hashSecret, sign).Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private static string HmacSha512(string key, string input)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        var hex = BitConverter.ToString(hashValue);
        return hex.Replace("-", "").ToLower();
    }
}

public class VnpCompare : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        var vnpCompare = string.CompareOrdinal(x, y);
        return vnpCompare;
    }
}
