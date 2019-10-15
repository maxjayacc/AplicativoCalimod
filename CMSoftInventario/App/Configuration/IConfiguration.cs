using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CMSoftInventario.App.Configuration
{
    public interface IConfigurationManager
    {
        Task<Config> GetAsync(CancellationToken cancellationToken);
    }

    public interface IConfigurationStreamProvider : IDisposable
    {
        Task<Stream> GetStreamAsync();
    }

    public interface IConfigurationStreamProviderFactory
    {
        IConfigurationStreamProvider Create();
    }

}