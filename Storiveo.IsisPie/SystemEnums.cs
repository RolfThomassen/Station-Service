namespace Storiveo.IsisPie
{
    public enum MessageType
    {
        Unknown = ' ',
        PumpStatus = 2,
        AuthorizationInfo = 'D',
        ReserveStatus = 'M'
    }

    public enum MessageCommand
    {
        ReceivedAcknowledge = 'E',
        Reserve = 'M',
        Authorize = 'A',
        GetFinalization = 'B',
        RequestPumpStatus = 'P',
        PumpCancel = 'B',
        PumpCancel2 = 'C'
    }

    public enum PumpStatus
    {
        Init = 1,
        Idle,
		DoNothing,
        NoRespond,
        NoAuthorise,
		PumpBlocked,
		Reserved,
        Authorized,
        StartedDispend,
        Dispensing,
        DispenseCompleted
    }

    public enum RequestPumpStatus
    {
        IdleNoRequest = 1,
        RequestReserve,
        RequestAutorise,
        RequestStartedDispense,
        RequestRunning,
        RequestDispenseCompleted,
        RequestFinalize,
        RequestCancel
    }
}
