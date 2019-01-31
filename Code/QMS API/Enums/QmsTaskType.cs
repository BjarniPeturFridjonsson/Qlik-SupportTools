namespace QMS_API.Enums
{
    //changes to this needs to be reflected in Valhalla.Model.Enums.QmsTaskType
    public enum QmsTaskType
    {
        Undefined = 0,
        DocumentTask = 1,
        ExternalProgramTask = 2,
        DbCommandTask = 3,
        PauseTask = 4,
        QvdCreationTask = 5,
        TemplateTask = 6,
    }
}
