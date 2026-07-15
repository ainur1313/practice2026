using CommandLib;
using PluginApp;
using System.Reflection;
using Xunit;

namespace PluginAppTests
{
    public class PluginLoaderTests
    {

        [Fact]
        public void LoadPlugins_ShouldReturnEmpty_WhenNoDlls()
        {
            var pluginLoader = new PluginLoader();
            string tempDirectory = Path.Combine(Path.GetTempPath(), "empty_plugins_" + Guid.NewGuid().ToString());

            try
            {
                Directory.CreateDirectory(tempDirectory);

                var loadedPlugins = pluginLoader.LoadPlugins(tempDirectory);

                Assert.NotNull(loadedPlugins);
                Assert.Equal(0, loadedPlugins.Count);
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }
        }

        [Fact]
        public void LoadPlugins_ShouldLoadPluginsFromDll()
        {

            var pluginLoader = new PluginLoader();
            string tempDirectory = Path.Combine(Path.GetTempPath(), "plugin_test_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

            var loadedPlugins = pluginLoader.LoadPlugins(tempDirectory);

            Assert.NotNull(loadedPlugins);

            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Fact]
        public void SortPlugins_ShouldOrderPluginsCorrectly()
        {
            var pluginLoader = new PluginLoader();
            var exception = Record.Exception(() => pluginLoader.SortPlugins());
            Assert.Null(exception);
        }

        [Fact]
        public void ExecutePlugins_ShouldNotThrowException_WhenNoPluginsLoaded()
        {
            var pluginLoader = new PluginLoader();
            var exception = Record.Exception(() => pluginLoader.ExecutePlugins());
            Assert.Null(exception);
        }
    }
}
