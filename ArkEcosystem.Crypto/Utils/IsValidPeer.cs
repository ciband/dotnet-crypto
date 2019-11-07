// Copyright (c) 2019 Ark Ecosystem <info@ark.io>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ArkEcosystem.Crypto {

public static class ValidPeer {
    public static bool IsLocalHost(string ip) {
        var addr = IPAddress.Parse(ip);
        if (addr == IPAddress.Loopback || addr.GetAddressBytes()[0] == 0) {  //TODO: from TS: || ["127.0.0.1", "::ffff:127.0.0.1"].includes(ip)  needed???
            return true;
        }
        var host = Dns.GetHostEntry(Dns.GetHostName());
        return host.AddressList.Any(local_ip => local_ip.AddressFamily == AddressFamily.InterNetwork && local_ip.ToString() == ip);
    }

    public static string SanitizeRemoteAddress(string ip) {
        try {
        var addr = IPAddress.Parse(ip);
        return addr.MapToIPv4().ToString();
        } catch {
            return null;
        }
    }

    public static bool IsValidPeer(string ip, string status) {
        ip = SanitizeRemoteAddress(ip);

        if (string.IsNullOrEmpty(ip)) { return false; }

        if (IsLocalHost(ip)) { return false; }

        return true;
    }
}

}
