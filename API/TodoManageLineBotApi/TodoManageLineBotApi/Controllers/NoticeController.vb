Imports System.Globalization
Imports System.IO
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Http
Imports Azure.Storage.Blobs
Imports CsvHelper
Imports Newtonsoft.Json

Public Class NoticeController
    Inherits ApiController

    Private ReadOnly _containerName As String
    Private ReadOnly _blobName As String
    Private ReadOnly _tmpPath As String
    Private ReadOnly _connectionString As String
    Private ReadOnly _lineMessageApiUrl As String
    Private ReadOnly _userId As String
    Private ReadOnly _accessToken As String

    Private ReadOnly _httpclient As New HttpClient()

    Public Sub New()
        _containerName = ConfigurationManager.AppSettings("ContainerName")
        _blobName = ConfigurationManager.AppSettings("BlobName")
        _tmpPath = ConfigurationManager.AppSettings("TmpPath")
        _connectionString = ConfigurationManager.AppSettings("ConnectionString")
        _lineMessageApiUrl = ConfigurationManager.AppSettings("LineMessageApiUrl")
        _userId = ConfigurationManager.AppSettings("UserId")
        _accessToken = ConfigurationManager.AppSettings("AccessToken")
    End Sub

    ' GET api/todos
    <Route("api/undotodos")>
    Public Async Function GetUndoTodos() As Threading.Tasks.Task(Of ActionResult)
        Dim container As New BlobContainerClient(_connectionString, _containerName)
        Dim blob As BlobClient = container.GetBlobClient(_blobName)
        Await blob.DownloadToAsync(_tmpPath + "/Todos.csv")

        '最終的に返却するListを生成
        Dim undoTodos As New List(Of Todos)()

        Using reader = New StreamReader(_tmpPath + "/Todos.csv")
            Using csv = New CsvReader(reader, CultureInfo.InvariantCulture)
                Dim groupTodosByTitleAndDate = csv.GetRecords(Of Todos) _
                    .ToList() _
                    .Where(Function(o) Date.Parse(o.ImplementationDate).Date = Date.Now.Date) _
                    .GroupBy(Function(o) Tuple.Create(o.Title, o.ImplementationDate))

                For Each groupTodos In groupTodosByTitleAndDate
                    '完了となっていないもののみ通知対象とする
                    If groupTodos.Count() = 1 Then
                        undoTodos.Add(groupTodos.First())
                    End If
                Next

            End Using
        End Using

        If Not undoTodos.Any() Then
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

        Dim SendData As New LineMessageApi With
        {
            .UserId = _userId,
            .Messages =
            {
                New Messages With
                {
                    .Type = "text",
                    .Text = sb.ToString()
                }
            }
        }
        Dim json = New StringContent(JsonConvert.SerializeObject(SendData), Encoding.UTF8, "application/json")

        _httpclient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", _accessToken)
        _httpclient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"))
        Dim res = Await _httpclient.PostAsync(_lineMessageApiUrl, json)
        Return New JsonResult With
        {
            .Data = "Ok"
        }
    End Function

    <Route("api/isexists-today-todo")>
    Public Async Function GetIsExistsTodayTodo() As Threading.Tasks.Task(Of ActionResult)
        Dim container As New BlobContainerClient(_connectionString, _containerName)
        Dim blob As BlobClient = container.GetBlobClient(_blobName)
        Await blob.DownloadToAsync(_tmpPath + "/Todos.csv")

        Using reader = New StreamReader(_tmpPath + "/Todos.csv")
            Using csv = New CsvReader(reader, CultureInfo.InvariantCulture)
                Dim isExistsTodayTodo = csv.GetRecords(Of Todos) _
                    .ToList() _
                    .Any()

                If isExistsTodayTodo Then
                    Return New JsonResult With
                    {
                        .Data = "Not Target"
                    }
                End If

                Dim sb As New StringBuilder()
                sb.AppendLine("まだ今日のタスクが登録されていないぞ！！")
                sb.AppendLine("怠けちゃいかん！！")

                Dim SendData As New LineMessageApi With
                {
                    .UserId = _userId,
                    .Messages =
                    {
                        New Messages With
                        {
                            .Type = "text",
                            .Text = sb.ToString()
                        }
                    }
                }
                Dim json = New StringContent(JsonConvert.SerializeObject(SendData), Encoding.UTF8, "application/json")

                _httpclient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", _accessToken)
                _httpclient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"))
                Dim res = Await _httpclient.PostAsync(_lineMessageApiUrl, json)
                Return New JsonResult With
                {
                    .Data = "Ok"
                }
            End Using
        End Using
    End Function
End Class
