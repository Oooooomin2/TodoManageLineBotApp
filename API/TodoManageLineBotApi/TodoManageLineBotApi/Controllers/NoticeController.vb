Imports System.Web.Http
Imports Microsoft.ApplicationInsights

Public Class NoticeController
    Inherits ApiController

    Private ReadOnly _resolveAzureBlobsRepository As IResolveAzureBlobsRepository
    Private ReadOnly _resolveTodoService As IResolveTodoService
    Private ReadOnly _sendLineAsync As ISendLineAsync
    Private ReadOnly _logger As TelemetryClient

    Public Sub New()
        _resolveAzureBlobsRepository = New ResolveAzureBlobsRepository()
        _resolveTodoService = New ResolveTodoService()
        _sendLineAsync = New SendLineAsync()
        _logger = New TelemetryClient()
    End Sub

    ' GET api/undotodos
    <Route("api/undotodos")>
    Public Async Function GetUndoTodos() As Threading.Tasks.Task(Of ActionResult)
        Await _resolveAzureBlobsRepository.DownloadCsvAsync()

        Dim undoTodos = _resolveTodoService.GetUndoTodos()
        If Not undoTodos.Any() Then
            _logger.TrackEvent("対象のデータはありません。")
            Return New JsonResult With
            {
                .Data = "Not Target"
            }
        End If

        Dim sb As New StringBuilder()
        sb.AppendLine("まだタスクが残ってるぞおおおおおおお！")
        sb.AppendLine("")

        For Each todo In undoTodos
            sb.AppendLine($"●{todo.Title}")
        Next

        sb.AppendLine("")
        sb.AppendLine("しっかりこなすのだ！！！！")

        Dim sendData = _resolveTodoService.CreateSendData(sb.ToString())
        Try
            _logger.TrackEvent("メッセージを送ります。")
            Await _sendLineAsync.SendMessageAsync(sendData)
            Return New JsonResult With
            {
                .Data = "Ok"
            }
        Catch ex As Exception
            _logger.TrackException(ex)
            Throw
        End Try
    End Function

    <Route("api/isexists-today-todo")>
    Public Async Function GetIsExistsTodayTodo() As Threading.Tasks.Task(Of ActionResult)
        Await _resolveAzureBlobsRepository.DownloadCsvAsync()

        Dim isExistsTodayTodo = _resolveTodoService.IsExistTodayTodo()
        If isExistsTodayTodo Then
            _logger.TrackEvent("対象のデータはありません。")
            Return New JsonResult With
            {
                .Data = "Not Target"
            }
        End If

        Dim sb As New StringBuilder()
        sb.AppendLine("まだ今日のタスクが登録されていないぞ！！")
        sb.AppendLine("怠けちゃいかん！！")

        Dim sendData = _resolveTodoService.CreateSendData(sb.ToString())
        Try
            _logger.TrackEvent("メッセージを送ります。")
            Await _sendLineAsync.SendMessageAsync(sendData)

            Return New JsonResult With
            {
                .Data = "Ok"
            }
        Catch ex As Exception
            _logger.TrackException(ex)
            Throw
        End Try

    End Function
End Class
