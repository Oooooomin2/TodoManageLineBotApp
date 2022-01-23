Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Interface ISendLineAsync
    Function SendMessageAsync(sendData As LineMessageApi) As Task
End Interface

Public Class SendLineAsync
    Implements ISendLineAsync

    Private ReadOnly _lineMessageApiUrl As String
    Private ReadOnly _accessToken As String

    Private ReadOnly _httpclient As New HttpClient()

    Public Sub New()
        _lineMessageApiUrl = ConfigurationManager.AppSettings("LineMessageApiUrl")
        _accessToken = ConfigurationManager.AppSettings("AccessToken")
    End Sub

    Public Async Function SendMessageAsync(sendData As LineMessageApi) As Task Implements ISendLineAsync.SendMessageAsync
        Dim json = New StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json")

        _httpclient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", _accessToken)
        _httpclient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"))
        Await _httpclient.PostAsync(_lineMessageApiUrl, json)
    End Function
End Class
