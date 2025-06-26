namespace Blazor.Diagrams.Core.Events;

public record TouchEventArgs(TouchPoint[] ChangedTouches, bool CtrlKey, bool ShiftKey, bool AltKey);
public record TouchPoint(long Identifier, double ScreenX, double ScreenY, double ClientX, double ClientY, double PageX, double PageY);
