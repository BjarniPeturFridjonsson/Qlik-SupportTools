namespace QMS_API.Enums
{
    //changes to this needs to be reflected in Valhalla.Model.Enums.QmsTaskType
    public enum QmsTaskStatusType 
    {
        Undefined = 0,
        Waiting = 1,
        Running = 2,
        Aborting = 3,
        Failed = 4,
        Warning = 5,
        Completed = 6,
    }
}