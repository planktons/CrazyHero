using System.Runtime.InteropServices;
using UnityEngine;

public class AdMobPlugin_iOSNativeBridge {

    // These are the interface to native implementation calls for iOS.
    [DllImport("__Internal")]
    extern static public void _SetCallbackHandlerName(string handlerName);

    [DllImport("__Internal")]
    private static extern void _CreateBannerView(string publisherId,
                                                 string adSize,
                                                 bool positionAtTop);

    [DllImport("__Internal")]
    private static extern void _RequestBannerAd(bool isTesting, string extras);

    [DllImport("__Internal")]
    private static extern void _HideBannerView();

    [DllImport("__Internal")]
    private static extern void _ShowBannerView();

	/// <summary>
	/// Mz_get implemented this.
	/// </summary>
	[DllImport("__Internal")]
	private static extern void _SetScreen(float width, float height);
	public static void SetScreen(float width, float height) {
		_SetScreen(width, height);
	}

    public static void CreateBannerView(string publisherId, string adSize, bool positionAtTop)
	{
        _CreateBannerView(publisherId, adSize, positionAtTop);
    }

    public static void RequestBannerAd(bool isTesting, string extras)
	{
        _RequestBannerAd(isTesting, extras);
    }

    public static void SetCallbackHandlerName(string callbackHandlerName)
	{
        _SetCallbackHandlerName(callbackHandlerName);
    }

    public static void HideBannerView()
	{
        _HideBannerView();
    }

    public static void ShowBannerView()
	{
        _ShowBannerView();
    }
}
