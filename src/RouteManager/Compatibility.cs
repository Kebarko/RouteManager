namespace KE.MSTS.RouteManager;

/// <summary>
/// Specifies the compatibility of two routes in Train Simulator.
/// </summary>
internal enum Compatibility
{
    /// <summary>
    /// Compatibility is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// Full compatibility: routes can co-exist with each other in Train Simulator.
    /// </summary>
    Full,

    /// <summary>
    /// Partial compatibility: routes cannot co-exist with each other in Train Simulator but share common Global or Sound folders.
    /// </summary>
    Partial,

    /// <summary>
    /// No compatibility: routes cannot co-exist with each other in Train Simulator.
    /// </summary>
    None
}
