using ElevatorMusic.Playback;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ElevatorMusic
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    ///    
    [Guid("7df0d6a2-ecd0-43e5-88e4-e4a037d0084b")]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("Build Elevator Music", "Plays elevator music when building solutions and projects.", "1.0")]
    // The following line will schedule the package to be initialized when a solution is being opened
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class ElevatorMusicPackage : AsyncPackage
    {
        /// <summary>
        /// ElevatorMusicPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "68fb2f81-f553-413b-a3e9-b0cadc68e530";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {            
            await InitPlaybackEventActionsAsync(await GetSoundShufflerAsync(), cancellationToken);
        }
        #endregion

        #region Playback
        private async Task<SoundShuffler> GetSoundShufflerAsync()
        {
            var soundShuffler = new SoundShuffler();

            // Add music files
            await Task.Run(() => 
            {
                var directoryPath = Environment.CurrentDirectory + "/Resources/Music/";
                string[] filePaths = Directory.GetFiles(@directoryPath, "*.wav", SearchOption.TopDirectoryOnly);
                foreach (string filePath in filePaths)
                {
                    soundShuffler.AddSound(filePath);
                }
            });

            return soundShuffler;
        }

        private async Task InitPlaybackEventActionsAsync(SoundShuffler soundShuffler, CancellationToken cancellationToken)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Play elevator music when the build starts and stop the playback when the build is finished
            var dte = await GetServiceAsync(typeof(DTE)) as DTE;
            Assumes.Present(dte);
            dte.Events.BuildEvents.OnBuildBegin += (vsBuildScope Scope, vsBuildAction Action) => soundShuffler.ShufflePlayAsync(true).ConfigureAwait(true);
            dte.Events.BuildEvents.OnBuildDone += (vsBuildScope Scope, vsBuildAction Action) => soundShuffler.StopPlayback();
        }
        #endregion

    }
}
