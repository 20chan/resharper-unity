using System;
using JetBrains.Annotations;
using JetBrains.Core;
using JetBrains.Lifetimes;
using JetBrains.Platform.Unity.EditorPluginModel;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Tasks;
using JetBrains.ReSharper.Features.Inspections.Bookmarks.NumberedBookmarks;
using JetBrains.ReSharper.Features.XamlRendererHost.Preview;
using JetBrains.ReSharper.Host.Features.Notifications;
using JetBrains.ReSharper.Host.Features.ProjectModel;
using JetBrains.ReSharper.Host.Features.TextControls;
using JetBrains.ReSharper.Plugins.Unity.AsmDefNew.Psi.Caches;
using JetBrains.Util;
using JetBrains.Util.Extension;

namespace JetBrains.ReSharper.Plugins.Unity.Rider.Notifications
{
    [SolutionComponent]
    public class GeneratedFileNotification
    {
        private readonly NotificationPanelHost myNotificationPanelHost;

        public GeneratedFileNotification(Lifetime lifetime, UnityHost unityHost, ConnectionTracker connectionTracker, UnityEditorProtocol editorProtocol, ISolution solution,
            AsmDefNameCache asmDefNameCache, [CanBeNull] TextControlHost textControlHost = null, [CanBeNull] SolutionLifecycleHost solutionLifecycleHost = null,
            [CanBeNull] NotificationPanelHost notificationPanelHost = null)
        {
            if (solutionLifecycleHost == null)
                return;
            
            myNotificationPanelHost = notificationPanelHost;
                
            solutionLifecycleHost.FullStartupFinished.Advise(lifetime, _ =>
            {
                textControlHost.ViewHostTextControls(lifetime, (lt, id, host) =>
                {
                    var projectFile = host.ToProjectFile(solution);
                    if (projectFile == null)
                        return;

                    if (projectFile.Location.ExtensionNoDot.Equals("csproj", StringComparison.OrdinalIgnoreCase))
                    {
                        connectionTracker.State.View(lt, (unityStateLifetime, state) =>
                        {
                            var name = projectFile.Location.NameWithoutExtension;
                            var path = asmDefNameCache.GetPathFor(name)?.TryMakeRelativeTo(solution.SolutionFilePath);

                            var elements = new LocalList<INotificationPanelHyperlink>();
                            if (path != null && state != UnityEditorState.Disconnected)
                            {
                                var strPath = path.Components.Join("/").RemoveStart("../");
                                elements.Add(new NotificationPanelCallbackHyperlink(unityStateLifetime, "Edit corresponding .asmdef in Unity", false,
                                    () =>
                                    {
                                        unityHost.PerformModelAction(t => t.AllowSetForegroundWindow.Start(unityStateLifetime, Unit.Instance).Result.Advise(unityStateLifetime,
                                            __ =>
                                            {
                                                editorProtocol.UnityModel.Value?.ShowFileInUnity.Fire(strPath);
                                            }));
                                    }));
                            }
                        
                            notificationPanelHost.AddNotificationPanel(unityStateLifetime, host, new NotificationPanel("This file is generated by Unity. Any changes made will be lost.", "UnityGeneratedFile", elements.ToArray()));
                        });

                    }
                });
            });
        }
    }
}