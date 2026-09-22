
public partial class TheSpredsheetEdmundHates
{
    public unsafe static delegate* unmanaged<nint, char*, nuint, nint> _assignString;
    unsafe static delegate* unmanaged<nint, nint, nint> _setText;
    unsafe static delegate* unmanaged<nint, nint, nint> _getChild;
    unsafe static delegate* unmanaged<nint, nint, nint> _getChildByPath;
    unsafe static delegate* unmanaged<nint, nint, nint, void> _trackMovieclipChildParentOffset;
    unsafe static delegate* unmanaged<nint, nint, uint, void> _attachChild;
    unsafe static delegate* unmanaged<nint, nint, nint> _createCatStatsDrawer;
    unsafe static delegate* unmanaged<nint, nint, nint> _getSpellIdFromButton;
    static unsafe delegate* unmanaged<nint, nint, nint> _createMenuPanel;
    static unsafe delegate* unmanaged<nint, nint, void> _assingRoomFn;
    static unsafe delegate* unmanaged<nint, nint> _getRenderer;
    static unsafe delegate* unmanaged<nint, nint> _createEntity;
    static nint _audioSourceVtable;
    unsafe static delegate* unmanaged<nint, double*, double*, nint, double, void> _transformPoint;
    unsafe static delegate* unmanaged<nint, float*, void> _getWorldTransform;

    internal unsafe void InitNativeFunctions() {
        _assignString = (delegate* unmanaged<nint, char*, nuint, nint>)(MewjectorApi.GameBase + 0x5b150);
        _getChild = (delegate* unmanaged<nint, nint, nint>)(MewjectorApi.GameBase + 0x99a0e0);
        _getChildByPath = (delegate* unmanaged<nint, nint, nint>)(MewjectorApi.GameBase + 0x99a1b0);
        _setText = (delegate* unmanaged<nint, nint, nint>)(MewjectorApi.GameBase + 0x98E8A0);
        _trackMovieclipChildParentOffset = (delegate* unmanaged<nint, nint, nint, void>)(MewjectorApi.GameBase + (nuint)0x204a80);
        _attachChild = (delegate* unmanaged<nint, nint, uint, void>)(MewjectorApi.GameBase + (nuint)0x999e40);
        _createCatStatsDrawer = (delegate* unmanaged<nint, nint, nint>)(MewjectorApi.GameBase + (nuint)0x1ace50);
        _getSpellIdFromButton = (delegate* unmanaged<nint, nint, nint>)(MewjectorApi.GameBase + (nuint)0x60e30);
        _getRenderer = (delegate* unmanaged<nint, nint>)(MewjectorApi.GameBase + 0x224cd0);
        _createEntity = (delegate* unmanaged<nint, nint>)(MewjectorApi.GameBase + 0x96b3e0);
        _audioSourceVtable = (nint)(MewjectorApi.GameBase + 0xED12A0);
        _assingRoomFn = (delegate* unmanaged<nint, nint, void>)(MewjectorApi.GameBase + 0x2e88d0);
        _createMenuPanel = (delegate* unmanaged<nint, nint, nint>) (MewjectorApi.GameBase + 0xe9340);

        _getWorldTransform = 
            (delegate* unmanaged<nint, float*, void>)(MewjectorApi.GameBase + (nuint)0x9b1880);
        _transformPoint =
            (delegate* unmanaged<nint, double*, double*, nint, double, void>)
            (MewjectorApi.GameBase + (nuint)0x97b130);

    }
}