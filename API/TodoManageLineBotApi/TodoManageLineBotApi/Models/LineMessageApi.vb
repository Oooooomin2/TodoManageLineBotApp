Imports Newtonsoft.Json

Public Class LineMessageApi

    <JsonProperty("to")>
    Public Property UserId() As String

    <JsonProperty("messages")>
    Public Property Messages() As Messages()

End Class

Public Class Messages

    <JsonProperty("type")>
    Public Property Type() As String

    <JsonProperty("text")>
    Public Property Text() As String

End Class