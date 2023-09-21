using System.Runtime.InteropServices;

namespace Nickvision.Aura.Taskbar;

/// <summary>
/// Windows ITaskbarList3 interface
/// </summary>
/// <remarks>https://www.pinvoke.net/default.aspx/shell32/ITaskbarList3.html</remarks>
[ComImport]
[Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ITaskbarList3
{
    /// <summary>
    /// Initializes the taskbar list object. This method must be called before any other ITaskbarList methods can be called.
    /// </summary>
    public void HrInit();

    /// <summary>
    /// Adds an item to the taskbar.
    /// </summary>
    /// <param name="hwnd">A handle to the window to be added to the taskbar.</param>
    public void AddTab(nint hwnd);

    /// <summary>
    /// Deletes an item from the taskbar.
    /// </summary>
    /// <param name="hwnd">A handle to the window to be deleted from the taskbar.</param>
    public void DeleteTab(nint hwnd);

    /// <summary>
    /// Activates an item on the taskbar. The window is not actually activated; the window's item on the taskbar is merely displayed as active.
    /// </summary>
    /// <param name="hwnd">A handle to the window on the taskbar to be displayed as active.</param>
    public void ActivateTab(nint hwnd);

    /// <summary>
    /// Marks a taskbar item as active but does not visually activate it.
    /// </summary>
    /// <param name="hwnd">A handle to the window to be marked as active.</param>
    public void SetActiveAlt(nint hwnd);

    /// <summary>
    /// Marks a window as full-screen
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="fFullscreen"></param>
    public void MarkFullscreenWindow(nint hwnd, int fFullscreen);

    /// <summary>
    /// Displays or updates a progress bar hosted in a taskbar button to show the specific percentage
    /// completed of the full operation.
    /// </summary>
    /// <param name="hwnd">The handle of the window whose associated taskbar button is being used as
    /// a progress indicator.</param>
    /// <param name="ullCompleted">An application-defined value that indicates the proportion of the
    /// operation that has been completed at the time the method is called.</param>
    /// <param name="ullTotal">An application-defined value that specifies the value ullCompleted will
    /// have when the operation is complete.</param>
    public void SetProgressValue(nint hwnd, ulong ullCompleted, ulong ullTotal);

    /// <summary>
    /// Sets the type and state of the progress indicator displayed on a taskbar button.
    /// </summary>
    /// <param name="hwnd">The handle of the window in which the progress of an operation is being
    /// shown. This window's associated taskbar button will display the progress bar.</param>
    /// <param name="tbpFlags">Flags that control the current state of the progress button. Specify
    /// only one of the following flags; all states are mutually exclusive of all others.</param>
    public void SetProgressState(nint hwnd, ProgressFlags tbpFlags);

    /// <summary>
    /// Informs the taskbar that a new tab or document thumbnail has been provided for display in an
    /// application's taskbar group flyout.
    /// </summary>
    /// <param name="hwndTab">Handle of the tab or document window. This value is required and cannot
    /// be NULL.</param>
    /// <param name="hwndMDI">Handle of the application's main window. This value tells the taskbar
    /// which application's preview group to attach the new thumbnail to. This value is required and
    /// cannot be NULL.</param>
    public void RegisterTab(nint hwndTab, nint hwndMDI);

    /// <summary>
    /// Removes a thumbnail from an application's preview group when that tab or document is closed in the application.
    /// </summary>
    /// <param name="hwndTab">The handle of the tab window whose thumbnail is being removed. This is the same
    /// value with which the thumbnail was registered as part the group through ITaskbarList3::RegisterTab.
    /// This value is required and cannot be NULL.</param>
    public void UnregisterTab(nint hwndTab);

    /// <summary>
    /// Inserts a new thumbnail into a tabbed-document interface (TDI) or multiple-document interface
    /// (MDI) application's group flyout or moves an existing thumbnail to a new position in the
    /// application's group.
    /// </summary>
    /// <param name="hwndTab">The handle of the tab window whose thumbnail is being placed. This value
    /// is required, must already be registered through ITaskbarList3::RegisterTab, and cannot be NULL.</param>
    /// <param name="hwndInsertBefore">The handle of the tab window whose thumbnail that hwndTab is
    /// inserted to the left of. This handle must already be registered through ITaskbarList3::RegisterTab.
    /// If this value is NULL, the new thumbnail is added to the end of the list.</param>
    public void SetTabOrder(nint hwndTab, nint hwndInsertBefore);

    /// <summary>
    /// Informs the taskbar that a tab or document window has been made the active window.
    /// </summary>
    /// <param name="hwndTab">Handle of the active tab window. This handle must already be registered
    /// through ITaskbarList3::RegisterTab. This value can be NULL if no tab is active.</param>
    /// <param name="hwndMDI">Handle of the application's main window. This value tells the taskbar
    /// which group the thumbnail is a member of. This value is required and cannot be NULL.</param>
    /// <param name="tbatFlags">None, one, or both of the following values that specify a thumbnail
    /// and peek view to use in place of a representation of the specific tab or document.</param>
    public void SetTabActive(nint hwndTab, nint hwndMDI, uint tbatFlags);

    /// <summary>
    /// Adds a thumbnail toolbar with a specified set of buttons to the thumbnail image of a window in a
    /// taskbar button flyout.
    /// </summary>
    /// <param name="hwnd">The handle of the window whose thumbnail representation will receive the toolbar.
    /// This handle must belong to the calling process.</param>
    /// <param name="cButtons">The number of buttons defined in the array pointed to by pButton. The maximum
    /// number of buttons allowed is 7.</param>
    /// <param name="pButton">A pointer to an array of THUMBBUTTON structures. Each THUMBBUTTON defines an
    /// individual button to be added to the toolbar. Buttons cannot be added or deleted later, so this must
    /// be the full defined set. Buttons also cannot be reordered, so their order in the array, which is the
    /// order in which they are displayed left to right, will be their permanent order.</param>
    public void ThumbBarAddButtons(
        nint hwnd,
        uint cButtons,
        [MarshalAs(UnmanagedType.LPArray)] nint pButton);

    public void ThumbBarUpdateButtons(
        nint hwnd,
        uint cButtons,
        [MarshalAs(UnmanagedType.LPArray)] nint pButton);

    /// <summary>
    /// Specifies an image list that contains button images for a toolbar embedded in a thumbnail image of a
    /// window in a taskbar button flyout.
    /// </summary>
    /// <param name="hwnd">The handle of the window whose thumbnail representation contains the toolbar to be
    /// updated. This handle must belong to the calling process.</param>
    /// <param name="himl">The handle of the image list that contains all button images to be used in the toolbar.</param>
    public void ThumbBarSetImageList(nint hwnd, nint himl);

    /// <summary>
    /// Applies an overlay to a taskbar button to indicate application status or a notification to the user.
    /// </summary>
    /// <param name="hwnd">The handle of the window whose associated taskbar button receives the overlay.
    /// This handle must belong to a calling process associated with the button's application and must be
    /// a valid hwnd or the call is ignored.</param>
    /// <param name="hIcon">The handle of an icon to use as the overlay. This should be a small icon,
    /// measuring 16x16 pixels at 96 dots per inch (dpi). If an overlay icon is already applied to the
    /// taskbar button, that existing overlay is replaced.</param>
    /// <param name="pszDescription">A pointer to a string that provides an alt text version of the
    /// information conveyed by the overlay, for accessibility purposes.</param>
    public void SetOverlayIcon(nint hwnd, nint hIcon, string pszDescription);

    /// <summary>
    /// Specifies or updates the text of the tooltip that is displayed when the mouse pointer rests on an
    /// individual preview thumbnail in a taskbar button flyout.
    /// </summary>
    /// <param name="hwnd">The handle to the window whose thumbnail displays the tooltip. This handle must
    /// belong to the calling process.</param>
    /// <param name="pszTip">The pointer to the text to be displayed in the tooltip. This value can be NULL,
    /// in which case the title of the window specified by hwnd is used as the tooltip.</param>
    public void SetThumbnailTooltip(nint hwnd, string pszTip);

    /// <summary>
    /// Selects a portion of a window's client area to display as that window's thumbnail in the taskbar.
    /// </summary>
    /// <param name="hwnd">The handle to a window represented in the taskbar.</param>
    /// <param name="prcClip">A pointer to a RECT structure that specifies a selection within the window's
    /// client area, relative to the upper-left corner of that client area. To clear a clip that is already
    /// in place and return to the default display of the thumbnail, set this parameter to NULL.</param>
    public void SetThumbnailClip(nint hwnd, nint prcClip);
}