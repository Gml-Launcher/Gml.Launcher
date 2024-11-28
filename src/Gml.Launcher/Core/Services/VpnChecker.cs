using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace Gml.Launcher.Core.Services;

public class VpnChecker : IVpnChecker
{
    private readonly string[] VpnKeywords =
    [
        "WireGuard",
        "OpenVPN",
        "VLESS",
        "VPN",
        "PPTP",
        "L2TP",
        "IPSec",
        "IKEv2",
        "SoftEther",
        "SSTP",
        "VPNClient",
        "ExpressVPN",
        "NordVPN",
        "CyberGhost",
        "Surfshark",
        "PrivateInternetAccess",
        "ProtonVPN",
        "HideMyAss",
        "VyprVPN",
        "Tunnelbear",
        "Hotspot Shield",
        "StrongVPN",
        "SaferVPN",
        "PureVPN",
        "IVPN",
        "TorGuard"
    ];

    public bool IsUseVpnTunnel()
    {
        var interfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var ni in interfaces.Where(c => c.OperationalStatus == OperationalStatus.Up))
        {
            if (ni.NetworkInterfaceType is (NetworkInterfaceType)53 or NetworkInterfaceType.Tunnel)
            {
                return true;
            }

            if (VpnKeywords.Any(keyword =>
                    ni.Description.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                    ni.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }

            if (ni is { NetworkInterfaceType: NetworkInterfaceType.Ppp })
            {
                return true;
            }
        }

        return false;
    }
}
