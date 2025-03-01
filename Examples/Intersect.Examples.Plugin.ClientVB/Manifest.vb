Imports Intersect.Plugins.Interfaces
Imports Intersect.Plugins.Manifests.Types
Imports Semver


''' <summary>
'''     Defines a plugin manifest in code rather than an embedded manifest.json file.
''' </summary>
Public Class Manifest
    Implements IManifestHelper

    ''' <inheritdoc />
#Disable Warning CA1822
        Public ReadOnly Property Name As String Implements IManifestHelper.Name
#Enable Warning CA1822
        Get
            Return GetType(Manifest).Namespace
        End Get
    End Property

    ''' <inheritdoc />
#Disable Warning CA1822
        Public ReadOnly Property Key As String Implements IManifestHelper.Key
#Enable Warning CA1822
        Get
            Return GetType(Manifest).Namespace
        End Get
    End Property

    ''' <inheritdoc />
#Disable Warning CA1822
        Public ReadOnly Property Version As SemVersion Implements IManifestHelper.Version
#Enable Warning CA1822
        Get
            Return New SemVersion(1)
        End Get
    End Property

    ''' <inheritdoc />
#Disable Warning CA1822
        Public ReadOnly Property Authors As Authors Implements IManifestHelper.Authors
#Enable Warning CA1822
        Get
            Return _
                "Ascension Game Dev <admin@ascensiongamedev.com> (https://github.com/AscensionGameDev/Intersect-Engine)"
        End Get
    End Property

    ''' <inheritdoc />
#Disable Warning CA1822
        Public ReadOnly Property Homepage As String Implements IManifestHelper.Homepage
#Enable Warning CA1822
        Get
            Return "https://github.com/AscensionGameDev/Intersect-Engine"
        End Get
    End Property
End Class