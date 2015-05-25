using System.ServiceModel.Channels;

namespace AppPlatform.Core.EnterpriseLibrary.Services.Context
{
    public interface IMessageHeaderBuilder
    {
        void AppendHeader(MessageHeaders messageHeader);
        MessageHeader GetHeaderToAppend();
    }
}
