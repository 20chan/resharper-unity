package com.jetbrains.rider.plugins.unity.toolWindow

import com.intellij.notification.NotificationGroup
import com.intellij.openapi.components.AbstractProjectComponent
import com.intellij.openapi.project.Project
import com.jetbrains.rider.plugins.unity.ProjectCustomDataHost
import com.jetbrains.rider.util.idea.ILifetimedComponent
import com.jetbrains.rider.util.idea.LifetimedComponent
import com.jetbrains.rider.util.idea.getLogger
import com.jetbrains.rider.util.reactive.viewNotNull

class UnityToolWindowManager(private val project: Project,
                             private val projectCustomDataHost: ProjectCustomDataHost,
                             private val unityToolWindowFactory: UnityToolWindowFactory)
    : AbstractProjectComponent(project), ILifetimedComponent by LifetimedComponent(project) {
    companion object {
        val BUILD_NOTIFICATION_GROUP = NotificationGroup.toolWindowGroup("Unity Console Messages", UnityToolWindowFactory.TOOLWINDOW_ID)
        private val myLogger = getLogger<UnityToolWindowManager>()
    }

    init {
        projectCustomDataHost.unitySession.viewNotNull(componentLifetime) { sessionLifetime, _ ->
            myLogger.info("new session")
            sessionLifetime.add {
                myLogger.info("terminate")
            }
            val context = unityToolWindowFactory.getOrCreateContext()
            //context.clear()
            val shouldReactivateBuildToolWindow = context.isActive


            if (shouldReactivateBuildToolWindow) {
                context.activateToolWindowIfNotActive()
            }
        }

        projectCustomDataHost.logSignal.advise(componentLifetime) { message ->
            val context = unityToolWindowFactory.getOrCreateContext()

            context.addEvent(message)
        }
    }
}

