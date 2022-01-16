Imports System.Globalization
Imports System.IO
Imports System.Web.Http
Imports Azure.Storage.Blobs
Imports CsvHelper
Imports CsvHelper.Configuration

Public Class TodosController
    Inherits ApiController

    Private ReadOnly _containerName As String
    Private ReadOnly _blobName As String
    Private ReadOnly _tmpPath As String
    Private ReadOnly _connectionString As String

    Public Sub New()
        _containerName = ConfigurationManager.AppSettings("ContainerName")
        _blobName = ConfigurationManager.AppSettings("BlobName")
        _tmpPath = ConfigurationManager.AppSettings("TmpPath")
        _connectionString = ConfigurationManager.AppSettings("ConnectionString")
    End Sub

    ' GET api/todos
    Public Async Function GetTodos() As Threading.Tasks.Task(Of IEnumerable(Of Todos))
        Dim container As New BlobContainerClient(_connectionString, _containerName)
        Dim blob As BlobClient = container.GetBlobClient(_blobName)
        If Not Directory.Exists(_tmpPath) Then
            Directory.CreateDirectory(_tmpPath)
        End If
        Await blob.DownloadToAsync(_tmpPath + "/Todos.csv")

        '最終的に返却するListを生成
        Dim todayTodos As New List(Of Todos)()

        Using reader = New StreamReader(_tmpPath + "/Todos.csv")
            Using csv = New CsvReader(reader, CultureInfo.InvariantCulture)
                Dim groupTodosByTitleAndDate = csv.GetRecords(Of Todos) _
                    .ToList() _
                    .Where(Function(o) Date.Parse(o.ImplementationDate).Date = Date.Now.Date) _
                    .GroupBy(Function(o) Tuple.Create(o.Title, o.ImplementationDate))

                For Each groupTodos In groupTodosByTitleAndDate
                    '未完了と完了のものがある場合は完了のtodoを返却する
                    If groupTodos.Count() > 1 Then
                        todayTodos.Add(groupTodos.Single(Function(o) o.Status = "完"))
                    Else
                        todayTodos.Add(groupTodos.First())
                    End If
                Next

                Return todayTodos
            End Using
        End Using
    End Function

    ' POST api/todos
    Public Async Function PostTodos(<FromBody()> todo As Todos) As Threading.Tasks.Task
        Dim config = New CsvConfiguration(CultureInfo.InvariantCulture) With {
            .HasHeaderRecord = False
        }

        Using stream = File.Open(_tmpPath + "/Todos.csv", FileMode.Append)
            Using writer = New StreamWriter(stream)
                Using csv = New CsvWriter(writer, config)
                    csv.WriteRecords(
                        New List(Of Todos) From
                        {
                            New Todos With
                            {
                                .Title = todo.Title,
                                .ImplementationDate = Date.Parse(todo.ImplementationDate),
                                .Status = todo.Status,
                                .CreatedDate = Date.Now
                            }
                        })
                End Using
            End Using
        End Using

        Dim container As New BlobContainerClient(_connectionString, _containerName)
        Dim blob As BlobClient = container.GetBlobClient(_blobName)
        Await blob.UploadAsync(_tmpPath + "/Todos.csv", True)
    End Function

    ' PUT api/todos/5
    Public Async Function PutTodos(<FromBody()> todo As Todos) As Threading.Tasks.Task
        Dim config = New CsvConfiguration(CultureInfo.InvariantCulture) With {
            .HasHeaderRecord = False
        }

        Using stream = File.Open(_tmpPath + "/Todos.csv", FileMode.Append)
            Using writer = New StreamWriter(stream)
                Using csv = New CsvWriter(writer, config)
                    csv.WriteRecords(
                        New List(Of Todos) From
                        {
                            New Todos With
                            {
                                .Title = todo.Title,
                                .ImplementationDate = Date.Parse(todo.ImplementationDate),
                                .Status = todo.Status,
                                .CreatedDate = Date.Now
                            }
                        })
                End Using
            End Using
        End Using

        Dim container As New BlobContainerClient(_connectionString, _containerName)
        Dim blob As BlobClient = container.GetBlobClient(_blobName)
        Await blob.UploadAsync(_tmpPath + "/Todos.csv", True)
    End Function

    <Route("api/isalive")>
    Public Function GetAlive() As String
        Return Date.Now.ToString()
    End Function
End Class
