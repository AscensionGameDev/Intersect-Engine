Imports Intersect.Plugins.Interfaces
Imports Intersect.Plugins.Manifests.Types
Imports Semver

Namespace Intersect.Examples.ClientPlugin
    ''' <summary>
    ''' Defines a plugin manifest in code rather than an embedded manifest.json file.
    ''' </summary>
    Public Class Manifest
        Implements IManifestHelper

        ''' <inheritdoc />
        Public ReadOnly Property Name As String Implements IManifestHelper.Name
            Get
                Return GetType(Manifest).Namespace
            End Get
        End Property

        ''' <inheritdoc />
        Public ReadOnly Property Key As String Implements IManifestHelper.Key
            Get
                Return GetType(Manifest).Namespace
            End Get
        End Property

        ''' <inheritdoc />
        Public ReadOnly Property Version As SemVersion Implements IManifestHelper.Version
            Get
                Return New SemVersion(1)
            End Get
        End Property

        ''' <inheritdoc />
        Public ReadOnly Property Authors As Authors Implements IManifestHelper.Authors
            Get
                Return _
                    "Ascension Game Dev <admin@ascensiongamedev.com> (https://github.com/AscensionGameDev/Intersect-Engine)"
            End Get
        End Property

        ''' <inheritdoc />
        Public ReadOnly Property Homepage As String Implements IManifestHelper.Homepage
            Get
                Return "https://github.com/AscensionGameDev/Intersect-Engine"
            End Get
        End Property
    End Class
End Namespace

