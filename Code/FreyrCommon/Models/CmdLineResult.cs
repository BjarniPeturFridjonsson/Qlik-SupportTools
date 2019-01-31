namespace FreyrCommon.Models
{
    public enum CmdLineExecType
    {
        Regular,FileContent,FileCrawling
    }
    public class CmdLineResult
    {
        public string Cmd { get; set; }
        public string Name { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public CmdLineExecType ExecType { get; set; } = CmdLineExecType.Regular;
        public bool RunComplete { get; set; }
        public int CmdExitCode { get; set; } = -1;
    }
}
