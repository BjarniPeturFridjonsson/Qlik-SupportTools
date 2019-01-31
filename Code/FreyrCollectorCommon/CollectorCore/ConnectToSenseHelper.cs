using System;
using FreyrCommon.Logging;
using SenseApiLibrary;

namespace FreyrCollectorCommon.CollectorCore
{
    public class ConnectToSenseHelper
    {
        private readonly ILogger _logger;

        public ConnectToSenseHelper(ILogger logger)
        {
            _logger = logger;
        }

        public SenseConnectDto ConnectToSenseApi(SenseConnectDto dto)
        {
            dto = TryAccessSenseApi(dto);
            if (dto.SenseServerLocationFinderStatus == SenseServerLocationFinderStatus.Success)
                return dto;
            dto = dto.ConnectToSenseApiManuallyDlg(dto); //ha ha
            return dto;
        }

        public SenseConnectDto TryAccessSenseApi(SenseConnectDto dto)
        {
            try
            {
                _logger.Add($"Trying connecting to Sense Server on {dto.SenseHostName}.");
                dto.SenseApiSupport = SenseApiSupport.Create(dto.SenseHostName);
                dto.SenseServerLocationFinderStatus = SenseServerLocationFinderStatus.Success;
                return dto;
            }
            catch (Exception ex)
            {
                if (ex is AggregateException agrEx)
                {
                    dto.SenseServerLocationFinderStatus = SenseServerLocationFinderStatus.NotAccessable;
                    foreach (var item in agrEx.InnerExceptions)
                    {
                        if (item.Message.Contains("403"))
                            dto.SenseServerLocationFinderStatus = SenseServerLocationFinderStatus.Forbidden;
                    }
                }
                else
                {
                    dto.SenseServerLocationFinderStatus =
                        ex.Message.Contains("No valid Qlik Sense client certificate found.")
                            ? SenseServerLocationFinderStatus.NoSertificateFound
                            : SenseServerLocationFinderStatus.NotAccessable;
                }
                _logger.Add($"TryAccessSenseApi failed locating api on machine {dto.SenseHostName} with status {dto.SenseServerLocationFinderStatus} and exception {ex}");
                if (dto.SenseServerLocationFinderStatus == SenseServerLocationFinderStatus.Undefined)
                    dto.SenseServerLocationFinderStatus = SenseServerLocationFinderStatus.UnknownFailure;
                return dto;
            }
            
        }
    }
}
