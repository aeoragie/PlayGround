// Aspire gRPC DashboardClient는 Windows 시스템 프록시가 localhost를 우회하지 않으면 실패함
// .NET HttpClient.DefaultProxy 변경만으로는 gRPC의 WinInet 직접 접근을 막을 수 없으므로,
// HKCU 레지스트리의 ProxyOverride에 <local>을 추가하고 WinInet에 즉시 반영
if (OperatingSystem.IsWindows())
{
    try
    {
        using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Internet Settings", writable: true);
        if (key != null)
        {
            var bypass = key.GetValue("ProxyOverride") as string ?? "";
            if (!bypass.Contains("<local>") && !bypass.Contains("localhost"))
            {
                key.SetValue("ProxyOverride", string.IsNullOrEmpty(bypass) ? "<local>" : $"{bypass};<local>");
                NativeMethods.InternetSetOption(nint.Zero, 39, nint.Zero, 0); // INTERNET_OPTION_SETTINGS_CHANGED
                NativeMethods.InternetSetOption(nint.Zero, 37, nint.Zero, 0); // INTERNET_OPTION_REFRESH
            }
        }
    }
    catch { /* 레지스트리 접근 실패 시 무시 */ }
}

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PlayGround_Server>("Playground-Server");

builder.Build().Run();

internal static partial class NativeMethods
{
    [System.Runtime.InteropServices.DllImport("wininet.dll", SetLastError = true)]
    internal static extern bool InternetSetOption(nint hInternet, int dwOption, nint lpBuffer, int dwBufferLength);
}
