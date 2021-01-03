Imports CommandLine
Imports Intersect.Utilities

Namespace Intersect.Examples.ClientPlugin
    ''' <summary>
    ''' Example immutable command line options structure.
    ''' </summary>
    Public Structure ExampleCommandLineOptions
        Implements IEquatable(Of ExampleCommandLineOptions)

        Public Sub New(ByVal exampleFlag As Boolean, ByVal exampleVariable As Integer)
            Me.ExampleFlag = exampleFlag
            Me.ExampleVariable = exampleVariable
        End Sub

        ''' <summary>
        ''' Flag that is true if the application was started with --example-flag
        ''' </summary>
        <[Option]("example-flag", [Default] := False, Required := False)>
        Public ReadOnly Property ExampleFlag As Boolean

        ''' <summary>
        ''' Integer argument that corresponds to --example-variable
        ''' </summary>
        <[Option]("example-variable", [Default] := 100, Required := False)>
        Public ReadOnly Property ExampleVariable As Integer

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return TypeOf obj Is ExampleCommandLineOptions And Equals(DirectCast(obj, ExampleCommandLineOptions))
        End Function

        Public Overloads Function Equals(other As ExampleCommandLineOptions) As Boolean _
            Implements IEquatable(Of ExampleCommandLineOptions).Equals
            Return ExampleFlag = other.ExampleFlag AndAlso ExampleVariable = other.ExampleVariable
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return ValueUtils.ComputeHashCode(ExampleFlag, ExampleVariable)
        End Function

        Public Shared Operator =(ByVal left As ExampleCommandLineOptions, ByVal right As ExampleCommandLineOptions) _
            As Boolean
            Return left.Equals(right)
        End Operator

        Public Shared Operator <>(ByVal left As ExampleCommandLineOptions, ByVal right As ExampleCommandLineOptions) _
            As Boolean
            Return Not (left = right)
        End Operator
    End Structure
End Namespace
