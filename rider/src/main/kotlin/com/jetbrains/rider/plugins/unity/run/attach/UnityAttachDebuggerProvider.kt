package com.jetbrains.rider.plugins.unity.run.attach

import com.intellij.execution.process.ProcessInfo
import com.intellij.openapi.project.Project
import com.intellij.openapi.util.UserDataHolder
import com.intellij.xdebugger.attach.*

class UnityAttachDebuggerProvider : XAttachDebuggerProvider {
    override fun getAvailableDebuggers(project: Project, host: XAttachHost, process: ProcessInfo, userData: UserDataHolder): MutableList<XAttachDebugger> {
        if (UnityRunUtil.isUnityEditorProcess(process))
            return mutableListOf(UnityAttachDebugger())
        return mutableListOf()
    }

    override fun isAttachHostApplicable(host: XAttachHost) = host is LocalAttachHost
    override fun getPresentationGroup(): XAttachPresentationGroup<ProcessInfo> = UnityAttachPresentationGroup
}